using System;
namespace Ro.Inventario.Web.Entities;

public class AjusteProducto
{
    public AjusteProducto()
    {
        Id = Guid.NewGuid();
        Notas = string.Empty;
    }
    public Guid Id { get; set; }
    public Guid ProductoId { get; set; }
    public Guid AjusteId { get; set; }
    public decimal Cantidad { get; set; }
    public string Notas { get; set; }
    public decimal PrecioUnitario { get; set; }

    public static implicit operator VentaProducto(AjusteProducto a)
    {
        return new VentaProducto()
        {
            Id = a.Id,
            ProductoId = a.ProductoId,
            VentaId = a.AjusteId,
            Cantidad = a.Cantidad,
            PrecioUnitario = a.PrecioUnitario
        };
    }
}