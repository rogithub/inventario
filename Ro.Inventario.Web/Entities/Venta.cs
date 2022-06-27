namespace Ro.Inventario.Web.Entities;

public class Venta
{
    public Venta()
    {
        Id = Guid.NewGuid();
        FechaVenta = DateTime.Now;
    }
    public Guid Id { get; set; }
    public DateTime FechaVenta { get; set; }
    public decimal Pago { get; set; }
    public decimal Cambio { get; set; }
}