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
    Task<IEnumerable<ProductoEncontrado>> SearchByQr(string qr);    
    Task<ProductoEncontrado> GetOne(Guid id);
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
                SELECT p.* FROM v_inventario p WHERE             
                nid IN (SELECT ROWID FROM Productos_fst WHERE Productos_fst MATCH @pattern ORDER BY rank)
                LIMIT 100
            ";
        var cmd = sql.ToCmd
        (
            "@pattern".ToParam(DbType.String, pattern)
        );
        return Db.GetRows(cmd, GetData);
    }

    public Task<IEnumerable<ProductoEncontrado>> SearchByQr(string qr)
    {
        if (string.IsNullOrWhiteSpace(qr))
        {
            return Task.FromResult(Enumerable.Empty<ProductoEncontrado>());
        }
        
        var sql =
            @"
                SELECT p.* FROM v_inventario p WHERE 
                    Id=@id or 
                    CodigoBarrasItem=@codigoBarrasItem or 
                    CodigoBarrasCaja=@codigoBarrasCaja
                LIMIT 1
            ";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, qr),
            "@codigoBarrasItem".ToParam(DbType.String, qr),
            "@codigoBarrasCaja".ToParam(DbType.String, qr)
        );
        return Db.GetRows(cmd, GetData);
    }

    public Task<ProductoEncontrado> GetOne(Guid id)
    {        
        var sql =
            @"SELECT * FROM v_inventario WHERE id=@id;";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, id.ToString())
        );
        return Db.GetOneRow(cmd, GetData);
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
            PrecioVenta = dr.GetDecimal("UltimoPrecioVenta"),
            CodigoBarrasItem = dr.GetString("CodigoBarrasItem"),
            CodigoBarrasCaja = dr.GetString("CodigoBarrasCaja"),
            Stock = dr.GetDecimal("Stock")
        };
    }
}