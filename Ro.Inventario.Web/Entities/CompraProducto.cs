namespace Ro.Inventario.Web.Entities;

public class CompraProducto
{
    public CompraProducto()
    {
        Id = Guid.NewGuid();
        ProductoId = Guid.NewGuid();
        CompraId = Guid.NewGuid();
        
    }
    public Guid Id { get; set; }
    public Guid ProductoId { get; set; }
    public Guid CompraId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioCompra { get; set; }
}