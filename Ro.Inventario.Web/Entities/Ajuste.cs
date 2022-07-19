using Ro.Inventario.Web.Entities;

public class Ajuste: IUserUpdated
{
    public Ajuste()
    {
        Id = Guid.NewGuid();
        FechaAjuste = DateTime.Now;
        UserUpdatedId = Guid.Empty;
    }
    public Guid Id { get; set; }
    public Guid UserUpdatedId { get; set; }
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
            Iva = a.Iva,
            UserUpdatedId = a.UserUpdatedId
        };
    }
}