using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Ro.Inventario.Web.Models;
using Ro.Inventario.Web.Entities;
using Ro.Inventario.Web.Repos;


namespace Ro.Inventario.Web.Controllers;

public class UsersController : BaseController
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUsersRepo _usersRepo;    
    private readonly IChangePasswordRepo _chgPwdRepo;
    private readonly IConfiguration _configuration;
    private readonly IRolesRepo _rolesRepo;
    
    public UsersController(ILogger<UsersController> logger,
	    IUsersRepo usersRepo,
        IChangePasswordRepo chgPwdRepo,
        IRolesRepo rolesRepo,
			  IConfiguration configuration)
    {
      _logger = logger;
      _usersRepo = usersRepo;
      _chgPwdRepo = chgPwdRepo;
      _rolesRepo = rolesRepo;
      _configuration = configuration;
    }


    [HttpPost]
    // No se necesita Authorization por que esto es público.
    public async Task<IActionResult> ChangePwd(ChangePwdModel m)
    {		
      var result = await _chgPwdRepo.ChangePassword(m.RequestId, m.Password);

      switch (result)
      {
        case ChangePasswordResult.Success:
          var loginUrl = Url.Action("Login", "Account");
          var indexUrl = Url.Action("Index", "Home");
          var url = string.Format("{0}?returnUrl={1}",
          loginUrl, indexUrl);
          return Redirect(url);
        case ChangePasswordResult.NotFound:
          ModelState.AddModelError("Request", "No se encontró solicitud de cambio de contraseña para su usuario.");
        break;
        case ChangePasswordResult.Expired:
          ModelState.AddModelError("Expirado", "Solicitud de cambio de contraseña expiró, solicitar nuevamente al administrador.");
        break;
        default:
          ModelState.AddModelError("Error", "Se encontró un error, avisar al administrador.");
        break;
      }    
      
		  return View(m);
    }
    
    [HttpGet]
    // No se necesita Authorization por que esto es público.
    public async Task<IActionResult> ChangePwd(Guid id)
    {		
      var model = new ChangePwdModel(id);
      var req = await _chgPwdRepo.GetOne(id);
      var user = await _usersRepo.GetOne(req.UserId);
      model.Email = user.Email;
		  return View(model);
    }

    [HttpGet]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> Index()
    {		
      var users = await _usersRepo.GetAll();
		  return View(users);
    }

    [HttpGet]
    [Authorize(Roles="Admin")]
    public IActionResult AddUser()
    {
      return View(new RegisterEmail());
    }

    [HttpPost]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> AddUser(RegisterEmail m)
    {		
      if (ModelState.IsValid == false)
      {
        return View(m);
      }

      var roleExists = await _rolesRepo.RoleExists(m.Role);
      if (!roleExists)
      {
        ModelState.AddModelError("Role", "Role no existe en la base de datos");
        return View(m);
      }      
      
      try {
        var e = new Register();
        e.Email = m.Email;
        e.Password = e.Id.ToString();
        var rows = await _usersRepo.Create(e);
        var count = await _rolesRepo.AddToRole(Guid.NewGuid(), e.Id, m.Role);
        if (count != 1)
        {
          throw new Exception("Role User not found.");
        }
      }
      catch (Exception ex) {
        _logger.LogError(1, ex, "Register Error");
        throw;
      }
      
      return Redirect("Index");
    }

    [HttpPost]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> ToggleActive(GuidModel m)
    {
      _logger.LogInformation(1, "--> Toggling active");       

       if (m.Id == Guid.Empty)
       {
         _logger.LogInformation(1, "--> Toggling active Guid empty");
         return Redirect("Index");
       }

      try {
        await _usersRepo.ToggleActive(m.Id);        
      }
      catch (Exception ex) {
        _logger.LogError(1, ex, "Toggle Active Error");
        throw;
      }
      
      return Redirect("Index");
    }

    [HttpPost]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> ResetPassword(GuidEmailModel m)
    {
      var redirect = string.Format("ResetPassword?email={0}", m.Email);

       if (m.Id == Guid.Empty)
       {
         _logger.LogInformation(1, "--> Reset pwd Guid empty");
         return Redirect(redirect);
       }

      try {
        var id = Guid.NewGuid();
        await _chgPwdRepo.Create(id, m.Id, DateTime.Now.AddDays(1));        
      }
      catch (Exception ex) {
        _logger.LogError(1, ex, "Toggle Active Error");
        throw;
      }
      
      return Redirect(redirect);
    }

    [HttpPost]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> DeleteResetPwd(GuidEmailModel m)
    {
      var redirect = string.Format("ResetPassword?email={0}", m.Email);     

       if (m.Id == Guid.Empty)
       {
         _logger.LogInformation(1, "--> Reset pwd Guid empty");
         return Redirect(redirect);
       }

      try {
        await _chgPwdRepo.Delete(m.Id);        
      }
      catch (Exception ex) {
        _logger.LogError(1, ex, "Delete reset pwd Error");
        throw;
      }
      
      return Redirect(redirect);
    }

    [HttpGet]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> ResetPassword(string email)
    {
      try {
        ViewData["email"] = email;
        var user = await _usersRepo.GetOne(email);        
        var list = await _chgPwdRepo.GetAll(user.Id);

        var m = new ResetPasswordListing(user, list);
        return View(m); 
      }
      catch (Exception ex) {
        _logger.LogError(1, ex, "ResetPassword Error");
        throw;
      }            
    }    

    
}
