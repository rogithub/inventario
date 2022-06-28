using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Ro.SQLite.Data
{
    public interface IDbAsync
    {
        Task<int> ExecuteNonQuery(DbCommand cmd);
        Task<object> ExecuteScalar(DbCommand cmd);
        Task ExecuteReader(DbCommand cmd, Action<IDataReader> mapper, CommandBehavior behavior = CommandBehavior.CloseConnection);
        Task<T> GetOneRow<T>(DbCommand cmd, Func<IDataReader, T> mapper);
        Task<IEnumerable<T>> GetRows<T>(DbCommand cmd, Func<IDataReader, T> mapper);
        Task<T> GetOneRowAsync<T>(DbCommand cmd, Func<IDataReader, Task<T>> mapper);
        Task<IEnumerable<T>> GetRowsAsync<T>(DbCommand cmd, Func<IDataReader, Task<T>> mapper);
    }
}
