using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Ro.SQLite.Data
{
    public class Database : IDbAsync
    {
        private IDbTasks _dbTasks;
        private string ConnectionString { get; set; }
        public Database(string connString, IDbTasks dbTasks)
        {
            this.ConnectionString = connString;
            this._dbTasks = dbTasks;
        }

        private DbConnection GetConnection()
        {            
            return new SQLiteConnection(this.ConnectionString);
        }

        public Task<int> ExecuteNonQuery(DbCommand cmd)
        {
            return _dbTasks.ExecuteNonQueryAsync(cmd, GetConnection());
        }

        public Task<object> ExecuteScalar(DbCommand cmd)
        {
            return _dbTasks.ExecuteScalarAsync(cmd, GetConnection());
        }

        public Task ExecuteReader(DbCommand cmd, Action<IDataReader> action, CommandBehavior behavior = CommandBehavior.CloseConnection)
        {
            return _dbTasks.ExecuteReaderAsync(cmd, GetConnection(), action, behavior);
        }

        public Task<T> GetOneRow<T>(DbCommand cmd, Func<IDataReader, T> mapper)
        {
            return _dbTasks.GetOneRow(cmd, GetConnection(), mapper);
        }

        public Task<IEnumerable<T>> GetRows<T>(DbCommand cmd, Func<IDataReader, T> mapper)
        {
            return _dbTasks.GetRows(cmd, GetConnection(), mapper);
        }

        public Task<T> GetOneRowAsync<T>(DbCommand cmd, Func<IDataReader, Task<T>> mapper)
        {
            return _dbTasks.GetOneRowAsync(cmd, GetConnection(), mapper);
        }

        public Task<IEnumerable<T>> GetRowsAsync<T>(DbCommand cmd, Func<IDataReader, Task<T>> mapper)
        {
            return _dbTasks.GetRowsAsync(cmd, GetConnection(), mapper);
        }
    }
}
