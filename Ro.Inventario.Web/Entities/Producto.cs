namespace Ro.Inventario.Web.Entities;

public class Producto: IUserUpdated
{
    public Producto()
    {
        Id = Guid.NewGuid();
        UnidadMedidaId = Guid.NewGuid();
        UserUpdatedId = Guid.Empty;
        Nombre = string.Empty;
        CodigoBarrasItem = string.Empty;
        CodigoBarrasCaja = string.Empty;        
    }
    public Guid Id { get; set; }
    public Guid UserUpdatedId { get; set; }

    public Guid UnidadMedidaId { get; set; }
    public string Nombre { get; set; }        
    public string CodigoBarrasItem { get; set; }
    public string CodigoBarrasCaja { get; set; }  
}