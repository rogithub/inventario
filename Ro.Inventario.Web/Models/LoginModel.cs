namespace Ro.Inventario.Web.Models;

public class LoginModel
{
    public LoginModel()
    {
        this.Email = string.Empty;
        this.Password = string.Empty;
    }
    public string Email { get; set; }
    public string Password { get; set; }
}
