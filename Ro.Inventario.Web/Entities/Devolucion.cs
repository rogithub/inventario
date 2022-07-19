namespace Ro.Inventario.Web.Entities;

public class Devolucion: IUserUpdated
{
    public Devolucion()
    {
        Id = Guid.NewGuid();
        AjusteId = Guid.NewGuid();
        ProductoId = Guid.NewGuid();
        Fecha = DateTime.Now;
    }
    public Guid Id { get; set; }
    public Guid UserUpdatedId { get; set; }
    public Guid AjusteId { get; set; }
    public Guid ProductoId { get; set; }
    public decimal Cantidad { get; set; }
    public DateTime Fecha { get; set; }
    public string Motivo { get; set; }
}