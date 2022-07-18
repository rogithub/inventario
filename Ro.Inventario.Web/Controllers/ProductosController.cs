using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Services;
using Ro.Inventario.Web.Repos;
using Newtonsoft.Json;

namespace Ro.Inventario.Web.Controllers;

public class ProductosController : BaseController
{
    private readonly ILogger<ProductosController> _logger;
    private readonly IComprasService _comprasSvc;
    private readonly IVentasService _ventasSvc;
    private readonly IProductosService _productosSvc;
    private readonly INuevosProductosValidatorService _pValidator;
    private readonly IBusquedaProductosRepo _pBuscar;


    public ProductosController(
        ILogger<ProductosController> logger,
        IComprasService comprasService,
        IVentasService ventasSvc,
        IProductosService productosService,
        IBusquedaProductosRepo pBuscar,
        INuevosProductosValidatorService pValidator)
    {
        _logger = logger;
        _pBuscar = pBuscar;
        _comprasSvc = comprasService;
        _ventasSvc = ventasSvc;
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

    public IActionResult Etiquetas()
    {
        return View();
    }

    public async Task<IActionResult> Stock(Guid id)
    {
        var p = await _pBuscar.GetOne(id);
        return View(p);
    }

    [HttpPost]
    public async Task<IActionResult> Stock([FromBody] StockAjusteModel model)
    {
        var val = await _ventasSvc.GuardarAjusteStock(model);
        return Json(new { updated = val });
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

    
}
