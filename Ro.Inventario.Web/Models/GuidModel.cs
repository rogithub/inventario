namespace Ro.Inventario.Web.Models;

public class GuidModel
{
    public GuidModel() => this.Id = Guid.Empty;
    public Guid Id { get; set; }
}


public class GuidEmailModel
{
    public GuidEmailModel()
    {
        this.Id = Guid.Empty;
        this.Email = string.Empty;
    }
    public Guid Id { get; set; }
    public string Email { get; set; }
}