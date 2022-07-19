using System;
namespace Ro.Inventario.Web.Entities;

public class DevolucionProducto: IUserUpdated
{
    public DevolucionProducto()
    {
        Id = Guid.NewGuid();
        AjusteProductoId = Guid.NewGuid();
        FechaCreado = DateTime.Now;
        UserUpdatedId = Guid.Empty;
    }
    public Guid Id { get; set; }
    public Guid AjusteProductoId { get; set; }
    public Guid UserUpdatedId { get; set; }
    public decimal CantidadEnBuenasCondiciones { get; set; }
    public decimal CantidadEnMalasCondiciones { get; set; }
    public DateTime FechaCreado { get; set; }    
}