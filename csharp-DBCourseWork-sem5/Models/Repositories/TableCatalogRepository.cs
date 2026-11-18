using csharp_DBCourseWork_sem5.Models;
using csharp_DBCourseWork_sem5.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace csharp_DBCourseWork_sem5.Services
{
    public class TableCatalogRepository : ITableCatalogRepository
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public TableCatalogRepository(IDbContextFactory<AppDbContext> factory) => _factory = factory;

        public IReadOnlyList<TableInfo> GetAllowedTables(string role)
        {
            using var db = _factory.CreateDbContext();

            var allowed = db.Rolepermissions
                .Where(r => r.Role == role)
                .Select(r => r.TableName)
                .ToHashSet();

            db.Database.OpenConnection();
            using var cmd = db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = @"SELECT c.relname AS table_name, COALESCE(d.description, c.relname) AS display_name
                                FROM pg_class c
                                JOIN pg_namespace n ON n.oid = c.relnamespace
                                LEFT JOIN pg_description d ON d.objoid = c.oid AND d.objsubid = 0
                                WHERE c.relkind IN ('r', 'v') AND n.nspname = 'public'
                                ORDER BY display_name";

            var result = new List<TableInfo>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string name = reader.GetString(0);
                if (allowed.Contains(name))
                    result.Add(new TableInfo { Name = name, DisplayName = reader.GetString(1) });
            }
            return result;
        }

        public HashSet<string> GetNotNullColumns(string tableName)
        {
            using var db = _factory.CreateDbContext();
            db.Database.OpenConnection();

            using var cmd = db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = @"SELECT column_name FROM information_schema.columns
                                WHERE table_schema = 'public' AND table_name = @t
                                  AND is_nullable = 'NO'";
            SqlHelper.AddParam(cmd, "t", tableName);

            var result = new HashSet<string>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(reader.GetString(0));
            return result;
        }

        public List<string> GetPrimaryKey(string tableName)
        {
            using var db = _factory.CreateDbContext();
            db.Database.OpenConnection();

            using var cmd = db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = @"SELECT a.attname::text
                                FROM pg_index i
                                JOIN pg_attribute a ON a.attrelid = i.indrelid AND a.attnum = ANY(i.indkey)
                                WHERE i.indisprimary
                                  AND i.indrelid = ('public.' || quote_ident(@t))::regclass";
            SqlHelper.AddParam(cmd, "t", tableName);

            var result = new List<string>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(reader.GetString(0));
            return result;
        }

        public Dictionary<string, object> GetDefaultValues(string tableName)
        {
            var defaults = new Dictionary<string, object>();

            using var db = _factory.CreateDbContext();
            db.Database.OpenConnection();
            var conn = db.Database.GetDbConnection();

            var columnsWithDefaults = new List<(string Name, string DefaultExpr)>();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT column_name, column_default
                                    FROM information_schema.columns
                                    WHERE table_schema = 'public' AND table_name = @t";
                SqlHelper.AddParam(cmd, "t", tableName);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.IsDBNull(1)) continue;

                    string expr = reader.GetString(1);

                    // nextval(...) нельзя вычислять «для показа»: каждый вызов расходует значение последовательности
                    if (string.IsNullOrEmpty(expr) || expr.Contains("nextval(", StringComparison.OrdinalIgnoreCase))
                        continue;

                    columnsWithDefaults.Add((reader.GetString(0), expr));
                }
            }

            if (columnsWithDefaults.Count == 0) return defaults;

            string selectList = string.Join(", ",
                columnsWithDefaults.Select(c => $"{c.DefaultExpr} AS {SqlHelper.Quote(c.Name)}"));

            using var cmdDefaults = conn.CreateCommand();
            cmdDefaults.CommandText = $"SELECT {selectList}";

            using var r = cmdDefaults.ExecuteReader();
            if (r.Read())
            {
                for (int i = 0; i < columnsWithDefaults.Count; i++)
                {
                    if (!r.IsDBNull(i))
                        defaults[columnsWithDefaults[i].Name] = r.GetValue(i);
                }
            }

            return defaults;
        }
    }
}