namespace Ro.Inventario.Web.Entities;

public class Compra
{    
    public Compra()
    {
        Id = Guid.NewGuid();
        Notas = string.Empty;
        FechaCreado = DateTime.Now;
        FechaFactura = DateTime.Now;
        
    }
    public Guid Id { get; set; }
    public string Notas { get; set; }
    public DateTime FechaFactura { get; set; }
    public DateTime FechaCreado { get; set; }
    public decimal CostoPaqueteria { get; set; }
    public decimal TotalFactura { get; set; }
    public decimal PorcentajeFacturaIVA { get; set; }
}