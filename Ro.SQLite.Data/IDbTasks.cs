using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;


namespace Ro.SQLite.Data
{
    public interface IDbTasks
    {
                
        IDbConnection OpenConnection(IDbConnection conn);

        IDbConnection CloseConnection(IDbConnection conn);

        Task<object> ExecuteScalarAsync(DbCommand command, DbConnection connection);

        Task<IEnumerable<T>> GetRows<T>(DbCommand cmd, DbConnection conn, Func<IDataReader, T> mapper);

        Task<IEnumerable<T>> GetRowsAsync<T>(DbCommand cmd, DbConnection conn, Func<IDataReader, Task<T>> mapper);


        Task ExecuteReaderAsync(DbCommand command, DbConnection connection, Action<IDataReader> action, CommandBehavior behavior = CommandBehavior.CloseConnection);

        Task<T> GetOneRow<T>(DbCommand command, DbConnection connection, Func<IDataReader, T> mapper);

        Task<T> GetOneRowAsync<T>(DbCommand command, DbConnection connection, Func<IDataReader, Task<T>> mapper);

        Task<int> ExecuteNonQueryAsync(DbCommand command, DbConnection connection);

        Task<T> ExecuteAsync<T>(DbCommand cmd, DbConnection conn, Func<DbCommand, DbConnection, Task<T>> f);
    }
}
