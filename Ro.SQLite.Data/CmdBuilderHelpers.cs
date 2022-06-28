using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Data.SQLite;

namespace Ro.SQLite.Data
{
    public static class CmdBuilderHelpers
    {
        public static DbCommand ToCmd(this string sql, params IDbDataParameter[] commandParameters)
        {
            DbCommand cmd = new SQLiteCommand(sql);
            cmd.AddParams(commandParameters);
            return cmd;
        }
        public static DbCommand ToCmd(this string sql, CommandType type, params IDbDataParameter[] commandParameters)
        {
            var cmd = ToCmd(sql, commandParameters);
            cmd.CommandType = type;
            return cmd;
        }
        public static DbCommand ToCmd(this string sql, DbType type, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            DbCommand cmd = ToCmd(sql);
            var name = sql.Split(' ').FirstOrDefault(param => param.StartsWith("@"));
            cmd.AddParams(name.Trim(), type, value, direction);
            return cmd;
        }

        public static IDbDataParameter ToParam(this string name, DbType type, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            IDbDataParameter param = new SQLiteParameter(name, value)
            {
                DbType = type,
                Direction = direction
            };

            return param;
        }

        public static void Add(this Dictionary<string, IDbDataParameter> d, string name, DbType type, object value)
        {
            d.Add(name, name.ToParam(type, value));
        }

        public static IDbCommand AddParams(this IDbCommand cmd, params IDbDataParameter[] commandParameters)
        {
            foreach (var p in commandParameters)
            {
                cmd.Parameters.Add(p);
            }
            return cmd;
        }

        public static IDbCommand AddParams(this IDbCommand cmd, string name, DbType type, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            cmd.AddParams(name.ToParam(type, value, direction));
            return cmd;
        }
    }
}
