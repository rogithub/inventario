using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;
using System.Linq;

namespace Ro.Inventario.Web.Repos;

public interface IUnidadMedidaRepo
{
    Task<int> Save(UnidadMedida it);
    Task<UnidadMedida> GetOne(Guid id);
    Task<int> BulkSave(string[] list);
    Task<IEnumerable<UnidadMedida>> GetAll();
    Task<Dictionary<string,Guid>> GetDictionary();
}

public class UnidadMedidaRepo : IUnidadMedidaRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public UnidadMedidaRepo(IDbAsync db)
    {
        this.Db = db;
    }
    
    public Task<int> Save(UnidadMedida it)
    {
        var sql = "INSERT INTO UnidadesMedida (ID, NOMBRE) VALUES (@id, @nombre)";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, it.Id.ToString()),
            "@nombre".ToParam(DbType.String, it.Id.ToString())
        );
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<UnidadMedida> GetOne(Guid id)
    {
        var sql = "SELECT ID, NOMBRE FROM UnidadesMedida WHERE Id = @id";
        var cmd = sql.ToCmd
        (            
            "@id".ToParam(DbType.String, id.ToString())
        );
        return Db.GetOneRow(cmd, GetData);
    }

    public Task<int> BulkSave(string[] list)
    {
        var parameters = new List<IDbDataParameter>();
        var sqlLine = "INSERT INTO UnidadesMedida (Id,Nombre) SELECT @id{0}, @nombre{0} WHERE NOT EXISTS (SELECT Id FROM UnidadesMedida WHERE NOMBRE = @nombre{0});";
        var sb = new StringBuilder();
        for (int i = 0; i < list.Length; i++)
        {
            var it = list[i];
            sb.AppendLine(string.Format(sqlLine, i));
            parameters.Add(string.Format("@id{0}", i).ToParam(DbType.String, Guid.NewGuid().ToString()));
            parameters.Add(string.Format("@nombre{0}", i).ToParam(DbType.String, it));
        }
        var cmd = sb.ToString().ToCmd(parameters.ToArray());
        return Db.ExecuteNonQuery(cmd);
    }

    private UnidadMedida GetData(IDataReader dr)
    {
        return new UnidadMedida()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            Nombre = dr.GetString("Nombre")
        };
    }

    public Task<IEnumerable<UnidadMedida>> GetAll()
    {
        var sql = "SELECT ID, NOMBRE FROM UnidadesMedida";
        var cmd = sql.ToCmd();
        return Db.GetRows(cmd, GetData);
    }

    public async Task<Dictionary<string,Guid>> GetDictionary()
    {
        var d = new Dictionary<string,Guid>();

        var all = await GetAll();
        foreach (var it in all)
        {
            if (!d.ContainsKey(it.Nombre))
            {
                d.Add(it.Nombre, it.Id);
            }
        }
        
        return d;
    }
}