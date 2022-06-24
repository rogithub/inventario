using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;

namespace Ro.Inventario.Web.Repos;

public interface IComprasProductosRepo
{
    Task<int> Save(CompraProducto it);
    Task<CompraProducto> GetOne(Guid id);
    Task<int> BulkSave(CompraProducto[] list);
}

public class ComprasProductosRepo : IComprasProductosRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public ComprasProductosRepo(IDbAsync db)
    {
        this.Db = db;
    }

    public Task<int> BulkSave(CompraProducto[] list)
    {                
        var parameters = new List<IDbDataParameter>();
        var sqlLine = @"INSERT INTO ComprasProductos (Id,ProductoId,CompraId,Cantidad,PrecioCompra) VALUES (@id{0},@productoId{0},@compraId{0},@cantidad{0},@precioCompra{0});";
        var sb = new StringBuilder();
        for (int i = 0; i < list.Length; i++)
        {   
            var it = list[i];
            sb.AppendLine(string.Format(sqlLine, i));
            parameters.Add(string.Format("@id{0}", i).ToParam(DbType.String, it.Id.ToString()));
            parameters.Add(string.Format("@productoId{0}", i).ToParam(DbType.String, it.ProductoId.ToString()));
            parameters.Add(string.Format("@compraId{0}", i).ToParam(DbType.String, it.CompraId.ToString()));
            parameters.Add(string.Format("@cantidad{0}", i).ToParam(DbType.Decimal, it.Cantidad));
            parameters.Add(string.Format("@precioCompra{0}", i).ToParam(DbType.Decimal, it.PrecioCompra));
        }
        var cmd = sb.ToString().ToCmd(parameters.ToArray());
        return Db.ExecuteNonQuery(cmd);
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