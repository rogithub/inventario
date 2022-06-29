using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Entities;
using Ro.Inventario.Web.Repos;

namespace Ro.Inventario.Web.Controllers;

public class VentasController : Controller
{
    private readonly IBusquedaProductosRepo _productos;
    private readonly IVentasProductosRepo _ventasProds;
    private readonly IVentasRepo _ventas;
    private readonly ILogger<VentasController> _logger;

    public VentasController(
        ILogger<VentasController> logger,
        IBusquedaProductosRepo productos,
        IVentasProductosRepo ventasProds,
        IVentasRepo ventas
        )
    {
        _logger = logger;
        _productos = productos;
        _ventasProds = ventasProds;
        _ventas = ventas;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Nuevo()
    {
        return View();
    }
    
    public async Task<IActionResult> BuscarProducto(string pattern)
    {
        var prods = await _productos.FulSearchText(pattern);
        return Json(prods.ToArray());
    }

    [HttpPost]
    public async Task<IActionResult> Guardar(VentaModel model)
    {
        var v = new Venta();
        v.Pago = model.Pago;
        v.Cambio = model.Cambio;
        var intVenta = await _ventas.Save(v);
        var prods = (from l in model.Items select new VentaProducto()
        {
            ProductoId = l.ProductoId,
            VentaId = v.Id,
            Cantidad = l.Cantidad
        }).ToArray();


        _logger.LogInformation("venta pago {pago}",model.Pago); 
        _logger.LogInformation("venta cambio {cambio}",model.Cambio); 
        _logger.LogInformation("items {len}",model.Items.Length); 

        var intVentaProducts = await _ventasProds.BulkSave(prods);

        return Json(new int[] { intVenta, intVentaProducts});
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
