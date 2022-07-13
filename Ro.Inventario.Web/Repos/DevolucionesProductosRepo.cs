using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;

namespace Ro.Inventario.Web.Repos;

public interface IDevolucionesProductosRepo
{        
    Task<int> BulkSave(DevolucionProducto[] list);    
}

public class DevolucionesProductosRepo : IDevolucionesProductosRepo
{
    private const string DATE_FORMAT = "yyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db;
    public DevolucionesProductosRepo(IDbAsync db)
    {
        this.Db = db;
    }

    public Task<int> BulkSave(DevolucionProducto[] list)
    {
        var parameters = new List<IDbDataParameter>();
        var sqlLine = @"INSERT INTO DevolucionesProductos (Id,AjusteProductoId,CantidadEnBuenasCondiciones,CantidadEnMalasCondiciones,FechaCreado) 
                        VALUES (@id{0},@ajusteProductoId{0},@cantidadEnBuenasCondiciones{0},@cantidadEnMalasCondiciones{0},@FechaCreado{0});";
        var sb = new StringBuilder();
        for (int i = 0; i < list.Length; i++)
        {
            var it = list[i];
            sb.AppendLine(string.Format(sqlLine, i));
            parameters.Add(string.Format("@id{0}", i).ToParam(DbType.String, it.Id.ToString()));
            parameters.Add(string.Format("@ajusteProductoId{0}", i).ToParam(DbType.String, it.AjusteProductoId.ToString()));
            parameters.Add(string.Format("@cantidadEnBuenasCondiciones{0}", i).ToParam(DbType.Decimal, it.CantidadEnBuenasCondiciones));
            parameters.Add(string.Format("@cantidadEnMalasCondiciones{0}", i).ToParam(DbType.Decimal, it.CantidadEnMalasCondiciones));
            parameters.Add(string.Format("@FechaCreado{0}", i).ToParam(DbType.String, it.FechaCreado.ToString(DATE_FORMAT)));        
        }
        var cmd = sb.ToString().ToCmd(parameters.ToArray());
        return Db.ExecuteNonQuery(cmd);
    }   
}