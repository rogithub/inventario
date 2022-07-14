using System;
namespace Ro.Inventario.Web.Entities;

public class DevolucionProducto
{
    public DevolucionProducto()
    {
        Id = Guid.NewGuid();
        AjusteProductoId = Guid.NewGuid();
        FechaCreado = DateTime.Now;
    }
    public Guid Id { get; set; }
    public Guid AjusteProductoId { get; set; }
    public decimal CantidadEnBuenasCondiciones { get; set; }
    public decimal CantidadEnMalasCondiciones { get; set; }
    public DateTime FechaCreado { get; set; }    
}