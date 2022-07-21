using Ro.Inventario.Web.Entities;
using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Repos;
namespace Ro.Inventario.Web.Services;

public interface IVentasService
{
    Task<VentasDiaResponseModel> Load(DateTime fecha);
    Task<int> GuardarAjusteStock(StockAjusteModel stock, Guid userId);
    Task<DevolucionModel> CargarDevolucion(Guid ventaId);
    Task<int> DevolverProductos(DevolucionProducto[] prods);
}

public class VentasService : IVentasService
{
    private IProductosRepo _productos;
    private IAjustesRepo _ventas;
    private IAjustesProductosRepo _ventasProductos;
    private IDevolucionesProductosRepo _devProds;
    private IUnidadMedidaRepo _uMedida;
    private ICategoriasRepo _categorias;
    private ICategoriasProductosRepo _categoriasProds;
    private readonly ILogger<VentasService> _logger;
    public VentasService(
        ILogger<VentasService> logger,
        IProductosRepo productRepo,
        IAjustesProductosRepo ventasProductos,
        IAjustesRepo ventas,
        IUnidadMedidaRepo umedida,
        ICategoriasRepo categorias,
        ICategoriasProductosRepo categoriasProds,
        IDevolucionesProductosRepo devProds)
    {
        _devProds = devProds;
        _categoriasProds = categoriasProds;
        _uMedida = umedida;
        _categorias = categorias;
        _logger = logger;
        _ventas = ventas;
        _productos = productRepo;
        _ventasProductos = ventasProductos;
    }

    public Task<int> DevolverProductos(DevolucionProducto[] prods)
    {        
        return _devProds.BulkSave(prods);
    }

    public async Task<int> GuardarAjusteStock(StockAjusteModel stock, Guid userId)
    {
        var a = new Ajuste();
        a.TipoAjuste = stock.TipoAjuste;
        a.UserUpdatedId = userId;
        await _ventas.Save(a);
        var ap = new AjusteProducto();
        ap.AjusteId = a.Id;
        ap.Cantidad = stock.Cantidad;
        ap.Notas = stock.Motivo;
        ap.ProductoId = stock.ProductoId;
        ap.UserUpdatedId = userId;
        return await _ventasProductos.Save(ap);
    }

    public async Task<DevolucionModel> CargarDevolucion(Guid ventaId)
    {
        var productos = new List<DevolucionProductoModel>();
        var venta = await _ventas.GetOne(ventaId);
        var ajustesProductos = await _ventasProductos.GetForAjuste(ventaId);
        var data = new DevolucionModel(venta);

        foreach (var it in ajustesProductos)
        {
            var producto = await _productos.GetOne(it.ProductoId);
            var p = new DevolucionProductoModel(producto);
            var catId = (await _categoriasProds.GetForProduct(producto.Id)).First().CategoriaId;

            p.AjusteProductoId = it.Id;
            p.Cantidad = it.Cantidad;
            p.PrecioUnitario = it.PrecioUnitario;

            p.Categoria = (await _categorias.GetOne(catId)).Nombre;
            p.UnidadMedida = (await _uMedida.GetOne(producto.UnidadMedidaId)).Nombre;

            productos.Add(p);
        }

        data.Devueltos = productos.ToArray();

        return data;
    }

    public async Task<VentasDiaResponseModel> Load(DateTime fecha)
    {
        var lista = new List<VentasPorDiaRowModel>();
        var ventas = await _ventas.VentasPorFecha(fecha);
        foreach (var v in ventas)
        {
            var ajustesProductos = await _ventasProductos.GetForAjuste(v.Id);
            var productos = new List<ProductoCantidadModel>();
            foreach (var it in ajustesProductos)
            {
                var producto = await _productos.GetOne(it.ProductoId);
                var p = new ProductoCantidadModel(producto, it.Cantidad, it.PrecioUnitario);
                productos.Add(p);
            }
            lista.Add(new VentasPorDiaRowModel(v, productos.ToArray()));
        }

        return new VentasDiaResponseModel(fecha, lista.ToArray());
    }
}
