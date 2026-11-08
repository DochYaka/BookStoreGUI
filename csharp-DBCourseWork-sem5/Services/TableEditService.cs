using csharp_DBCourseWork_sem5.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace csharp_DBCourseWork_sem5.Services
{
    public static class TableEditService
    {
        public static HashSet<string> GetRequiredColumns(string table)
        {
            using var db = new BookStoreDbContext();
            db.Database.OpenConnection();

            using var cmd = db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = @"SELECT column_name FROM information_schema.columns
                                WHERE table_schema = 'public' AND table_name = @t
                                  AND is_nullable = 'NO'
                                  AND column_default IS NULL
                                  AND is_identity = 'NO'";
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

        public static int InsertRows(string table, IEnumerable<DataRow> rows)
        {
            using var db = new BookStoreDbContext();
            db.Database.OpenConnection();

            var conn = db.Database.GetDbConnection();
            using var tx = conn.BeginTransaction();
            int count = 0;

            foreach (var row in rows)
            {
                var cols = row.Table.Columns.Cast<DataColumn>()
                    .Where(c => !IsEmpty(row[c])).ToList();
                if (cols.Count == 0) continue;

                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText =
                    $"INSERT INTO {Quote(table)} ({string.Join(", ", cols.Select(c => Quote(c.ColumnName)))}) " +
                    $"VALUES ({string.Join(", ", cols.Select((_, i) => "@p" + i))})";

                for (int i = 0; i < cols.Count; i++)
                    AddParam(cmd, "p" + i, row[cols[i]]);

                cmd.ExecuteNonQuery();
                count++;
            }

            tx.Commit();
            return count;
        }

        public static Dictionary<string, int> PreviewDelete(string table, List<string> pk, IEnumerable<DataRow> rows)
        {
            using var db = new BookStoreDbContext();
            db.Database.OpenConnection();
            var conn = db.Database.GetDbConnection();

            var (where, ps) = BuildRowsWhere(pk, rows);
            var counts = new Dictionary<string, int>();
            CollectCounts(conn, null, table, where, ps, counts, new HashSet<string> { table });
            return counts;
        }

        public static Dictionary<string, int> DeleteRows(string table, List<string> pk, IEnumerable<DataRow> rows)
        {
            using var db = new BookStoreDbContext();
            db.Database.OpenConnection();
            var conn = db.Database.GetDbConnection();

            using var tx = conn.BeginTransaction();

            var (where, ps) = BuildRowsWhere(pk, rows);
            var deleted = new Dictionary<string, int>();
            DeleteCascade(conn, tx, table, where, ps, deleted, new HashSet<string> { table });

            tx.Commit();

            return deleted.OrderByDescending(kv => kv.Key == table).ThenBy(kv => kv.Key)
                          .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private sealed record FkInfo(string ChildTable, string[] ChildCols, string[] ParentCols);

        private static List<FkInfo> GetReferencingFks(DbConnection conn, DbTransaction? tx, string table)
        {
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"
                SELECT cl.relname::text,
                       (SELECT array_agg(a.attname::text ORDER BY k.ord)
                          FROM unnest(con.conkey) WITH ORDINALITY AS k(attnum, ord)
                          JOIN pg_attribute a ON a.attrelid = con.conrelid AND a.attnum = k.attnum),
                       (SELECT array_agg(a.attname::text ORDER BY k.ord)
                          FROM unnest(con.confkey) WITH ORDINALITY AS k(attnum, ord)
                          JOIN pg_attribute a ON a.attrelid = con.confrelid AND a.attnum = k.attnum)
                FROM pg_constraint con
                JOIN pg_class cl ON cl.oid = con.conrelid
                JOIN pg_class pr ON pr.oid = con.confrelid
                JOIN pg_namespace n ON n.oid = pr.relnamespace
                WHERE con.contype = 'f' AND n.nspname = 'public' AND pr.relname = @t";
            AddParam(cmd, "t", table);

            var result = new List<FkInfo>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new FkInfo(
                    reader.GetString(0),
                    reader.GetFieldValue<string[]>(1),
                    reader.GetFieldValue<string[]>(2)));
            }
            return result;
        }

        private static void CollectCounts(DbConnection conn, DbTransaction? tx, string table, string where,
            List<(string Name, object Value)> ps, Dictionary<string, int> counts, HashSet<string> path)
        {
            foreach (var fk in GetReferencingFks(conn, tx, table))
            {
                if (path.Contains(fk.ChildTable)) continue;

                var childWhere = BuildChildWhere(fk, table, where);
                int n = Convert.ToInt32(Scalar(conn, tx, $"SELECT COUNT(*) FROM {Quote(fk.ChildTable)} WHERE {childWhere}", ps));
                if (n == 0) continue;

                counts[fk.ChildTable] = counts.GetValueOrDefault(fk.ChildTable) + n;

                path.Add(fk.ChildTable);
                CollectCounts(conn, tx, fk.ChildTable, childWhere, ps, counts, path);
                path.Remove(fk.ChildTable);
            }
        }

        private static void DeleteCascade(DbConnection conn, DbTransaction tx, string table, string where,
            List<(string Name, object Value)> ps, Dictionary<string, int> deleted, HashSet<string> path)
        {
            foreach (var fk in GetReferencingFks(conn, tx, table))
            {
                if (path.Contains(fk.ChildTable)) continue;

                var childWhere = BuildChildWhere(fk, table, where);

                path.Add(fk.ChildTable);
                DeleteCascade(conn, tx, fk.ChildTable, childWhere, ps, deleted, path);
                path.Remove(fk.ChildTable);
            }

            int n = NonQuery(conn, tx, $"DELETE FROM {Quote(table)} WHERE {where}", ps);
            deleted[table] = deleted.GetValueOrDefault(table) + n;
        }

        private static string BuildChildWhere(FkInfo fk, string parentTable, string parentWhere)
        {
            var childCols = string.Join(", ", fk.ChildCols.Select(Quote));
            var parentCols = string.Join(", ", fk.ParentCols.Select(Quote));
            return $"({childCols}) IN (SELECT {parentCols} FROM {Quote(parentTable)} WHERE {parentWhere})";
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
                    ps.Add((name, row[pk[i]]));
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

        private static object? Scalar(DbConnection conn, DbTransaction? tx, string sql, List<(string Name, object Value)> ps)
        {
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;
            foreach (var (name, value) in ps) AddParam(cmd, name, value);
            return cmd.ExecuteScalar();
        }

        private static int NonQuery(DbConnection conn, DbTransaction? tx, string sql, List<(string Name, object Value)> ps)
        {
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;
            foreach (var (name, value) in ps) AddParam(cmd, name, value);
            return cmd.ExecuteNonQuery();
        }
    }
}