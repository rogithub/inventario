using Ro.Inventario.Web.Repos;
using Ro.SQLite.Data;
using System.Data.Common;
using System.Data;
using System.IO;
using Xunit.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Ro.Inventario.Web.Tests;

public static class DatabaseProvider
{
    public static string DbPath { get; set; }
    public static string InteropPath { get; set; }
    public static string ScriptsPath { get; set; }
    public static string[] InitScripts { get; set; }

    static DatabaseProvider()
    {
        InitScripts = new string[]
        {
            "inventario.sql",
            "reportes.sql"
        };
        Configure();
    }

    private static void Configure()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Testing.json", optional: true)
            .Build();
        DbPath = config.GetSection("DbPath").Value;
        InteropPath = config.GetSection("InteropPath").Value;
        ScriptsPath = config.GetSection("ScriptsPath").Value;
    }


    public static void InitDb()
    {
        File.Delete(DbPath);
        var db = GetDb();
        foreach (var fileName in InitScripts)
        {
            var path = Path.Join(ScriptsPath, fileName);
            string sql = File.ReadAllText(path);
            db.ExecuteNonQuery(sql.ToCmd()).Wait();
        }
    }

    public static IDbAsync GetDb()
    {
        var connString = string.Format("Data Source={0}; Version=3;", DbPath);
        return new Database(connString, new DbTasks(InteropPath));
    }
}