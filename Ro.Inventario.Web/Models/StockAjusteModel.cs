using Newtonsoft.Json;
namespace Ro.Inventario.Web.Models;

public class StockAjusteModel
{
    public StockAjusteModel()
    {
        this.ProductoId = Guid.NewGuid();
        this.TipoAjuste = TipoAjuste.IngresoSinCompra;
        this.Motivo = string.Empty;
    }

    [JsonProperty(PropertyName = "productoId")]
    public Guid ProductoId { get; set; }
    [JsonProperty(PropertyName = "tipoAjuste")]
    public TipoAjuste TipoAjuste { get; set; }
    [JsonProperty(PropertyName = "cantidad")]
    public decimal Cantidad { get; set; }
    [JsonProperty(PropertyName = "motivo")]
    public string Motivo { get; set; }
}