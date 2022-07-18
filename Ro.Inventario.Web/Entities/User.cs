
using Ro.Inventario.Web.Models;

namespace Ro.Inventario.Web.Entities;

public class User
{
    public User()
    {
        this.Email = string.Empty;
        this.Id = Guid.NewGuid();
        this.IsActive = false;
    }
    public User(UserModel model)
    {
        this.Email = model.Email;
        this.Id = Guid.NewGuid();
        this.IsActive = false;
    }
    public Guid Id { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
}

