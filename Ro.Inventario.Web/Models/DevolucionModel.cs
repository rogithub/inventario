using System;
using System.Linq;
using Ro.Inventario.Web.Entities;
using Newtonsoft.Json;
namespace Ro.Inventario.Web.Models;


public class DevolucionProductoModel
{
    public DevolucionProductoModel()
    {
        this.Producto = new Producto();
        this.Categoria = string.Empty;
        this.UnidadMedida = string.Empty;
    }
    public DevolucionProductoModel(Producto p)
    {
        this.Producto = p;
        this.Categoria = string.Empty;
        this.UnidadMedida = string.Empty;
    }
    [JsonProperty(PropertyName = "producto")]
    public Producto Producto { get; set; }
    [JsonProperty(PropertyName = "ajusteProductoId")]
    public Guid AjusteProductoId { get; set; }
    [JsonProperty(PropertyName = "cantidad")]
    public decimal Cantidad { get; set; }
    [JsonProperty(PropertyName = "CantidadDevuelta")]
    public decimal CantidadDevuelta { get; set; }
    [JsonProperty(PropertyName = "PrecioUnitario")]
    public decimal PrecioUnitario { get; set; }

    [JsonProperty(PropertyName = "categoria")]
    public string Categoria { get; set; }
    [JsonProperty(PropertyName = "unidad medida")]
    public string UnidadMedida { get; set; }
}


public class DevolucionModel
{
    public DevolucionModel()
    {
        this.Venta = new Venta();
        this.Devueltos = new DevolucionProductoModel[] { };
    }

    public DevolucionModel(Venta v)
    {
        this.Venta = v;
        this.Devueltos = new DevolucionProductoModel[] { };
    }

    public Venta Venta { get; set; }
    public DevolucionProductoModel[] Devueltos { get; set; }
}