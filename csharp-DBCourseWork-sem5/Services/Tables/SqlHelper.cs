using Npgsql;
using NpgsqlTypes;
using System.Data.Common;

namespace csharp_DBCourseWork_sem5.Services
{
    internal static class SqlHelper
    {
        public static string Quote(string name) => "\"" + name.Replace("\"", "\"\"") + "\"";

        public static bool IsEmpty(object? value) =>
            value == null || value == DBNull.Value || (value is string s && string.IsNullOrWhiteSpace(s));

        public static object NormalizeValue(object? value) => IsEmpty(value) ? DBNull.Value : value!;

        public static void AddParam(DbCommand cmd, string name, object? value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }

        public static void AddValueParam(DbCommand cmd, string name, object? value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            if (p is NpgsqlParameter np && value is string)
                np.NpgsqlDbType = NpgsqlDbType.Unknown;
            cmd.Parameters.Add(p);
        }
    }
}