using System;
using System.Linq;
using Ro.Inventario.Web.Entities;
using Newtonsoft.Json;
namespace Ro.Inventario.Web.Models;

public class VentaProductoModel
{     
    [JsonProperty("productoId")]
    public Guid ProductoId { get; set; }
    [JsonProperty("cantidad")]
    public decimal Cantidad { get; set; }
}

public class VentaModel
{
    public VentaModel()
    {

        this.Items = new VentaProductoModel[] {};
    }
    [JsonProperty("pago")]
    public decimal Pago { get; set; }
    [JsonProperty("cambio")]
    public decimal Cambio { get; set; }
    [JsonProperty("items")]
    public VentaProductoModel[] Items { get; set; }
}