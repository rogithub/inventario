using System;
using Ro.Inventario.Web.Entities;
using Newtonsoft.Json;
namespace Ro.Inventario.Web.Models;

public class DevolucionProductoSaveModel
{
    public DevolucionProductoSaveModel()
    {        
        AjusteProductoId = Guid.NewGuid();        
    }    
    [JsonProperty(PropertyName = "ajusteProductoId")]
    public Guid AjusteProductoId { get; set; }
    [JsonProperty(PropertyName = "cantidadEnBuenasCondiciones")]
    public decimal CantidadEnBuenasCondiciones { get; set; }
    [JsonProperty(PropertyName = "cantidadEnMalasCondiciones")]
    public decimal CantidadEnMalasCondiciones { get; set; }    
}