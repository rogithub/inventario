using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;

namespace Ro.Inventario.Web.Repos;

public interface ICategoriasProductosRepo
{
    Task<int> Save(CategoriaProducto it);
    Task<CategoriaProducto> GetOne(Guid id);
    Task<int> BulkSave(CategoriaProducto[] list);
}

public class CategoriasProductosRepo : ICategoriasProductosRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public CategoriasProductosRepo(IDbAsync db)
    {
        this.Db = db;
    }

    public Task<int> BulkSave(CategoriaProducto[] list)
    {                
        var parameters = new List<IDbDataParameter>();
        var sqlLine = @"INSERT INTO CategoriasProductos (Id,ProductoId,CategoriaId) VALUES (@id{0},@productoId{0},@categoriaId{0});";
        var sb = new StringBuilder();
        for (int i = 0; i < list.Length; i++)
        {   
            var it = list[i];
            sb.AppendLine(string.Format(sqlLine, i));
            parameters.Add(string.Format("@id{0}", i).ToParam(DbType.String, it.Id.ToString()));
            parameters.Add(string.Format("@productoId{0}", i).ToParam(DbType.String, it.ProductoId.ToString()));
            parameters.Add(string.Format("@categoriaId{0}", i).ToParam(DbType.String, it.CategoriaId.ToString()));
        }
        var cmd = sb.ToString().ToCmd(parameters.ToArray());
        return Db.ExecuteNonQuery(cmd);
    }
    
    public Task<int> Save(CategoriaProducto it)
    {
        var sql = @"INSERT INTO CategoriasProductos (Id,ProductoId,CategoriaId) VALUES 
                    (@id,@productoId,@categoriaId);";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, it.Id.ToString()),
            "@productoId".ToParam(DbType.String, it.ProductoId.ToString()),
            "@categoriaId".ToParam(DbType.String, it.CategoriaId.ToString())
        );
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<CategoriaProducto> GetOne(Guid id)
    {
        var sql = "SELECT Id,ProductoId,CategoriaId FROM CategoriasProductos WHERE Id = @id";
        var cmd = sql.ToCmd
        (            
            "@id".ToParam(DbType.String, id.ToString())
        );
        return Db.GetOneRow(cmd, GetData);
    }
   

    private CategoriaProducto GetData(IDataReader dr)
    {
        return new CategoriaProducto()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            ProductoId = Guid.Parse(dr.GetString("ProductoId")),
            CategoriaId = Guid.Parse(dr.GetString("CategoriaId"))
        };
    }   
}