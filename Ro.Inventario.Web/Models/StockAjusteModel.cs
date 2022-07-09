namespace Ro.Inventario.Web.Models;


public class StockAjusteModel
{
    public StockAjusteModel()
    {
        this.ProductoId = Guid.NewGuid();
        this.TipoAjuste = TipoAjuste.IngresoSinCompra;
        this.Motivo = string.Empty;
    }

    public Guid ProductoId { get; set; }
    public TipoAjuste TipoAjuste { get; set; }
    public decimal Cantidad { get; set; }
    public string Motivo { get; set; }
}