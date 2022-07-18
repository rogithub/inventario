using Ro.Inventario.Web.Models;
namespace Ro.Inventario.Web.Entities;

public class Register
{
    public Register()
    {
        this.Email = string.Empty;
        this.Id = Guid.NewGuid();	
        this.Password = string.Empty;
    }
    public Register(RegisterModel model)
    {
        this.Password = model.Password;
        this.Email = model.Email;
        this.Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}

