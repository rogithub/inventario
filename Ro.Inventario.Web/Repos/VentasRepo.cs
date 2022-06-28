using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;

namespace Ro.Inventario.Web.Repos;

public interface IVentasRepo
{
    Task<int> Save(Venta it);
    Task<Venta> GetOne(Guid id);
}

public class VentasRepo : IVentasRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public VentasRepo(IDbAsync db)
    {
        this.Db = db;
    }
    
    public Task<int> Save(Venta it)
    {
        var sql = @"INSERT INTO Ajustes (Id,Pago,Cambio,FechaAjuste) VALUES 
                    (@id,@pago,@cambio,@fechaAjuste);";
        var cmd = sql.ToCmd
        (            
            "@id".ToParam(DbType.String, it.Id.ToString()),
            "@pago".ToParam(DbType.Decimal, it.Pago),
            "@cambio".ToParam(DbType.Decimal, it.Cambio),
            "@fechaAjuste".ToParam(DbType.String, it.FechaVenta.ToString(DATE_FORMAT))
        );
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<Venta> GetOne(Guid id)
    {
        var sql = "SELECT Id,Pago,Cambio,FechaAjuste FROM Ajustes WHERE Id = @id";
        var cmd = sql.ToCmd
        (            
            "@id".ToParam(DbType.String, id.ToString())
        );
        return Db.GetOneRow(cmd, GetData);
    }
   

    private Venta GetData(IDataReader dr)
    {
        return new Venta()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            Pago = dr.GetDecimal("Pago"),
            Cambio = dr.GetDecimal("Cambio"),
            FechaVenta = DateTime.Parse(dr.GetString("FechaAjuste"))
        };
    }   
}