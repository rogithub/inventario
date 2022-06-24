namespace Ro.Inventario.Web.Entities;

public class UnidadMedida
{    
    public UnidadMedida()
    {
        Id = Guid.NewGuid();
        Nombre = string.Empty;
    }
    public Guid Id { get; set; }
    public string Nombre { get; set; }
}
