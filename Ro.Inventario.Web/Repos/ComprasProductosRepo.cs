using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;

namespace Ro.Inventario.Web.Repos;

public interface IComprasProductosRepo
{
    Task<int> Save(CompraProducto it);
    Task<CompraProducto> GetOne(Guid id);
}

public class ComprasProductosRepo : IComprasProductosRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public ComprasProductosRepo(IDbAsync db)
    {
        this.Db = db;
    }
    
    public Task<int> Save(CompraProducto it)
    {
        var sql = @"INSERT INTO ComprasProductos (Id,ProductoId,CompraId,Cantidad,PrecioCompra) VALUES 
                    (@id,@productoId,@compraId,@cantidad,@precioCompra);";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, it.Id.ToString()),
            "@productoId".ToParam(DbType.String, it.ProductoId.ToString()),
            "@compraId".ToParam(DbType.String, it.CompraId.ToString()),            
            "@cantidad".ToParam(DbType.Decimal, it.Cantidad),
            "@precioCompra".ToParam(DbType.Decimal, it.PrecioCompra)
        );
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<CompraProducto> GetOne(Guid id)
    {
        var sql = "SELECT Id,ProductoId,CompraId,Cantidad,PrecioCompra FROM ComprasProductos WHERE Id = @id";
        var cmd = sql.ToCmd
        (            
            "@id".ToParam(DbType.String, id.ToString())
        );
        return Db.GetOneRow(cmd, GetData);
    }
   

    private CompraProducto GetData(IDataReader dr)
    {
        return new CompraProducto()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            ProductoId = Guid.Parse(dr.GetString("ProductoId")),
            CompraId = Guid.Parse(dr.GetString("CompraId")),            
            Cantidad = dr.GetDecilmal("Cantidad"),
            PrecioCompra = dr.GetDecilmal("PrecioCompra")
        };
    }   
}