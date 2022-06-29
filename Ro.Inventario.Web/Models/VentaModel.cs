using System;
using System.Linq;
using Ro.Inventario.Web.Entities;
using Newtonsoft.Json;
namespace Ro.Inventario.Web.Models;

public class VentaProductoModel
{     
    [JsonProperty(PropertyName = "productoId")]
    public Guid ProductoId { get; set; }
    [JsonProperty(PropertyName = "cantidad")]
    public decimal Cantidad { get; set; }
}

public class VentaModel
{
    public VentaModel()
    {
        this.Items = new VentaProductoModel[] {};
    }
    [JsonProperty(PropertyName = "pago")]
    public decimal Pago { get; set; }
    [JsonProperty(PropertyName = "cambio")]
    public decimal Cambio { get; set; }
    [JsonProperty(PropertyName = "items")]
    public VentaProductoModel[] Items { get; set; }
}