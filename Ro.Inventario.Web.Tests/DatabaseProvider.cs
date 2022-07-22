using Ro.Inventario.Web.Repos;
using Ro.SQLite.Data;
using System.Data.Common;
using System.Data;
using System.IO;
using Xunit.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Ro.Inventario.Web.Tests;

public class DatabaseProvider
{
    public string DbPath  { get; set; }
    public string InteropPath  { get; set; }
    public string ScriptsPath  { get; set; }
    public string[] InitScripts  { get; set; }    
    private readonly ITestOutputHelper output;

    public DatabaseProvider(ITestOutputHelper output)
    {
        this.output = output;        
        this.InitScripts = new string[] 
        {
            "inventario.sql",
            "reportes.sql"
        };
        Configure();
    }    

    private void Configure()
    {        
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())            
            .AddJsonFile("appsettings.Testing.json", optional: true)
            .Build();
        this.DbPath = config.GetSection("DbPath").Value;
        this.InteropPath = config.GetSection("InteropPath").Value;
        this.ScriptsPath = config.GetSection("ScriptsPath").Value;

        output.WriteLine(this.DbPath);
        output.WriteLine(this.InteropPath);
        output.WriteLine(this.ScriptsPath);
    }


    public async Task InitDb()
    {     
        File.Delete(this.DbPath);
        var db = GetDb();
        foreach (var fileName in this.InitScripts)
        {
            var path = Path.Join(this.ScriptsPath, fileName);
            string sql = File.ReadAllText(path);            
            await db.ExecuteNonQuery(sql.ToCmd());
        }
    }

    public IDbAsync GetDb()
    {     
        var connString = string.Format("Data Source={0}; Version=3;", this.DbPath);
        return new Database(connString, new DbTasks(this.InteropPath));
    }
}