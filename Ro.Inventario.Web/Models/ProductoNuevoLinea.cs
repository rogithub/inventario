using System;
using System.Linq;
using Ro.Inventario.Web.Entities;
namespace Ro.Inventario.Web.Models;

public class ProductoNuevoLinea
{
    public ProductoNuevoLinea()
    {
        Id = Guid.NewGuid();
        Nombre = string.Empty;
        CodigoBarrasItem = string.Empty;
        CodigoBarrasCaja = string.Empty;
        UnidadDeMedida = string.Empty;
        Categoria = string.Empty;
    }
    public Guid Id { get; set; }
    public string Nombre { get; set; }    
    public decimal Cantidad { get; set; }
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public string CodigoBarrasItem { get; set; }
    public string CodigoBarrasCaja { get; set; }
    public string UnidadDeMedida { get; set; }
    public string Categoria { get; set; }

    public Producto ToEntity(Dictionary<string,Guid> unidadesMedida)
    {
        var p = new Producto();
        p.Id = this.Id;
        p.Nombre = this.Nombre;        
        p.CodigoBarrasItem = this.CodigoBarrasItem;
        p.CodigoBarrasCaja = this.CodigoBarrasCaja;
        p.UnidadMedidaId = unidadesMedida[this.UnidadDeMedida];        
            
        return p;
    }
}