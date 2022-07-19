namespace Ro.Inventario.Web.Entities;

public class PrecioProducto: IUserUpdated
{
    public PrecioProducto()
    {
        Id = Guid.NewGuid();
        UserUpdatedId = Guid.Empty;
        ProductoId = Guid.NewGuid();
        FechaCreado = DateTime.Now;        
    }
    public Guid Id { get; set; }
    public Guid UserUpdatedId { get; set; }
    public Guid ProductoId { get; set; }
    public DateTime FechaCreado { get; set; }    
    public decimal PrecioVenta { get; set; }
}