using Ro.Inventario.Web.Models;
namespace Ro.Inventario.Web.Entities;

public class Login
{
    public Login()
    {
	this.Email = string.Empty;
	this.Password = string.Empty;
    }
    public Login(LoginModel model)
    {
	this.Email = model.Email;
	this.Password = model.Password;
    }
    public string Email { get; set; }
    public string Password { get; set; }
}
