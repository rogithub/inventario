using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;

namespace Ro.Inventario.Web.Repos;

public interface IVentasProductosRepo
{
    Task<int> Save(VentaProducto it);
    Task<VentaProducto> GetOne(Guid id);
    Task<int> BulkSave(VentaProducto[] list);
}

public class VentasProductosRepo : IVentasProductosRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public VentasProductosRepo(IDbAsync db)
    {
        this.Db = db;
    }

    public Task<int> BulkSave(VentaProducto[] list)
    {                
        var parameters = new List<IDbDataParameter>();
        var sqlLine = @"INSERT INTO VentasProductos (Id,ProductoId,VentaId,Cantidad) VALUES (@id{0},@productoId{0},@ventaId{0},@cantidad{0});";
        var sb = new StringBuilder();
        for (int i = 0; i < list.Length; i++)
        {   
            var it = list[i];
            sb.AppendLine(string.Format(sqlLine, i));
            parameters.Add(string.Format("@id{0}", i).ToParam(DbType.String, it.Id.ToString()));
            parameters.Add(string.Format("@productoId{0}", i).ToParam(DbType.String, it.ProductoId.ToString()));
            parameters.Add(string.Format("@ventaId{0}", i).ToParam(DbType.String, it.VentaId.ToString()));
            parameters.Add(string.Format("@cantidad{0}", i).ToParam(DbType.Decimal, it.Cantidad));
        }
        var cmd = sb.ToString().ToCmd(parameters.ToArray());
        return Db.ExecuteNonQuery(cmd);
    }
    
    public Task<int> Save(VentaProducto it)
    {
        var sql = @"INSERT INTO VentasProductos (Id,ProductoId,VentaId,Cantidad) VALUES 
                    (@id,@productoId,@ventaId,@cantidad);";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, it.Id.ToString()),
            "@productoId".ToParam(DbType.String, it.ProductoId.ToString()),
            "@ventaId".ToParam(DbType.String, it.VentaId.ToString()),
            "@cantidad".ToParam(DbType.Decimal, it.Cantidad)
        );
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<VentaProducto> GetOne(Guid id)
    {
        var sql = "SELECT Id,ProductoId,VentaId,Cantidad FROM VentasProductos WHERE Id = @id";
        var cmd = sql.ToCmd
        (            
            "@id".ToParam(DbType.String, id.ToString())
        );
        return Db.GetOneRow(cmd, GetData);
    }
   

    private VentaProducto GetData(IDataReader dr)
    {
        return new VentaProducto()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            ProductoId = Guid.Parse(dr.GetString("ProductoId")),
            VentaId = Guid.Parse(dr.GetString("VentaId")),
            Cantidad = dr.GetDecilmal("Cantidad")
        };
    }   
}