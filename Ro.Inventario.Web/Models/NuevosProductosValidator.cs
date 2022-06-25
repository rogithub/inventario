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
    }
    public Guid Id { get; set; }
    public string Nombre { get; set; }    
    public decimal Cantidad { get; set; }
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public string CodigoBarrasItem { get; set; }
    public string CodigoBarrasCaja { get; set; }
    public string UnidadDeMedida { get; set; }

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

public static class NuevosProductosValidator
{
    private static bool IsValid(string line)
    {
        var length = 7;
        var arr = line.Split(",");
        if (arr.Length != length) return false;
        if (string.IsNullOrWhiteSpace(arr[0])) return false;
        if (decimal.TryParse(arr[1], out var cantidad)) return false;
        if (decimal.TryParse(arr[2], out var compra)) return false;
        if (decimal.TryParse(arr[3], out var venta)) return false;

        return true;
    }

    private static ProductoNuevoLinea Parse(string line)
    {
        var arr = line.Split(",");
        decimal.TryParse(arr[1], out var cantidad);
        decimal.TryParse(arr[2], out var compra);
        decimal.TryParse(arr[3], out var venta);

        return new ProductoNuevoLinea()
        {
            Id = Guid.NewGuid(),
            Nombre = arr[0],
            Cantidad = cantidad,
            PrecioCompra = compra,
            PrecioVenta = venta,
            CodigoBarrasItem = Convert.ToString(arr[4]),
            CodigoBarrasCaja = Convert.ToString(arr[5]),
            UnidadDeMedida = Convert.ToString(arr[6])
        };
    }

    public static bool ValidateProducts(this IEnumerable<string> list)
    {        
        return list.Skip(1).All(line => IsValid(line)) && list.Count() >= 2;
    }

    public static IEnumerable<ProductoNuevoLinea> ParseProducts(this IEnumerable<string> list)
    {        
        return list.Skip(1).Select(line => Parse(line));
    }
}
