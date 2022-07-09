using System;
namespace Ro.Inventario.Web.Entities;

public class VentaProducto
{
    public VentaProducto()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public Guid ProductoId { get; set; }
    public Guid VentaId { get; set; }
    public decimal Cantidad { get; set; }

    public static implicit operator AjusteProducto(VentaProducto a)
    {
        return new AjusteProducto()
        {
            Id = a.Id,
            ProductoId = a.ProductoId,
            AjusteId = a.VentaId,
            Cantidad = a.Cantidad,
            Notas = string.Empty
        };
    }
}