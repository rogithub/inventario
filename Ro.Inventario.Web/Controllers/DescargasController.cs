using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ro.Inventario.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace Ro.Inventario.Web.Controllers;

[Authorize]
public class DescargasController : BaseController
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
}
