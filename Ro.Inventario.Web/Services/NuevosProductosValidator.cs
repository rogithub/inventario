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
        var errors = new List<int>();
        var length = 8;
        var arr = line.Split(",");
        if (string.IsNullOrWhiteSpace(arr[0])) errors.Add(0);
        if (!decimal.TryParse(arr[1], out cantidad)) errors.Add(1);
        if (!decimal.TryParse(arr[2], out compra)) errors.Add(2);
        if (!decimal.TryParse(arr[3], out venta)) errors.Add(3);
        if (string.IsNullOrWhiteSpace(arr[6])) errors.Add(6);
        if (string.IsNullOrWhiteSpace(arr[7])) errors.Add(7);

        if (errors.Count > 0 || arr.Length != length)
        {
            _logger.LogInformation("Bad data length actual {l1} expected {l2}. Column errors: {errors}", 
            arr.Length, length, string.Join(",", errors.ToArray()).ToString());
            _logger.LogInformation("values: {a},{b},{c},{d},{e},{f},{g},{h}",
            arr[0],arr[1],arr[2],arr[3],arr[4],arr[5],arr[6],arr[7]);
        }

        return arr.Length == length && errors.Count == 0;
    }

    private ProductoNuevoLinea Parse(string line)
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
            UnidadDeMedida = Convert.ToString(arr[6]),
            Categoria = Convert.ToString(arr[7])
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
