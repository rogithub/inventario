using System.ComponentModel.DataAnnotations;
using Ro.Inventario.Web.Entities;
using Ro.Inventario.Web.Repos;

namespace Ro.Inventario.Web.Models;

public class ResetPasswordListing
{
    public ResetPasswordListing(
        User user,
        IEnumerable<ChangePassword> requests
    ) 
    {
        this.User = user;
        this.Requests = requests;
    }
    public User User { get; set; }
    public IEnumerable<ChangePassword> Requests { get; set; }
}

public class ChangePwdModel
{
    public ChangePwdModel() : this(Guid.Empty)
    {        
    }
    public ChangePwdModel(Guid requestId)
    {
        this.Email = string.Empty;
        this.RequestId = requestId;
        this.Password = string.Empty;
        this.PasswordConfirm = string.Empty;
    }
    public Guid RequestId { get; set; }

    [Required(ErrorMessage = "Email requerido")]
    [EmailAddress(ErrorMessage = "Correo no válido")]
    public string Email { get; set; }    
    [Required(ErrorMessage = "Password requerido")]
    [StringLength(255, ErrorMessage = "Contraseña debe tener entre 5 y 255 caractéres", MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required(ErrorMessage = "Se requirere confirmación de password")]
    [StringLength(255, ErrorMessage = "Contraseña debe tener entre 5 y 255 caractéres", MinimumLength = 5)]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string PasswordConfirm { get; set; }    
}

