using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Repos;
using Ro.Inventario.Web.Entities;
using System.Linq;
namespace Ro.Inventario.Web.Services;

public interface IProductosService
{
    Task<ProductoModel> LoadModel(Guid productoId);
}

public class ProductosService : IProductosService
{
    private IUnidadMedidaRepo _uMedida;
    private IProductosRepo _producto;
    private ICategoriasRepo _categorias;
    private ICategoriasProductosRepo _categoriasProds;
    private IPreciosProductosRepo _precios;
    private readonly ILogger<ProductosService> _logger;
    public ProductosService(
        ILogger<ProductosService> logger,
        IUnidadMedidaRepo unidadMedida,
        IProductosRepo productRepo,
        ICategoriasRepo categoriasRepo,
        ICategoriasProductosRepo categoriasProductosRepo,
        IPreciosProductosRepo preciosRepo)
    {
        _logger = logger;
        _uMedida = unidadMedida;
        _producto = productRepo;
        _categorias = categoriasRepo;
        _categoriasProds = categoriasProductosRepo;
        _precios = preciosRepo;
    }

    public async Task<ProductoModel> LoadModel(Guid productoId)
    {
        var p = new ProductoModel();
        var entity = await _producto.GetOne(productoId);
        var uMedida = await _uMedida.GetOne(entity.UnidadMedidaId);
        var categorias = await _categoriasProds.GetForProduct(entity.Id);
        var precio = await _precios.GetOneForProduct(entity.Id);


        return p;
    }
}
