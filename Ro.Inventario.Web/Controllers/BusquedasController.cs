using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Entities;
using Ro.Inventario.Web.Repos;

namespace Ro.Inventario.Web.Controllers;

public class BusquedasController : BaseController
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

    public async Task<IActionResult> GetByQr(string qr)
    {
        var prods = await _productos.SearchByQr(qr);
        return Json(prods.ToArray());
    }
}
