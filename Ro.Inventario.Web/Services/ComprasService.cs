using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Repos;
using Ro.Inventario.Web.Entities;
using System.Linq;
namespace Ro.Inventario.Web.Services;

public interface  IComprasService
{
    Task ProcessModel(CompraNuevosProductos model);
}

public class ComprasService: IComprasService
{    
    private IUnidadMedidaRepo _uMedida;
    private IProductosRepo _producto;
    private IComprasRepo _compras;
    private readonly ILogger<ComprasService> _logger;
    public ComprasService(
        ILogger<ComprasService> logger,
        IUnidadMedidaRepo unidadMedida,
        IProductosRepo productRepo,
        IComprasRepo comprasRepo)
    {
        _logger = logger;
       _uMedida = unidadMedida;       
       _producto = productRepo;
       _compras = comprasRepo;
    }    

    public async Task ProcessModel(CompraNuevosProductos model)
    {
        var lines = model.Archivo.ReadLines();        
        var productLines = lines.ParseProducts();

        var compra = new Compra();
        compra.Notas = model.Notas;
        compra.FechaFactura = model.FechaFactura;
        compra.CostoPaqueteria = model.CostoPaqueteria;
        compra.TotalFactura = model.TotalFactura;
        await _compras.Save(compra);
        _logger.LogInformation("Generando compra {id}", compra.Id);
        
        var unidadesMedida = (from p in productLines select p.UnidadDeMedida);
        var uMedidaCounter = await _uMedida.BulkSave(unidadesMedida.ToArray());
        _logger.LogInformation("{uMedidaCounter} Unidades de medida guardadas", uMedidaCounter);        
        var dUnidadMedida = await _uMedida.GetDictionary();

        var productos = (from p in productLines select p.ToEntity(dUnidadMedida));
        var prodCounter = await _producto.BulkSave(productos.ToArray());
        _logger.LogInformation("{prodCounter} Produtos nuevos guardados", prodCounter);
        
    }
}
