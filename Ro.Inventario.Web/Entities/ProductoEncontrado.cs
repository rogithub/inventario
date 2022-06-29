namespace Ro.Inventario.Web.Entities;

public class ProductoEncontrado
{
    public ProductoEncontrado()
    {
        Id = Guid.NewGuid();
        Nombre = string.Empty;
        UnidadMedida = string.Empty;
        Categoria = string.Empty;
        CodigoBarrasItem = string.Empty;
        CodigoBarrasCaja = string.Empty;            
    }
    public int Nid { get; set; }
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public string UnidadMedida { get; set; }
    public string Categoria { get; set; }
    public string CodigoBarrasItem { get; set; }
    public string CodigoBarrasCaja { get; set; }
    public decimal PrecioVenta { get; set; }
}