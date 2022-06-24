using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;

namespace Ro.Inventario.Web.Repos;

public interface IPreciosProductos
{
    Task<int> Save(PrecioProducto it);
    Task<PrecioProducto> GetOne(Guid id);
}

public class PreciosProductosRepo : IPreciosProductos
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public PreciosProductosRepo(IDbAsync db)
    {
        this.Db = db;
    }
    
    public Task<int> Save(PrecioProducto it)
    {
        var sql = @"INSERT INTO PreciosProductos (Id,ProductoId,FechaCreado,PrecioVenta) VALUES 
                    (@id,@productoId,@fechaCreado,@precioVenta);";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, it.Id.ToString()),
            "@productoId".ToParam(DbType.String, it.ProductoId.ToString()),
            "@fechaCreado".ToParam(DbType.String, it.FechaCreado.ToString(DATE_FORMAT)),
            "@precioVenta".ToParam(DbType.Decimal, it.PrecioVenta)
        );
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<PrecioProducto> GetOne(Guid id)
    {
        var sql = "SELECT Id,ProductoId,FechaCreado,PrecioVenta FROM PreciosProductos WHERE Id = @id;";
        var cmd = sql.ToCmd
        (            
            "@id".ToParam(DbType.String, id.ToString())
        );
        return Db.GetOneRow(cmd, GetData);
    }
   

    private PrecioProducto GetData(IDataReader dr)
    {
        return new PrecioProducto()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            ProductoId = Guid.Parse(dr.GetString("ProductoId")),
            FechaCreado = DateTime.Parse(dr.GetString("FechaCreado")),
            PrecioVenta = dr.GetDecilmal("PrecioVenta")
        };
    }   
}