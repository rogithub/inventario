using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;
using System.Linq;

namespace Ro.Inventario.Web.Repos;

public interface IBusquedaProductosRepo
{
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

    public Task<IEnumerable<ProductoEncontrado>> FulSearchText(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern)) 
        {
            return Task.FromResult(Enumerable.Empty<ProductoEncontrado>());
        }
        var sql =
            @"
                SELECT nid,Id,Nombre,Categoria,UnidadMedida,PrecioVenta,CodigoBarrasItem,CodigoBarrasCaja FROM v_productos p WHERE             
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
            PrecioVenta =  dr.GetDecimal("PrecioVenta"),
            CodigoBarrasItem = dr.GetString("CodigoBarrasItem"),
            CodigoBarrasCaja = dr.GetString("CodigoBarrasCaja")
        };
    }   
}