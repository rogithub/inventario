namespace Ro.Inventario.Web.Entities;

public class PrecioProducto
{
    public PrecioProducto()
    {
        Id = Guid.NewGuid();
        ProductoId = Guid.NewGuid();
        FechaCreado = DateTime.Now;        
    }
    public Guid Id { get; set; }
    public Guid ProductoId { get; set; }
    public DateTime FechaCreado { get; set; }    
    public decimal PrecioVenta { get; set; }
}