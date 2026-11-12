using csharp_DBCourseWork_sem5.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Data.Common;

namespace csharp_DBCourseWork_sem5.Services
{
    public static class TableEditService
    {
        public static HashSet<string> GetNotNullColumns(string table)
        {
            using var db = new BookStoreDbContext();
            db.Database.OpenConnection();

            using var cmd = db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = @"SELECT column_name FROM information_schema.columns
                                WHERE table_schema = 'public' AND table_name = @t
                                  AND is_nullable = 'NO'";
            AddParam(cmd, "t", table);

            var result = new HashSet<string>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(reader.GetString(0));
            return result;
        }

        public static List<string> GetPrimaryKey(string table)
        {
            using var db = new BookStoreDbContext();
            db.Database.OpenConnection();

            using var cmd = db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = @"SELECT a.attname::text
                                FROM pg_index i
                                JOIN pg_attribute a ON a.attrelid = i.indrelid AND a.attnum = ANY(i.indkey)
                                WHERE i.indisprimary
                                  AND i.indrelid = ('public.' || quote_ident(@t))::regclass";
            AddParam(cmd, "t", table);

            var result = new List<string>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(reader.GetString(0));
            return result;
        }

        public static int InsertRows(string tableName, List<DataRow> rows)
        {
            int count = 0;
            using var db = new BookStoreDbContext();
            db.Database.OpenConnection();

            var connection = (NpgsqlConnection)db.Database.GetDbConnection();

            foreach (var row in rows)
            {
                var columns = row.Table.Columns.Cast<DataColumn>()
                    .Where(c => !c.ColumnName.Equals("id", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                string columnNames = string.Join(", ", columns.Select(c => $"\"{c.ColumnName}\""));
                string parameters = string.Join(", ", columns.Select(c => $"@{c.ColumnName}"));

                string sql = $"INSERT INTO \"{tableName}\" ({columnNames}) VALUES ({parameters})";

                using var cmd = new NpgsqlCommand(sql, connection);

                foreach (var col in columns)
                {
                    object val = row[col] == DBNull.Value || row[col] == null ? DBNull.Value : row[col];
                    cmd.Parameters.AddWithValue($"@{col.ColumnName}", val);
                }

                count += cmd.ExecuteNonQuery();
            }

            return count;
        }

        public static int UpdateRows(string table, List<string> pk, IEnumerable<DataRow> rows)
        {
            using var db = new BookStoreDbContext();
            db.Database.OpenConnection();

            var conn = db.Database.GetDbConnection();
            using var tx = conn.BeginTransaction();
            int count = 0;

            foreach (var row in rows)
            {
                var changed = row.Table.Columns.Cast<DataColumn>()
                    .Where(c => !pk.Contains(c.ColumnName) && IsChanged(row, c))
                    .ToList();
                if (changed.Count == 0) continue;

                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;

                var sets = changed.Select((c, i) => $"{Quote(c.ColumnName)} = @v{i}");
                var wheres = pk.Select((c, i) => $"{Quote(c)} = @k{i}");
                cmd.CommandText = $"UPDATE {Quote(table)} SET {string.Join(", ", sets)} " +
                                  $"WHERE {string.Join(" AND ", wheres)}";

                for (int i = 0; i < changed.Count; i++)
                    AddValueParam(cmd, "v" + i, NormalizeValue(row[changed[i]]));

                for (int i = 0; i < pk.Count; i++)
                    AddValueParam(cmd, "k" + i, row[pk[i], DataRowVersion.Original]);

                int affected = cmd.ExecuteNonQuery();
                if (affected != 1)
                    throw new InvalidOperationException(
                        "Не удалось обновить строку: возможно, она уже удалена или изменена другим пользователем.");

                count++;
            }

            tx.Commit();
            return count;
        }

        private static bool IsChanged(DataRow row, DataColumn column)
        {
            var original = row[column, DataRowVersion.Original];
            var current = row[column, DataRowVersion.Current];

            if (IsEmpty(original) && IsEmpty(current)) return false;
            return !Equals(original, current);
        }

        private static object NormalizeValue(object? value) => IsEmpty(value) ? DBNull.Value : value!;

        public static int DeleteRows(string table, List<string> pk, IEnumerable<DataRow> rows)
        {
            using var db = new BookStoreDbContext();
            db.Database.OpenConnection();
            var conn = db.Database.GetDbConnection();

            using var tx = conn.BeginTransaction();

            var (where, ps) = BuildRowsWhere(pk, rows);
            int n = NonQuery(conn, tx, $"DELETE FROM {Quote(table)} WHERE {where}", ps);

            tx.Commit();
            return n;
        }

        private static (string Where, List<(string Name, object Value)> Params) BuildRowsWhere(
            List<string> pk, IEnumerable<DataRow> rows)
        {
            var ps = new List<(string, object)>();
            var parts = new List<string>();
            int r = 0;

            foreach (var row in rows)
            {
                var conds = new List<string>();
                for (int i = 0; i < pk.Count; i++)
                {
                    string name = $"k{r}_{i}";
                    ps.Add((name, row[pk[i], DataRowVersion.Original]));
                    conds.Add($"{Quote(pk[i])} = @{name}");
                }
                parts.Add("(" + string.Join(" AND ", conds) + ")");
                r++;
            }

            return (string.Join(" OR ", parts), ps);
        }

        public static bool IsEmpty(object? value) =>
            value == null || value == DBNull.Value || (value is string s && string.IsNullOrWhiteSpace(s));

        private static string Quote(string name) => "\"" + name.Replace("\"", "\"\"") + "\"";

        private static void AddParam(DbCommand cmd, string name, object? value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }

        private static void AddValueParam(DbCommand cmd, string name, object? value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            if (p is NpgsqlParameter np && value is string)
                np.NpgsqlDbType = NpgsqlDbType.Unknown;
            cmd.Parameters.Add(p);
        }

        private static int NonQuery(DbConnection conn, DbTransaction? tx, string sql, List<(string Name, object Value)> ps)
        {
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;
            foreach (var (name, value) in ps) AddParam(cmd, name, value);
            return cmd.ExecuteNonQuery();
        }

        public static Dictionary<string, object> GetDefaultValues(string tableName)
        {
            var defaults = new Dictionary<string, object>();

            using var db = new BookStoreDbContext();
            db.Database.OpenConnection();
            var conn = db.Database.GetDbConnection();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT column_name, column_default, data_type 
                FROM information_schema.columns 
                WHERE table_schema = 'public' AND table_name = @t";

            var p = cmd.CreateParameter();
            p.ParameterName = "t";
            p.Value = tableName;
            cmd.Parameters.Add(p);

            var columnsWithDefaults = new List<(string Name, string DefaultExpr, string DataType)>();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = reader.GetString(0);
                    string defaultExpr = reader.IsDBNull(1) ? null : reader.GetString(1);
                    string dataType = reader.GetString(2);

                    if (!string.IsNullOrEmpty(defaultExpr))
                        columnsWithDefaults.Add((name, defaultExpr, dataType));
                }
            }

            if (columnsWithDefaults.Count > 0)
            {
                string selectList = string.Join(", ", columnsWithDefaults.Select(c => $"{c.DefaultExpr} AS \"{c.Name}\""));
                using var cmdDefaults = conn.CreateCommand();
                cmdDefaults.CommandText = $"SELECT {selectList}";

                using var reader = cmdDefaults.ExecuteReader();
                if (reader.Read())
                {
                    for (int i = 0; i < columnsWithDefaults.Count; i++)
                    {
                        if (!reader.IsDBNull(i))
                            defaults[columnsWithDefaults[i].Name] = reader.GetValue(i);
                    }
                }
            }

            return defaults;
        }
    }
}