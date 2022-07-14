using Ro.SQLite.Data;
using System.Data;

namespace Ro.Inventario.Web.Repos;

public interface IAjustesRepo
{
    Task<int> Save(Ajuste it);
    Task<Ajuste> GetOne(Guid id);
    Task<IEnumerable<Ajuste>> VentasPorFecha(DateTime date);
}

public class AjustesRepo : IAjustesRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public AjustesRepo(IDbAsync db)
    {
        this.Db = db;
    }

    public Task<int> Save(Ajuste it)
    {
        var sql = @"INSERT INTO Ajustes (Id,Pago,Cambio,FechaAjuste,TipoAjuste,IvaVenta) VALUES 
                    (@id,@pago,@cambio,@fechaAjuste,@tipoAjuste,@ivaVenta);";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, it.Id.ToString()),
            "@pago".ToParam(DbType.Decimal, it.Pago),
            "@cambio".ToParam(DbType.Decimal, it.Cambio),
            "@fechaAjuste".ToParam(DbType.String, it.FechaAjuste.ToString(DATE_FORMAT)),
            "@tipoAjuste".ToParam(DbType.Int32, (int)it.TipoAjuste),
            "@ivaVenta".ToParam(DbType.Decimal, it.Iva)
        );
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<Ajuste> GetOne(Guid id)
    {
        var sql = "SELECT Id,Pago,Cambio,FechaAjuste,TipoAjuste,IvaVenta FROM Ajustes WHERE Id = @id";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, id.ToString())
        );
        return Db.GetOneRow(cmd, GetData);
    }
    public Task<IEnumerable<Ajuste>> VentasPorFecha(DateTime fecha)
    {
        var sql = @"SELECT * FROM Ajustes 
                    WHERE    TipoAjuste = 0 AND FechaAjuste 
                    BETWEEN  DATE(@fecha,'start of day') AND DATE(@fecha,'start of day', '+1 day') 
                    ORDER BY FechaAjuste;";
        var cmd = sql.ToCmd
        (
            "@fecha".ToParam(DbType.DateTime, fecha.ToString(DATE_FORMAT))
        );
        return Db.GetRows(cmd, GetData);
    }

    private Ajuste GetData(IDataReader dr)
    {
        return new Ajuste()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            Pago = dr.GetDecimal("Pago"),
            Cambio = dr.GetDecimal("Cambio"),
            FechaAjuste = DateTime.Parse(dr.GetString("FechaAjuste")),
            TipoAjuste = (TipoAjuste)(dr.GetInt("TipoAjuste")),
            Iva = dr.GetDecimal("IvaVenta")
        };
    }
}