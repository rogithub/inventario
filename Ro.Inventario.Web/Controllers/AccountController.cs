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

public class AccountController : BaseController
{
    private readonly ILogger<AccountController> _logger;
    private readonly IUsersRepo _usersRepo;
	private readonly IRolesRepo _rolesRepo;
    private readonly IConfiguration _configuration;
    
    public AccountController(ILogger<AccountController> logger,
			  IUsersRepo usersRepo,
			  IRolesRepo rolesRepo,
			  IConfiguration configuration)
    {
		_logger = logger;
		_usersRepo = usersRepo;
		_rolesRepo = rolesRepo;
		_configuration = configuration;
    }

	[HttpGet]
    public async Task<IActionResult> Logout(string returnUrl)
    {	
		await HttpContext.SignOutAsync();

		return Redirect("/");
    }

	[HttpGet]
    public IActionResult Denied()
    {	
		return View();
    }

	[HttpGet]
    public IActionResult ResetPassword(Guid id)
    {	
		return View();
    }
	    
	[HttpPost]
    public async Task<IActionResult> Register(RegisterModel m)
    {		
		if (ModelState.IsValid == false)
		{
			return View(m);
		}

		var roles = await _rolesRepo.GetRoles();		
		if (roles.ContainsKey(m.Role) == false)
		{
			ModelState.AddModelError("Role", "Role no encontrado");
			return View(m);
		}

		var roleId = roles[m.Role];
		
		try 
		{
			var user = new Register(m);
			var rows = await _usersRepo.Create(user);
			if (rows == 1)
			{
				var id = Guid.NewGuid();
				await _rolesRepo.AddToRole(id, user.Id, roleId);
			}
		}
		catch (Exception ex) {
			_logger.LogError(1, ex, "Register Error");
			return View(m);
		}

		var loginUrl = Url.Action("Login", "Account");
		var indexUrl = Url.Action("Index", "Home");
		var url = string.Format("{0}?returnUrl={1}",
			loginUrl, indexUrl);

		return Redirect(url);
    }

	[HttpGet]
    public IActionResult Register()
    {		
		return View(new RegisterModel());
    }

	[HttpGet]
    public IActionResult Registro()
    {		
		return View(new RegisterModel());
    }

	[HttpGet]
	public IActionResult Login(string returnUrl)
    {	
		ViewData["ReturnUrl"] = returnUrl;

		return View();
    }


	[HttpPost]
    public async Task<IActionResult> Login(
		string email, 
		string password, 
		string returnUrl)
    {	
		try {
			var entity = new Login();
			entity.Email = email;
			entity.Password = password;

			returnUrl = System.Net.WebUtility.UrlDecode(returnUrl);
			_logger.LogInformation("Return url {url}", returnUrl);			

			if (!await _usersRepo.HasAccess(entity))
			{
				_logger.LogWarning("Intento fallido de login {user}", entity.Email);
				ViewData["ReturnUrl"] = returnUrl;
				TempData["Error"] = "Error. Su email o password no es correcto.";
				return View("Login");
			}
			
			var user = await _usersRepo.GetOne(email);
			await SetupClaims(user);		
			return Redirect(returnUrl);	
		}
		catch (Exception ex) {
			_logger.LogError(1, ex, "Login Error");
			throw;
		}					
    }

	[NonAction] 
    private async Task SetupClaims(User user)
    {		
		var roles = await _rolesRepo.GetRoles(user.Id);
		_logger.LogInformation(0, "--> Claim {email}", ClaimTypes.Email);
		_logger.LogInformation(0, "--> User {email}", user.Email);
		List<Claim> claims = new List<Claim>
		{
			new Claim(ClaimTypes.Email, user.Email),			
			new Claim("Id", user.Id.ToString())			
		};
		foreach(var r in roles)
		{
			_logger.LogInformation(0, "--> Roles {email} {rol}", user.Email, r);
			claims.Add(new Claim(ClaimTypes.Role, r));
		}
		var identity = new ClaimsIdentity(claims, 
		CookieAuthenticationDefaults.AuthenticationScheme);

		var principal = new ClaimsPrincipal(identity);		
		await HttpContext.SignInAsync(principal);
		
    }	
}
