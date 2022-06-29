using System;
using System.Linq;
using Ro.Inventario.Web.Entities;
namespace Ro.Inventario.Web.Models;

public class VentaProductoModel
{     
    public Guid ProductoId { get; set; }
    public decimal Cantidad { get; set; }
}

public class VentaModel
{
    public VentaModel()
    {

        this.Items = new VentaProductoModel[] {};
    }
    public decimal Pago { get; set; }
    public decimal Cambio { get; set; }
    public VentaProductoModel[] Items { get; set; }
}