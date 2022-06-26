using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ro.Inventario.Web.Models;

namespace Ro.Inventario.Web.Controllers;

public class DescargasController : Controller
{
    private readonly ILogger<DescargasController> _logger;
    private readonly IConfiguration _config;

    public DescargasController(
        ILogger<DescargasController> logger,
        IConfiguration configRoot)
    {
        _logger = logger;
        _config = configRoot;
    }

    public IActionResult BaseDeDatos()
    {
        var path = _config.GetSection("DbPath").Value;
        byte[] fileBytes = System.IO.File.ReadAllBytes(path);
        string fileName = "inventario.db";
        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
