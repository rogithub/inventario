using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Services;

namespace Ro.Inventario.Web.Controllers;

public class ProductosController : Controller
{
    private readonly ILogger<ProductosController> _logger;
    private readonly IComprasService _comprasSvc;
    private readonly IProductosService _productosSvc;
    private readonly INuevosProductosValidatorService _pValidator;

    public ProductosController(
        ILogger<ProductosController> logger,
        IComprasService comprasService,
        IProductosService productosService,
        INuevosProductosValidatorService pValidator)
    {
        _logger = logger;
        _comprasSvc = comprasService;
        _productosSvc = productosService;
        _pValidator = pValidator;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Editar(Guid id)
    {
        var m = await _productosSvc.LoadModel(id);
        return View(m);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(ProductoDotnetModel m)
    {
        if (ModelState.IsValid == false)
        {
            return View(m);
        }
        await _productosSvc.Actualizar(m);
        return RedirectToAction("Index", "Productos");
    }

    public IActionResult Nuevo()
    {
        return View(new CompraNuevosProductos());
    }

    [HttpPost]
    public async Task<IActionResult> Nuevo([FromForm] CompraNuevosProductos model)
    {
        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        var lines = model.Archivo?.ReadLines();
        var areLinesValid = _pValidator.ValidateProducts(lines);
        if (!areLinesValid)
        {
            ModelState.AddModelError("Archivo",
            "El archivo tiene datos incorrectos, quite comas y verifique no tener columnas extra.");
            return View(model);
        }

        await _comprasSvc.ProcessModel(model, lines);

        return RedirectToAction("Index", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
