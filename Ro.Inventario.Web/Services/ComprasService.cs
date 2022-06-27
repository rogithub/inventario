using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Repos;
using Ro.Inventario.Web.Entities;
using System.Linq;
namespace Ro.Inventario.Web.Services;

public interface  IComprasService
{
    Task ProcessModel(CompraNuevosProductos model, IEnumerable<string> lines);
}

public class ComprasService: IComprasService
{    
    private IUnidadMedidaRepo _uMedida;
    private IProductosRepo _producto;
    private IComprasRepo _compras;
    private ICategoriasRepo _categorias;
    private ICategoriasProductosRepo _categoriasProds;
    private IPreciosProductosRepo _precios;
    private IComprasProductosRepo _comprasProductos;
    private INuevosProductosValidatorService _pValidator;
    private readonly ILogger<ComprasService> _logger;
    public ComprasService(
        ILogger<ComprasService> logger,
        IUnidadMedidaRepo unidadMedida,
        IProductosRepo productRepo,
        IComprasRepo comprasRepo,
        ICategoriasRepo categoriasRepo,
        ICategoriasProductosRepo categoriasProductosRepo,
        IPreciosProductosRepo preciosRepo,
        IComprasProductosRepo comprasProductosRepo,
        INuevosProductosValidatorService pValidator)
    {
        _logger = logger;
       _uMedida = unidadMedida;       
       _producto = productRepo;
       _compras = comprasRepo;
       _categorias = categoriasRepo;
       _categoriasProds = categoriasProductosRepo;
       _precios = preciosRepo;
       _comprasProductos = comprasProductosRepo;
        _pValidator = pValidator;
    }    

    public async Task ProcessModel(CompraNuevosProductos model, 
        IEnumerable<string> lines)
    {        
        var productLines = _pValidator.ParseProducts(lines);

        var compra = new Compra();
        compra.Proveedor = model.Proveedor;
        compra.FechaFactura = model.FechaFactura;
        compra.CostoPaqueteria = model.CostoPaqueteria;
        compra.TotalFactura = model.TotalFactura;
        await _compras.Save(compra);
        _logger.LogInformation("Generando compra {id}", compra.Id);
        
        var unidadesMedida = (from p in productLines select p.UnidadDeMedida);
        var uMedidaCounter = await _uMedida.BulkSave(unidadesMedida.ToArray());
        _logger.LogInformation("{uMedidaCounter} Unidades de medida guardadas", uMedidaCounter);        
        var dUnidadMedida = await _uMedida.GetDictionary();

        var categorias = (from p in productLines select p.Categoria);
        var categoriasCounter = await _categorias.BulkSave(categorias.ToArray());
        _logger.LogInformation("{categoriasCounter} Categorias guardadas", categoriasCounter);        
        var dCategorias = await _categorias.GetDictionary();

        var productos = (from p in productLines select p.ToEntity(dUnidadMedida));
        var prodCounter = await _producto.BulkSave(productos.ToArray());
        _logger.LogInformation("{prodCounter} Produtos nuevos guardados", prodCounter);

        var catProds = 
            (from p in productLines
            from c in dCategorias
            select new CategoriaProducto()
            {
                ProductoId = p.Id,
                CategoriaId = dCategorias[p.Categoria]
            });        
        var catProdCounter = await _categoriasProds.BulkSave(catProds.ToArray());
        _logger.LogInformation("{catProdCounter} Produtos asociados a una categoría", catProdCounter);

        var preciosProds = 
            (from p in productLines
            select new PrecioProducto()
            {
                ProductoId = p.Id,
                PrecioVenta = p.PrecioVenta
            });        
        var preciosCounter = await _precios.BulkSave(preciosProds.ToArray());
        _logger.LogInformation("{preciosCounter} Produtos asociados a un precio", preciosCounter);

        var productosComprados = 
            (from p in productLines
            select new CompraProducto()
            {
                ProductoId = p.Id,
                Cantidad = p.Cantidad,
                CompraId = compra.Id,
                PrecioCompra = p.PrecioCompra
            });        
        var productosEnCompra = await _comprasProductos.BulkSave(productosComprados.ToArray());
        _logger.LogInformation("{productosEnCompra} Produtos asociados a esta compra", productosEnCompra);
    }
}
