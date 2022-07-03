using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Repos;
using Ro.Inventario.Web.Entities;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ro.Inventario.Web.Services;

public interface IProductosService
{
    Task<ProductoDotnetModel> LoadModel(Guid productoId);
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

    public async Task<ProductoDotnetModel> LoadModel(Guid productoId)
    {
        var p = new ProductoDotnetModel();
        var entity = await _producto.GetOne(productoId);
        var uMedida = await _uMedida.GetOne(entity.UnidadMedidaId);
        var categorias = await _categoriasProds.GetForProduct(entity.Id);
        var categoriaId = categorias.FirstOrDefault().CategoriaId;
        var categoria = await _categorias.GetOne(categoriaId);
        var precio = await _precios.GetOneForProduct(entity.Id);
        p.Id = entity.Id;
        p.Nombre = entity.Nombre;
        p.Categoria = categoria.Nombre;
        p.CategoriaId = categoria.Id;
        p.CodigoBarrasCaja = entity.CodigoBarrasCaja;
        p.CodigoBarrasItem = entity.CodigoBarrasItem;
        p.PrecioVenta = precio.PrecioVenta;
        p.PrecioVentaId = precio.Id;
        p.UnidadMedida = uMedida.Nombre;
        p.UnidadMedidaId = uMedida.Id;

        p.UnidadesMedida.AddRange(from u in await _uMedida.GetAll()
                                  select new SelectListItem()
                                  {
                                      Value = u.Id.ToString(),
                                      Text = u.Nombre,
                                      Selected = u.Id == p.UnidadMedidaId
                                  });
        p.Categorias.AddRange(from c in await _categorias.GetAll()
                              select new SelectListItem()
                              {
                                  Value = c.Id.ToString(),
                                  Text = c.Nombre,
                                  Selected = c.Id == p.UnidadMedidaId
                              });

        return p;
    }
}
