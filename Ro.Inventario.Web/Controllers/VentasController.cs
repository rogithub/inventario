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
    private readonly IAjustesProductosRepo _ventasProds;
    private readonly IAjustesRepo _ventas;
    private readonly ISettingsRepo _settings;
    private readonly ILogger<VentasController> _logger;

    private readonly IVentasService _ventasService;

    public VentasController(
        ILogger<VentasController> logger,
        IAjustesProductosRepo ventasProds,
        IAjustesRepo ventas,
        ISettingsRepo settings,
        IVentasService ventasService
        )
    {
        _logger = logger;
        _ventasProds = ventasProds;
        _ventas = ventas;
        _ventasService = ventasService;
        _settings = settings;
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

    public IActionResult Devoluciones(Guid ventaId)
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
        v.Iva = await _settings.GetValue("IVA", (iva) => decimal.Parse(iva));
        var intVenta = await _ventas.Save(v);
        var prods = (from l in model.Items
                     select new VentaProducto()
                     {
                         ProductoId = l.ProductoId,
                         VentaId = v.Id,
                         Cantidad = l.Cantidad,
                         PrecioUnitario = l.PrecioUnitario
                     }).ToArray();

        _logger.LogInformation("Venta id {id}", v.Id.ToString());
        _logger.LogInformation("Venta pago {pago}", model.Pago);
        _logger.LogInformation("Venta cambio {cambio}", model.Cambio);
        _logger.LogInformation("Items {len}", model.Items.Length);

        var intVentaProducts = await _ventasProds.BulkSave(prods.Cast<AjusteProducto>().ToArray());

        return Json(new int[] { intVenta, intVentaProducts });
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
