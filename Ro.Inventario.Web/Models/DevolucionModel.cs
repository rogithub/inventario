using System;
using System.Linq;
using Ro.Inventario.Web.Entities;
using Newtonsoft.Json;
namespace Ro.Inventario.Web.Models;


public class DevolucionProductoModel
{     
    [JsonProperty(PropertyName = "productoId")]
    public Guid ProductoId { get; set; }
    [JsonProperty(PropertyName = "cantidad")]
    public decimal Cantidad { get; set; }
    [JsonProperty(PropertyName = "motivo")]
    public string Motivo { get; set; }
}


public class DevolucionModel
{
    public DevolucionModel()
    {
        this.Venta = new Venta();
        this.Devueltos = new DevolucionProductoModel[] {};
    }
    
    public Venta Venta { get; set; }
    public DevolucionProductoModel[] Devueltos { get; set; }
}