using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;
using System.Linq;

namespace Ro.Inventario.Web.Repos;

public interface IProductosRepo
{
    Task<int> Save(Producto it);
    Task<Producto> GetOne(Guid id);
}

public class ProductosRepo : IProductosRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public ProductosRepo(IDbAsync db)
    {
        this.Db = db;
    }
    
    public Task<int> Save(Producto it)
    {
        var sql = "INSERT INTO Productos (Id,Nombre,UnidadMedidaId,CodigoBarrasItem,CodigoBarrasCaja) VALUES (@id,@nombre,@unidadMedidaId,@codigoItem,@codigoCaja)";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, it.Id.ToString()),
            "@nombre".ToParam(DbType.String, it.Id.ToString()),
            "@unidadMedidaId".ToParam(DbType.String, it.Id.ToString()),
            "@codigoItem".ToParam(DbType.String, it.Id.ToString()),
            "@codigoCaja".ToParam(DbType.String, it.Id.ToString())
        );
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<Producto> GetOne(Guid id)
    {
        var sql = "SELECT Id,Nombre,UnidadMedidaId,CodigoBarrasItem,CodigoBarrasCaja FROM Productos WHERE Id = @id";
        var cmd = sql.ToCmd
        (            
            "@id".ToParam(DbType.String, id.ToString())
        );
        return Db.GetOneRow(cmd, GetData);
    }
   

    private Producto GetData(IDataReader dr)
    {
        return new Producto()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            UnidadMedidaId = Guid.Parse(dr.GetString("UnidadMedidaId")),
            Nombre = dr.GetString("Nombre"),
            CodigoBarrasItem = dr.GetString("CodigoBarrasItem"),
            CodigoBarrasCaja = dr.GetString("CodigoBarrasCaja"),            
        };
    }   
}