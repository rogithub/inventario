using System;
using System.Linq;
using Ro.Inventario.Web.Entities;
using Newtonsoft.Json;
namespace Ro.Inventario.Web.Models;

public class ProductoCantidadModel : Producto
{
    public ProductoCantidadModel(Producto p, decimal cantidad, decimal precioUnitario)
    {
        this.Id = p.Id;
        this.Cantidad = cantidad;
        this.CodigoBarrasCaja = p.CodigoBarrasCaja;
        this.CodigoBarrasItem = p.CodigoBarrasItem;
        this.Nombre = p.Nombre;
        this.UnidadMedidaId = p.UnidadMedidaId;
        this.PrecioUnitario = precioUnitario;
    }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public class VentasPorDiaRowModel
{
    public VentasPorDiaRowModel
    (
        Venta venta,
        ProductoCantidadModel[] prods
    )
    {
        this.Venta = venta;
        this.Productos = prods;
    }

    public Venta Venta { get; set; }
    public ProductoCantidadModel[] Productos { get; set; }
}

public class VentasDiaResponseModel
{
    public VentasDiaResponseModel(DateTime fecha, VentasPorDiaRowModel[] ventas)
    {
        this.Fecha = fecha;
        this.Rows = ventas;

    }
    public DateTime Fecha { get; set; }
    public VentasPorDiaRowModel[] Rows { get; set; }
}

