using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Entities;
using Ro.Inventario.Web.Repos;

namespace Ro.Inventario.Web.Controllers;

public class BusquedasController : Controller
{
    private readonly IBusquedaProductosRepo _productos;
    private readonly ILogger<BusquedasController> _logger;

    public BusquedasController(
        ILogger<BusquedasController> logger,
        IBusquedaProductosRepo productos        
        )
    {
        _logger = logger;
        _productos = productos;
    }
   
    public async Task<IActionResult> Productos(string pattern)
    {
        var prods = await _productos.FulSearchText(pattern);
        return Json(prods.ToArray());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
