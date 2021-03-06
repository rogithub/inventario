using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;
using System.Linq;

namespace Ro.Inventario.Web.Repos;

public interface IProductosRepo
{
    Task<int> Save(Producto it);
    Task<int> Update(Producto it);
    Task<Producto> GetOne(Guid id);
    Task<int> BulkSave(Producto[] list);
}

public class ProductosRepo : IProductosRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public ProductosRepo(IDbAsync db)
    {
        this.Db = db;
    }

    public Task<int> BulkSave(Producto[] list)
    {
        var parameters = new List<IDbDataParameter>();
        var sqlLine = @"INSERT INTO Productos (Id,Nombre,UnidadMedidaId,CodigoBarrasItem,CodigoBarrasCaja,DateStamp,UserUpdatedId) VALUES 
                                               (@id{0},@nombre{0},@unidadMedidaId{0},@codigoItem{0},@codigoCaja{0},@dateStamp{0},@userUpdatedId{0});";
        var sb = new StringBuilder();
        for (int i = 0; i < list.Length; i++)
        {
            var it = list[i];
            sb.AppendLine(string.Format(sqlLine, i));
            parameters.Add(string.Format("@id{0}", i).ToParam(DbType.String, it.Id.ToString()));
            parameters.Add(string.Format("@nombre{0}", i).ToParam(DbType.String, it.Nombre));
            parameters.Add(string.Format("@unidadMedidaId{0}", i).ToParam(DbType.String, it.UnidadMedidaId.ToString()));
            parameters.Add(string.Format("@codigoItem{0}", i).ToParam(DbType.String, it.CodigoBarrasItem));
            parameters.Add(string.Format("@codigoCaja{0}", i).ToParam(DbType.String, it.CodigoBarrasCaja));
            parameters.Add(string.Format("@dateStamp{0}", i).ToParam(DbType.String, DateTime.Now.ToString(DATE_FORMAT)));
            parameters.Add(string.Format("@userUpdatedId{0}", i).ToParam(DbType.String, it.UserUpdatedId.ToString()));
        }
        var cmd = sb.ToString().ToCmd(parameters.ToArray());
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<int> Update(Producto it)
    {
        var sql = @"UPDATE Productos SET 
                        Nombre=@nombre,
                        UnidadMedidaId=@unidadMedidaId,
                        CodigoBarrasItem=@codigoItem,
                        CodigoBarrasCaja=@codigoCaja,
                        DateStamp=@dateStamp,
                        UserUpdatedId=@userUpdatedId                        
                    WHERE Id=@id";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, it.Id.ToString()),
            "@nombre".ToParam(DbType.String, it.Nombre),
            "@unidadMedidaId".ToParam(DbType.String, it.UnidadMedidaId.ToString()),
            "@codigoItem".ToParam(DbType.String, it.CodigoBarrasItem),
            "@codigoCaja".ToParam(DbType.String, it.CodigoBarrasCaja),
            "@dateStamp".ToParam(DbType.String, DateTime.Now.ToString(DATE_FORMAT)),
            "@userUpdatedId".ToParam(DbType.String, it.UserUpdatedId.ToString())
        );
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<int> Save(Producto it)
    {
        var sql = @"INSERT INTO Productos (Id,Nombre,UnidadMedidaId,CodigoBarrasItem,CodigoBarrasCaja,DateStamp,UserUpdatedId) 
                            VALUES (@id,@nombre,@unidadMedidaId,@codigoItem,@codigoCaja,@dateStamp,@userUpdatedId)";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, it.Id.ToString()),
            "@nombre".ToParam(DbType.String, it.Nombre),
            "@unidadMedidaId".ToParam(DbType.String, it.UnidadMedidaId.ToString()),
            "@codigoItem".ToParam(DbType.String, it.CodigoBarrasItem),
            "@codigoCaja".ToParam(DbType.String, it.CodigoBarrasCaja),
            "@dateStamp".ToParam(DbType.String, DateTime.Now.ToString(DATE_FORMAT)),
            "@userUpdatedId".ToParam(DbType.String, it.UserUpdatedId.ToString())
        );
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<Producto> GetOne(Guid id)
    {
        var sql = "SELECT Id,Nombre,UnidadMedidaId,CodigoBarrasItem,CodigoBarrasCaja,UserUpdatedId FROM Productos WHERE Id = @id";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, id.ToString())
        );
        return Db.GetOneRow(cmd, GetData);
    }

    private Producto GetData(IDataReader dr)
    {
        var userId = dr.GetString("UserUpdatedId");
        return new Producto()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            UserUpdatedId = string.IsNullOrWhiteSpace(userId) ? Guid.Empty : Guid.Parse(userId),
            UnidadMedidaId = Guid.Parse(dr.GetString("UnidadMedidaId")),
            Nombre = dr.GetString("Nombre"),
            CodigoBarrasItem = dr.GetString("CodigoBarrasItem"),
            CodigoBarrasCaja = dr.GetString("CodigoBarrasCaja"),
        };
    }
}