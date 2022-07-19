using System.ComponentModel.DataAnnotations;
namespace Ro.Inventario.Web.Models;

public class RegisterEmail
{
    public RegisterEmail()
    {
        this.Email = string.Empty; 
        this.Role = string.Empty;        
    }
    [Required(ErrorMessage = "Email requerido")]
    [EmailAddress(ErrorMessage = "Correo no válido")]
    public string Email { get; set; }   
    [Required(ErrorMessage = "Role requerido")]    
    public string Role { get; set; }   
}

public class RegisterModel
{
    public RegisterModel()
    {
        this.Email = string.Empty;
        this.Role = string.Empty;
        this.Password = string.Empty;
        this.PasswordConfirm = string.Empty;
    }
    [Required(ErrorMessage = "Email requerido")]
    [EmailAddress(ErrorMessage = "Correo no válido")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Role requerido")]
    [StringLength(255, ErrorMessage = "Role debe tener entre 4 y 255 caractéres", MinimumLength = 4)]
    public string Role { get; set; }
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
