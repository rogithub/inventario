using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Repos;
using Ro.Inventario.Web.Entities;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ro.Inventario.Web.Services;

public interface IProductosService
{
    Task<ProductoDotnetModel> LoadModel(Guid productoId);
    Task Actualizar(ProductoDotnetModel model);
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
        var categoriaId = categorias.First().CategoriaId;
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

        p.UnidadesMedida.AddRange(from u in await _uMedida.GetAll() select u);
        p.Categorias.AddRange(from c in await _categorias.GetAll() select c);

        return p;
    }

    public async Task Actualizar(ProductoDotnetModel m)
    {
        _logger.LogInformation("Guardando producto {id}", m.Id);
        _logger.LogInformation("Nombre {Nombre}", m.Nombre);
        _logger.LogInformation("CategoriaID {CategoriaID}", m.CategoriaId);
        _logger.LogInformation("CodigoBarrasItem {CodigoBarrasItem}", m.CodigoBarrasItem);
        _logger.LogInformation("CodigoBarrasCaja {CodigoBarrasCaja}", m.CodigoBarrasCaja);
        _logger.LogInformation("PrecioVenta {PrecioVenta}", m.PrecioVenta);
        _logger.LogInformation("UnidadMedidaId {UnidadMedidaId}", m.UnidadMedidaId);

        var i = await _producto.GetOne(m.Id);
        if (m.Nombre != i.Nombre ||
            m.CodigoBarrasCaja != i.CodigoBarrasCaja ||
            m.CodigoBarrasItem != i.CodigoBarrasItem ||
            m.UnidadMedidaId != i.UnidadMedidaId)
        {
            i.Nombre = m.Nombre;
            i.UnidadMedidaId = m.UnidadMedidaId;
            i.CodigoBarrasItem = m.CodigoBarrasItem;
            i.CodigoBarrasCaja = m.CodigoBarrasCaja;
            await _producto.Update(i);
        }

        var cat = (await _categoriasProds.GetForProduct(m.Id)).First();
        if (m.CategoriaId != cat.CategoriaId)
        {
            cat.CategoriaId = m.CategoriaId;
            await _categoriasProds.Update(cat);
        }

        var precio = await _precios.GetOneForProduct(m.Id);
        if (precio.PrecioVenta != m.PrecioVenta)
        {
            var precios = new PrecioProducto[] { new PrecioProducto {
                FechaCreado = DateTime.Now,
                Id = Guid.NewGuid(),
                PrecioVenta = m.PrecioVenta,
                ProductoId = m.Id
            } };
            await _precios.BulkSave(precios);
        }
    }
}
