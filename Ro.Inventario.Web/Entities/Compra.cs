namespace Ro.Inventario.Web.Entities;

public class Compra: IUserUpdated
{    
    public Compra()
    {
        Id = Guid.NewGuid();
        UserUpdatedId = Guid.Empty;
        Proveedor = string.Empty;
        FechaCreado = DateTime.Now;
        FechaFactura = DateTime.Now;
        
    }
    public Guid Id { get; set; }
    public Guid UserUpdatedId { get; set; }
    public string Proveedor { get; set; }
    public DateTime FechaFactura { get; set; }
    public DateTime FechaCreado { get; set; }
    public decimal CostoPaqueteria { get; set; }
    public decimal TotalFactura { get; set; }
    public decimal PorcentajeFacturaIVA { get; set; }
}
