namespace Ro.Inventario.Web.Models;

public class ProductoModel
{
    public ProductoModel()
    {
        Id = Guid.NewGuid();
        UnidadMedidaId = Guid.NewGuid();
        CategoriaId = Guid.NewGuid();
        PrecioVentaId = Guid.NewGuid();

        Nombre = string.Empty;
        UnidadMedida = string.Empty;
        Categoria = string.Empty;
        CodigoBarrasItem = string.Empty;
        CodigoBarrasCaja = string.Empty;
    }
    public Guid Id { get; set; }
    public Guid UnidadMedidaId { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid PrecioVentaId { get; set; }
    public string Nombre { get; set; }
    public string UnidadMedida { get; set; }
    public string Categoria { get; set; }
    public string CodigoBarrasItem { get; set; }
    public string CodigoBarrasCaja { get; set; }
    public decimal PrecioVenta { get; set; }
}