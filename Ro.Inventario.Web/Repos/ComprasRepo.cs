using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;

namespace Ro.Inventario.Web.Repos;

public interface IComprasRepo
{
    Task<int> Save(Compra it);
    Task<Compra> GetOne(Guid id);
}

public class ComprasRepo : IComprasRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public ComprasRepo(IDbAsync db)
    {
        this.Db = db;
    }
    
    public Task<int> Save(Compra it)
    {
        var sql = @"INSERT INTO Compras (Id,Notas,FechaFactura,FechaCreado,CostoPaqueteria,TotalFactura,PorcentajeFacturaIVA) VALUES 
                    (@id,@notas,@fechaFactura,@fechaCreado,@costoPaqueteria,@totalFactura,@porcentajeFacturaIVA);";
        var cmd = sql.ToCmd
        (
            "@id".ToParam(DbType.String, it.Id.ToString()),
            "@notas".ToParam(DbType.String, it.Notas.ToString()),
            "@fechaFactura".ToParam(DbType.String, it.FechaFactura.ToString(DATE_FORMAT)),
            "@fechaCreado".ToParam(DbType.String, it.FechaCreado.ToString(DATE_FORMAT)),
            "@costoPaqueteria".ToParam(DbType.Decimal, it.CostoPaqueteria),
            "@totalFactura".ToParam(DbType.Decimal, it.TotalFactura),
            "@porcentajeFacturaIVA".ToParam(DbType.Decimal, it.PorcentajeFacturaIVA)
        );
        return Db.ExecuteNonQuery(cmd);
    }

    public Task<Compra> GetOne(Guid id)
    {
        var sql = "SELECT Id,Notas,FechaFactura,FechaCreado,CostoPaqueteria,TotalFactura,PorcentajeFacturaIVA FROM Compras WHERE Id = @id";
        var cmd = sql.ToCmd
        (            
            "@id".ToParam(DbType.String, id.ToString())
        );
        return Db.GetOneRow(cmd, GetData);
    }
   

    private Compra GetData(IDataReader dr)
    {
        return new Compra()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            Notas = dr.GetString("Notas"),
            FechaFactura = DateTime.Parse(dr.GetString("FechaFactura")),
            FechaCreado = DateTime.Parse(dr.GetString("FechaCreado")),
            CostoPaqueteria = dr.GetDecilmal("CostoPaqueteria"),
            TotalFactura = dr.GetDecilmal("TotalFactura"),
            PorcentajeFacturaIVA = dr.GetDecilmal("TotalFactura")
        };
    }   
}