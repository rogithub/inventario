using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Repos;

namespace Ro.Inventario.Web.Controllers;

public class VentasController : Controller
{
    private readonly IProductosRepo _productos;
    private readonly ILogger<VentasController> _logger;

    public VentasController(
        ILogger<VentasController> logger,
        IProductosRepo productos
        )
    {
        _logger = logger;
        _productos = productos;
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
        var res = (
            from p in prods select 
            new { id = p.Id, value = p.Nombre }
        ).ToArray();

        return Json(res, true);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
