using System;
using System.Linq;
using Ro.Inventario.Web.Models;
namespace Ro.Inventario.Web.Services;

public interface INuevosProductosValidatorService
{
    bool ValidateProducts(IEnumerable<string> list);
    IEnumerable<ProductoNuevoLinea> ParseProducts(IEnumerable<string> list);
}

public class NuevosProductosValidatorService : INuevosProductosValidatorService
{
    private readonly ILogger<NuevosProductosValidatorService> _logger;
    public NuevosProductosValidatorService(ILogger<NuevosProductosValidatorService> logger)
    {
        _logger = logger;
    }
    
    private bool IsValid(string line)
    {
        decimal cantidad, compra, venta;
        var errors = new List<string>();
        var length = 9;
        var arr = line.Split(",");
        var id = Guid.Empty; 
        if (!string.IsNullOrWhiteSpace(arr[0]) && Guid.TryParse(arr[0], out id)) errors.Add("0 Guid no valido");
        if (string.IsNullOrWhiteSpace(arr[1])) errors.Add("1 Nombre vacío");
        if (!decimal.TryParse(arr[2], out cantidad)) errors.Add("2 Cantidad no vália");
        if (!decimal.TryParse(arr[3], out compra)) errors.Add("3 P.Compra no válido");
        if (!decimal.TryParse(arr[4], out venta)) errors.Add("4 P.Venta no válido");
        if (string.IsNullOrWhiteSpace(arr[7])) errors.Add("7 Unidad de Medida no válida");
        if (string.IsNullOrWhiteSpace(arr[8])) errors.Add("8 Categoría no válido");

        if (errors.Count > 0 || arr.Length != length)
        {
            _logger.LogInformation("Bad data length actual {l1} expected {l2}. Column errors: {errors}", 
            arr.Length, length, string.Join(",", errors.ToArray()).ToString());
            _logger.LogInformation("values: {a},{b},{c},{d},{e},{f},{g},{h},{i}",
            arr[0],arr[1],arr[2],arr[3],arr[4],arr[5],arr[6],arr[7],arr[8]);
        }

        return arr.Length == length && errors.Count == 0;
    }

    private ProductoNuevoLinea Parse(string line)
    {        
        var id = Guid.Empty; 
        var arr = line.Split(",");
        Guid.TryParse(arr[0], out id);
        decimal.TryParse(arr[2], out var cantidad);
        decimal.TryParse(arr[3], out var compra);
        decimal.TryParse(arr[4], out var venta);

        return new ProductoNuevoLinea(id)
        {
            // do not add id here
            Nombre = arr[1],
            Cantidad = cantidad,
            PrecioCompra = compra,
            PrecioVenta = venta,
            CodigoBarrasItem = Convert.ToString(arr[5]),
            CodigoBarrasCaja = Convert.ToString(arr[6]),
            UnidadDeMedida = Convert.ToString(arr[7]),
            Categoria = Convert.ToString(arr[8])
        };
    }

    public bool ValidateProducts(IEnumerable<string> list)
    {        
        return list.Skip(1).All((line) => IsValid(line));
    }

    public IEnumerable<ProductoNuevoLinea> ParseProducts(IEnumerable<string> list)
    {        
        return list.Skip(1).Select(line => Parse(line));
    }
}
