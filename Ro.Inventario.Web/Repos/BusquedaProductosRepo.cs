using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;
using System.Linq;

namespace Ro.Inventario.Web.Repos;

public interface IBusquedaProductosRepo
{
    Task<IEnumerable<ProductoEncontrado>> EnCategoria(string pattern, Guid categoriaId);
    Task<IEnumerable<ProductoEncontrado>> FulSearchText(string pattern);
}

public class BusquedaProductosRepo : IBusquedaProductosRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public BusquedaProductosRepo(IDbAsync db)
    {
        this.Db = db;
    }

    public Task<IEnumerable<ProductoEncontrado>> EnCategoria(string pattern, Guid categoriaId)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return Task.FromResult(Enumerable.Empty<ProductoEncontrado>());
        }
        var sql =
            @"
                SELECT p.* FROM v_productos p JOIN CategoriasProductos c 
                WHERE c.CategoriaId in (@categoriaId) AND Nombre like '%@pattern%'
            ";
        var cmd = sql.ToCmd
        (
            "@categoriaId".ToParam(DbType.String, categoriaId.ToString()),
            "@pattern".ToParam(DbType.String, pattern)
        );
        return Db.GetRows(cmd, GetData);
    }

    public Task<IEnumerable<ProductoEncontrado>> FulSearchText(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return Task.FromResult(Enumerable.Empty<ProductoEncontrado>());
        }

        if (pattern.Split(" ").Length == 1)
        {
            pattern += "*";
        }

        var sql =
            @"
                SELECT p.* FROM v_productos p WHERE             
                nid IN (SELECT ROWID FROM Productos_fst WHERE Productos_fst MATCH @pattern ORDER BY rank)
                LIMIT 100
            ";
        var cmd = sql.ToCmd
        (
            "@pattern".ToParam(DbType.String, pattern)
        );
        return Db.GetRows(cmd, GetData);
    }


    private ProductoEncontrado GetData(IDataReader dr)
    {
        return new ProductoEncontrado()
        {
            Nid = dr.GetInt("nid"),
            Id = Guid.Parse(dr.GetString("Id")),
            Nombre = dr.GetString("Nombre"),
            Categoria = dr.GetString("Categoria"),
            UnidadMedida = dr.GetString("UnidadMedida"),
            PrecioVenta = dr.GetDecimal("PrecioVenta"),
            CodigoBarrasItem = dr.GetString("CodigoBarrasItem"),
            CodigoBarrasCaja = dr.GetString("CodigoBarrasCaja")
        };
    }
}