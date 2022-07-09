using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;

namespace Ro.Inventario.Web.Repos;

public interface IAjustesProductosRepo
{
    Task<int> Save(AjusteProducto it);
    Task<AjusteProducto> GetOne(Guid id);
    Task<int> BulkSave(AjusteProducto[] list);
    Task<IEnumerable<AjusteProducto>> GetForAjuste(Guid ajusteId);
}

public class AjustesProductosRepo : IAjustesProductosRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public AjustesProductosRepo(IDbAsync db)
    {
        this.Db = db;
    }

    public Task<int> BulkSave(AjusteProducto[] list)
    {
        var parameters = new List<IDbDataParameter>();
        var sqlLine = @"INSERT INTO AjustesProductos (Id,ProductoId,AjusteId,Cantidad,Notas) 
                        VALUES (@id{0},@productoId{0},@ajusteId{0},@cantidad{0},@notas{0});";
        var sb = new StringBuilder();
        for (int i = 0; i < list.Length; i++)
        {
            var it = list[i];
            sb.AppendLine(string.Format(sqlLine, i));
            parameters.Add(string.Format("@id{0}", i).ToParam(DbType.String, it.Id.ToString()));
            parameters.Add(string.Format("@productoId{0}", i).ToParam(DbType.String, it.ProductoId.ToString()));
            parameters.Add(string.Format("@ajusteId{0}", i).ToParam(DbType.String, it.AjusteId.ToString()));
            parameters.Add(string.Format("@cantidad{0}", i).ToParam(DbType.Decimal, it.Cantidad));
            parameters.Add(string.Format("@notas{0}", i).ToParam(DbType.String, it.Notas));
        }
        var cmd = sb.ToString().ToCmd(parameters.ToArray());
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<int> Save(AjusteProducto it)
    {
        var sql = @"INSERT INTO AjustesProductos (Id,ProductoId,AjusteId,Cantidad,Notas) VALUES 
                    (@id,@productoId,@ajusteId,@cantidad,@notas);";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, it.Id.ToString()),
            "@productoId".ToParam(DbType.String, it.ProductoId.ToString()),
            "@ajusteId".ToParam(DbType.String, it.AjusteId.ToString()),
            "@cantidad".ToParam(DbType.Decimal, it.Cantidad),
            "@notas".ToParam(DbType.String, it.Notas)
        );
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<AjusteProducto> GetOne(Guid id)
    {
        var sql = "SELECT Id,ProductoId,AjusteId,Cantidad,Notas FROM AjustesProductos WHERE Id = @id";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, id.ToString())
        );
        return Db.GetOneRow(cmd, GetData);
    }

    public Task<IEnumerable<AjusteProducto>> GetForAjuste(Guid ajusteId)
    {
        var sql = @"SELECT    Id,ProductoId,AjusteId,Cantidad,Notas
                      FROM    AjustesProductos 
                     WHERE    AjusteId = @ajusteId;";
        var cmd = sql.ToCmd
        (
            "@ajusteId".ToParam(DbType.String, ajusteId.ToString())
        );
        return Db.GetRows(cmd, GetData);
    }


    private AjusteProducto GetData(IDataReader dr)
    {
        return new AjusteProducto()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            ProductoId = Guid.Parse(dr.GetString("ProductoId")),
            AjusteId = Guid.Parse(dr.GetString("AjusteId")),
            Cantidad = dr.GetDecimal("Cantidad"),
            Notas = dr.GetString("Notas")
        };
    }
}