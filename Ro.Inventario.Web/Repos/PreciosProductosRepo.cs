using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;

namespace Ro.Inventario.Web.Repos;

public interface IPreciosProductosRepo
{
    Task<int> Save(PrecioProducto it);
    Task<PrecioProducto> GetOne(Guid id);
    Task<PrecioProducto> GetOneForProduct(Guid productoId);
    Task<int> BulkSave(PrecioProducto[] list);
}

public class PreciosProductosRepo : IPreciosProductosRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public PreciosProductosRepo(IDbAsync db)
    {
        this.Db = db;
    }


    public Task<int> BulkSave(PrecioProducto[] list)
    {
        var parameters = new List<IDbDataParameter>();
        var sqlLine = "INSERT INTO PreciosProductos (Id,ProductoId,FechaCreado,PrecioVenta) VALUES (@id{0},@productoId{0},@fechaCreado{0},@precioVenta{0});";
        var sb = new StringBuilder();
        for (int i = 0; i < list.Length; i++)
        {
            var it = list[i];
            sb.AppendLine(string.Format(sqlLine, i));
            parameters.Add(string.Format("@id{0}", i).ToParam(DbType.String, it.Id.ToString()));
            parameters.Add(string.Format("@productoId{0}", i).ToParam(DbType.String, it.ProductoId.ToString()));
            parameters.Add(string.Format("@fechaCreado{0}", i).ToParam(DbType.String, it.FechaCreado.ToString(DATE_FORMAT)));
            parameters.Add(string.Format("@precioVenta{0}", i).ToParam(DbType.Decimal, it.PrecioVenta));
        }
        var cmd = sb.ToString().ToCmd(parameters.ToArray());
        return Db.ExecuteNonQuery(cmd);
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

    public Task<PrecioProducto> GetOneForProduct(Guid productoId)
    {
        var sql = @"SELECT    Id,ProductoId,FechaCreado,PrecioVenta 
                      FROM    PreciosProductos 
                     WHERE    ProductoId = @productoId
                     ORDER BY datetime(FechaCreado) DESC LIMIT 1;";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, productoId.ToString())
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
            PrecioVenta = dr.GetDecimal("PrecioVenta")
        };
    }
}