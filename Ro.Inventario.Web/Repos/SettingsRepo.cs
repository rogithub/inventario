using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;

namespace Ro.Inventario.Web.Repos;

public interface ISettingsRepo
{
    Task<T> GetValue<T>(string key, Func<string, T> parser);
}

public class SettingsRepo : ISettingsRepo
{
    private IDbAsync Db;
    public SettingsRepo(IDbAsync db)
    {
        this.Db = db;
    }

    public async Task<T> GetValue<T>(string key, Func<string, T> parser)
    {
        var sql = "SELECT Key, Value FROM Settings WHERE key = @key";
        var cmd = sql.ToCmd
        (
            "@nombre".ToParam(DbType.String, key)
        );
        var setting = await Db.GetOneRow(cmd, GetData);

        return parser(setting.Value);
    }

    private Setting GetData(IDataReader dr)
    {
        return new Setting()
        {
            Key = dr.GetString("Key"),
            Value = dr.GetString("Value")
        };
    }
}