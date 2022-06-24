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

    

    public IActionResult Guardar([FromForm]CompraNuevosProductos model)
    {
        _logger.LogInformation("model {model.Id}", model.Id);
        _logger.LogInformation("model {model.FechaCreado}", model.FechaCreado);
        _logger.LogInformation("model {model.FechaFactura}", model.FechaFactura);
        _logger.LogInformation("model {model.Notas}", model.Notas);
        _logger.LogInformation("model {model.CostoPaqueteria}", model.CostoPaqueteria);
        _logger.LogInformation("model {model.TotalFactura}", model.TotalFactura);
        var lines = model.Archivo.ReadLines();
        //if (!lines.ValidateProducts()) return;
        var products = lines.ParseProducts();        
                
        return View("Nuevo", model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
