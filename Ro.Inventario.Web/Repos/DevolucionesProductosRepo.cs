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

    /***
    -- Productos en buen estado vuelven al stock, por el hecho de reducir su cantidad en
    -- la venta original. Para productos en mal estado, se reduce la cantidad en la
    -- venta y se captura una merma. 
    ***/
    public Task<int> BulkSave(DevolucionProducto[] list)
    {
        var parameters = new List<IDbDataParameter>();
        var sqlLine = @"INSERT INTO DevolucionesProductos (Id,AjusteProductoId,CantidadEnBuenasCondiciones,CantidadEnMalasCondiciones,FechaCreado,UserUpdatedId) 
                        VALUES (@id{0},@ajusteProductoId{0},@cantidadEnBuenasCondiciones{0},@cantidadEnMalasCondiciones{0},@fechaCreado{0},@userUpdatedId{0});";
        var sb = new StringBuilder();
        for (int i = 0; i < list.Length; i++)
        {
            var it = list[i];
            sb.AppendLine(string.Format(sqlLine, i));
            parameters.Add(string.Format("@id{0}", i).ToParam(DbType.String, it.Id.ToString()));
            parameters.Add(string.Format("@ajusteProductoId{0}", i).ToParam(DbType.String, it.AjusteProductoId.ToString()));
            parameters.Add(string.Format("@cantidadEnBuenasCondiciones{0}", i).ToParam(DbType.Decimal, it.CantidadEnBuenasCondiciones));
            parameters.Add(string.Format("@cantidadEnMalasCondiciones{0}", i).ToParam(DbType.Decimal, it.CantidadEnMalasCondiciones));
            parameters.Add(string.Format("@fechaCreado{0}", i).ToParam(DbType.String, it.FechaCreado.ToString(DATE_FORMAT)));
            parameters.Add(string.Format("@userUpdatedId{0}", i).ToParam(DbType.String, it.UserUpdatedId.ToString()));            

            if (it.CantidadEnBuenasCondiciones > 0 || it.CantidadEnMalasCondiciones > 0)
            {
                var suma = (it.CantidadEnBuenasCondiciones+it.CantidadEnMalasCondiciones);
                var updateAjusteProd = @"UPDATE AjustesProductos 
                                            SET Cantidad = (Cantidad - @updateCantidad{0}), 
                                                UserUpdatedId=@userUpdatedId_uno{0}, 
                                                DateStamp=@dateStamp_uno{0} 
                                          WHERE Id=@updateApId{0};";
                sb.AppendLine(string.Format(updateAjusteProd, i));
                parameters.Add(string.Format("@updateCantidad{0}", i).ToParam(DbType.Decimal, suma));
                parameters.Add(string.Format("@updateApId{0}", i).ToParam(DbType.String, it.AjusteProductoId.ToString()));
                parameters.Add(string.Format("@userUpdatedId_uno{0}", i).ToParam(DbType.String, it.UserUpdatedId.ToString()));
                parameters.Add(string.Format("@dateStamp_uno{0}", i).ToParam(DbType.String, DateTime.Now.ToString(DATE_FORMAT)));
            }

            if (it.CantidadEnMalasCondiciones > 0)
            {
                Guid ajusteId = Guid.NewGuid();
                var insertAjuste = @"INSERT INTO Ajustes (Id,Pago,Cambio,FechaAjuste,TipoAjuste,IvaVenta,UserUpdatedId) VALUES 
                                                         (@ajusteId{0},0,0,@ajusteFecha{0},@tipoAjuste{0},0, @userUpdatedId_dos{0});";
                sb.AppendLine(string.Format(insertAjuste, i));
                
                parameters.Add(string.Format("@ajusteId{0}", i).ToParam(DbType.String, ajusteId.ToString()));
                parameters.Add(string.Format("@ajusteFecha{0}", i).ToParam(DbType.String, DateTime.Now.ToString(DATE_FORMAT)));
                parameters.Add(string.Format("@tipoAjuste{0}", i).ToParam(DbType.Int32, (int)TipoAjuste.Merma));
                parameters.Add(string.Format("@userUpdatedId_dos{0}", i).ToParam(DbType.String, it.UserUpdatedId.ToString()));

                var insertAjusteProd = @"INSERT INTO AjustesProductos (Id,ProductoId,AjusteId,Cantidad,PrecioUnitarioVenta,Notas,UserUpdatedId,DateStamp)
                                SELECT @insertApId{0},ProductoId,@nuevoAjuste{0},@insertApCantidad{0},PrecioUnitarioVenta,@mermaNotas{0},@userUpdatedId_tres{0},dateStamp_tres{0}
                                 FROM AjustesProductos WHERE Id=@existingApId{0};";
                sb.AppendLine(string.Format(insertAjusteProd, i));
                
                parameters.Add(string.Format("@insertApId{0}", i).ToParam(DbType.String, Guid.NewGuid().ToString()));
                parameters.Add(string.Format("@nuevoAjuste{0}", i).ToParam(DbType.String, ajusteId.ToString()));
                parameters.Add(string.Format("@insertApCantidad{0}", i).ToParam(DbType.Decimal, it.CantidadEnMalasCondiciones));                
                parameters.Add(string.Format("@mermaNotas{0}", i).ToParam(DbType.String, string.Format("Merma por devolucionId {0}", it.Id.ToString())));
                parameters.Add(string.Format("@existingApId{0}", i).ToParam(DbType.String, it.AjusteProductoId.ToString()));
                parameters.Add(string.Format("@userUpdatedId_tres{0}", i).ToParam(DbType.String, it.UserUpdatedId.ToString()));
                parameters.Add(string.Format("@dateStamp_tres{0}", i).ToParam(DbType.String, DateTime.Now.ToString(DATE_FORMAT)));
            }
        }
        var cmd = sb.ToString().ToCmd(parameters.ToArray());
        return Db.ExecuteNonQuery(cmd);
    }   
}