using Ro.Inventario.Web.Repos;
using Ro.SQLite.Data;
using System.Data.Common;
using System.Data;
using System.IO;
using Xunit.Abstractions;


namespace Ro.Inventario.Web.Tests;

public class DatabaseProvider
{
    public string ConnectionString  { get; set; }
    public string InteropPath  { get; set; }
    public string ScriptsPath  { get; set; }
    public string[] InitScripts  { get; set; }
    public IDbAsync Db { get; set; }
    private readonly ITestOutputHelper output;

    public DatabaseProvider(ITestOutputHelper output)
    {
        this.output = output;
        this.ConnectionString = "Data Source=:memory:;";// "FullUri=file::memory:?cache=shared";
        this.InteropPath = "/home/ro/Documents/code/inventario/Ro.Inventario.Web/bin/Debug/net6.0/runtimes/linux-x64/native/SQLite.Interop.dll";
        this.ScriptsPath = "/home/ro/Documents/code/inventario/dbscripts";
        this.InitScripts = new string[] 
        {
            "inventario.sql",
            "reportes.sql"
        };

        this.Db = new Database(this.ConnectionString, new DbTasks(this.InteropPath));        
    }    

    public async Task InitDb()
    {        
        foreach (var fileName in this.InitScripts)
        {
            var path = Path.Join(this.ScriptsPath, fileName);
            string sql = File.ReadAllText(path);
            output.WriteLine(sql);
            await this.Db.ExecuteNonQuery(sql.ToCmd());
        }
    }
}