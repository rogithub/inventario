using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Ro.SQLite.Data
{
    public class DbTasks: IDbTasks
    {
        private string _interopPath;
        public DbTasks(string interopPath)
        {
            this._interopPath = interopPath;
        }

        public IDbConnection OpenConnection(IDbConnection conn)
        {                
            if (conn.State != System.Data.ConnectionState.Open)
            {                
                conn.Open();
                var c = conn as SQLiteConnection;
                c.EnableExtensions(true);
                c.LoadExtension(_interopPath, "sqlite3_fts5_init");
            }
            return conn;
        }

        public IDbConnection CloseConnection(IDbConnection conn)
        {            
            if (conn.State != System.Data.ConnectionState.Closed)
            {
                conn.Close();
            }
            return conn;
        }

        public async Task<object> ExecuteScalarAsync(DbCommand command, DbConnection connection)
        {
            return await ExecuteAsync<object>(command, connection, async (cmd, conn) =>
            {
                return await cmd.ExecuteScalarAsync();
            });
        }

        public async Task<IEnumerable<T>> GetRows<T>(DbCommand cmd, DbConnection conn, Func<IDataReader, T> mapper)
        {
            List<T> list = new List<T>();
            await ExecuteReaderAsync(cmd, conn, (dr) => list.Add(mapper(dr)), CommandBehavior.SingleResult);
            return list;
        }

        public async Task<IEnumerable<T>> GetRowsAsync<T>(DbCommand cmd, DbConnection conn, Func<IDataReader, Task<T>> mapper)
        {
            List<T> list = new List<T>();
            await ExecuteReaderAsync(cmd, conn, async (dr) =>
            {
                list.Add(await mapper(dr));
            }, CommandBehavior.SingleResult);
            return list;
        }


        public async Task ExecuteReaderAsync(DbCommand command, DbConnection connection, Action<IDataReader> action, CommandBehavior behavior = CommandBehavior.CloseConnection)
        {
            await ExecuteAsync<Task>(command, connection, async (cmd, conn) =>
            {
                using (IDataReader dr = await cmd.ExecuteReaderAsync(behavior))
                {
                    while (dr.Read())
                    {
                        action(dr);
                    }
                }
                return Task.CompletedTask;
            });
        }

        public async Task<T> GetOneRow<T>(DbCommand command, DbConnection connection, Func<IDataReader, T> mapper)
        {
            return await ExecuteAsync<T>(command, connection, async (cmd, conn) =>
            {
                using (IDataReader dr = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow))
                {
                    return dr.Read() ? mapper(dr) : default(T);
                }
            });
        }

        public async Task<T> GetOneRowAsync<T>(DbCommand command, DbConnection connection, Func<IDataReader, Task<T>> mapper)
        {
            return await ExecuteAsync<T>(command, connection, async (cmd, conn) =>
            {
                using (IDataReader dr = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow))
                {
                    return dr.Read() ? await mapper(dr) : default(T);
                }
            });
        }

        public async Task<int> ExecuteNonQueryAsync(DbCommand command, DbConnection connection)
        {
            return await ExecuteAsync<int>(command, connection, async (cmd, conn) =>
            {
                return await cmd.ExecuteNonQueryAsync();
            });
        }

        public async Task<T> ExecuteAsync<T>(DbCommand cmd, DbConnection conn, Func<DbCommand, DbConnection, Task<T>> f)
        {
            try
            {
                cmd.Connection = conn;
                using (OpenConnection(conn))
                {
                    using (cmd)
                    {
                        return await f(cmd, conn);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                // Not necesary
                // CloseConnection(conn);
            }
        }
    }
}
