using Ro.Inventario.Web.Entities;

public class Ajuste
{
    public Ajuste()
    {
        Id = Guid.NewGuid();
        FechaAjuste = DateTime.Now;
    }
    public Guid Id { get; set; }
    public DateTime FechaAjuste { get; set; }
    public decimal Pago { get; set; }
    public decimal Cambio { get; set; }
    public TipoAjuste TipoAjuste { get; set; }
    public decimal Iva { get; set; }
    public static implicit operator Venta(Ajuste a)
    {
        return new Venta()
        {
            Id = a.Id,
            FechaVenta = a.FechaAjuste,
            Pago = a.Pago,
            Cambio = a.Cambio,
            Iva = a.Iva
        };
    }
}