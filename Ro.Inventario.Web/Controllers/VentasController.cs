using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Entities;
using Ro.Inventario.Web.Repos;
using Ro.Inventario.Web.Services;

namespace Ro.Inventario.Web.Controllers;

public class VentasController : Controller
{
    private readonly IVentasProductosRepo _ventasProds;
    private readonly IVentasRepo _ventas;
    private readonly ILogger<VentasController> _logger;

    private readonly IVentasService _ventasService;

    public VentasController(
        ILogger<VentasController> logger,
        IVentasProductosRepo ventasProds,
        IVentasRepo ventas,
        IVentasService ventasService
        )
    {
        _logger = logger;
        _ventasProds = ventasProds;
        _ventas = ventas;
        _ventasService = ventasService;
    }

    public async Task<IActionResult> Index(DateTime? fecha)
    {
        var date = fecha.HasValue ? fecha.Value : DateTime.Now;
        var model = await _ventasService.Load(date);
        return View(model);
    }

    public IActionResult Nuevo()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Guardar([FromBody] VentaModel model)
    {
        var v = new Venta();
        v.Pago = model.Pago;
        v.Cambio = model.Cambio;
        v.FechaVenta = model.Fecha;
        var intVenta = await _ventas.Save(v);
        var prods = (from l in model.Items
                     select new VentaProducto()
                     {
                         ProductoId = l.ProductoId,
                         VentaId = v.Id,
                         Cantidad = l.Cantidad
                     }).ToArray();

        _logger.LogInformation("Venta id {id}", v.Id.ToString());
        _logger.LogInformation("Venta pago {pago}", model.Pago);
        _logger.LogInformation("Venta cambio {cambio}", model.Cambio);
        _logger.LogInformation("Items {len}", model.Items.Length);

        var intVentaProducts = await _ventasProds.BulkSave(prods);

        return Json(new int[] { intVenta, intVentaProducts });
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
