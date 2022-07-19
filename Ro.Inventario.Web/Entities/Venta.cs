namespace Ro.Inventario.Web.Entities;

public class Venta: IUserUpdated
{
    public Venta()
    {
        Id = Guid.NewGuid();
        FechaVenta = DateTime.Now;
        UserUpdatedId = Guid.Empty;
    }
    public Guid Id { get; set; }
    public Guid UserUpdatedId { get; set; }
    public DateTime FechaVenta { get; set; }
    public decimal Pago { get; set; }
    public decimal Cambio { get; set; }
    public decimal Iva { get; set; }


    public static implicit operator Ajuste(Venta v)
    {
        return new Ajuste()
        {
            Id = v.Id,
            FechaAjuste = v.FechaVenta,
            Pago = v.Pago,
            Cambio = v.Cambio,
            TipoAjuste = TipoAjuste.Venta,
            Iva = v.Iva,
            UserUpdatedId = v.UserUpdatedId
        };
    }
}