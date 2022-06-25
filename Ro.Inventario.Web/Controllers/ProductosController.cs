using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Services;
using Microsoft.AspNetCore.Hosting;

namespace Ro.Inventario.Web.Controllers;

public class ProductosController : Controller
{
    private readonly ILogger<ProductosController> _logger;
    private readonly IComprasService _comprasSvc;

    public ProductosController(
        ILogger<ProductosController> logger,
        IComprasService comprasService)
    {
        _logger = logger;
        _comprasSvc = comprasService;
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
        if (ModelState.IsValid == false)
        {
          return View("Nuevo", model);
        }
        
        var lines = model.Archivo?.ReadLines();
        var areLinesValid = lines?.ValidateProducts();
        if (!areLinesValid.GetValueOrDefault())
        {
            ModelState.AddModelError("Archivo", 
            "El archivo tiene datos incorrectos, quite comas y verifique no tener columnas extra.");
            return View("Nuevo", model);
        }
        
        return Index();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
