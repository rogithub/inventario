using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;

using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Entities;
using Ro.Inventario.Web.Repos;


namespace Ro.Inventario.Web.Controllers;

public class BaseController : Controller
{    
    
    public BaseController()
    {
		
    }

    [NonAction]
    protected Guid GetUserId()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity == null) 
        {            
            return Guid.Empty;
        }

        var claim = identity.FindFirst("Id");
        if (claim == null) 
        {            
            return Guid.Empty;
        }
        
        return Guid.Parse(claim.Value); 
    }

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }   
}
