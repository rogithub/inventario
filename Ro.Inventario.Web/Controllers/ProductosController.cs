using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ro.Inventario.Web.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Ro.Inventario.Web.Controllers;

public class ProductosController : Controller
{
    private readonly ILogger<ProductosController> _logger;

    public ProductosController(ILogger<ProductosController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Nuevo()
    {
        return View(new CompraNuevosProductos());
    }

    public IActionResult Guardar(CompraNuevosProductos model, List<IFormFile> files)
    {
        _logger.Lof        
        return View(new CompraNuevosProductos());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
