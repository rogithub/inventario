using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Repos;
namespace Ro.Inventario.Web.Services;

public interface IVentasService
{
    Task<VentasDiaResponseModel> Load(DateTime fecha);
}

public class VentasService : IVentasService
{
    private IProductosRepo _productos;
    private IAjustesRepo _ventas;
    private IAjustesProductosRepo _ventasProductos;
    private readonly ILogger<VentasService> _logger;
    public VentasService(
        ILogger<VentasService> logger,
        IProductosRepo productRepo,
        IAjustesProductosRepo ventasProductos,
        IAjustesRepo ventas)
    {
        _logger = logger;
        _ventas = ventas;
        _productos = productRepo;
        _ventasProductos = ventasProductos;
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
                var p = new ProductoCantidadModel(producto, it.Cantidad);
                productos.Add(p);
            }
            lista.Add(new VentasPorDiaRowModel(v, productos.ToArray()));
        }

        return new VentasDiaResponseModel(fecha, lista.ToArray());
    }
}
