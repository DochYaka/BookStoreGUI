using csharp_DBCourseWork_sem5.Models;
using csharp_DBCourseWork_sem5.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace csharp_DBCourseWork_sem5.Services
{
    public class TableDataRepository : ITableDataRepository
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public TableDataRepository(IDbContextFactory<AppDbContext> factory) => _factory = factory;

        public DataTable Load(string tableName, int limit = 1000)
        {
            using var db = _factory.CreateDbContext();
            db.Database.OpenConnection();

            using var cmd = db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = $"SELECT * FROM {SqlHelper.Quote(tableName)} LIMIT {limit}";

            using var reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);

            foreach (DataColumn col in dt.Columns)
                col.ReadOnly = false;

            return dt;
        }

        public DataTable ExecuteRawSql(string sql)
        {
            using var db = _factory.CreateDbContext();
            db.Database.OpenConnection();

            using var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;
            command.CommandTimeout = 30;

            using var reader = command.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        public int Save(string tableName, IReadOnlyList<string> primaryKey,
                        IReadOnlyCollection<DataRow> addedRows,
                        IReadOnlyCollection<DataRow> modifiedRows)
        {
            using var db = _factory.CreateDbContext();
            db.Database.OpenConnection();
            var conn = db.Database.GetDbConnection();

            using var tx = conn.BeginTransaction();

            int total = InsertRows(conn, tx, tableName, addedRows)
                      + UpdateRows(conn, tx, tableName, primaryKey, modifiedRows);

            tx.Commit();
            return total;
        }

        public int Delete(string tableName, IReadOnlyList<string> primaryKey, IReadOnlyCollection<DataRow> rows)
        {
            if (rows.Count == 0) return 0;

            using var db = _factory.CreateDbContext();
            db.Database.OpenConnection();
            var conn = db.Database.GetDbConnection();

            using var tx = conn.BeginTransaction();

            var (where, ps) = BuildRowsWhere(primaryKey, rows);

            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = $"DELETE FROM {SqlHelper.Quote(tableName)} WHERE {where}";
            foreach (var (name, value) in ps)
                SqlHelper.AddParam(cmd, name, value);

            int n = cmd.ExecuteNonQuery();
            tx.Commit();
            return n;
        }

        private static int InsertRows(DbConnection conn, DbTransaction tx, string table, IEnumerable<DataRow> rows)
        {
            int count = 0;

            foreach (var row in rows)
            {
                var columns = row.Table.Columns.Cast<DataColumn>()
                    .Where(c => !c.ColumnName.Equals("id", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                string names = string.Join(", ", columns.Select(c => SqlHelper.Quote(c.ColumnName)));
                string values = string.Join(", ", columns.Select((_, i) => $"@p{i}"));

                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = $"INSERT INTO {SqlHelper.Quote(table)} ({names}) VALUES ({values})";

                for (int i = 0; i < columns.Count; i++)
                    SqlHelper.AddValueParam(cmd, "p" + i, SqlHelper.NormalizeValue(row[columns[i]]));

                count += cmd.ExecuteNonQuery();
            }

            return count;
        }

        private static int UpdateRows(DbConnection conn, DbTransaction tx, string table,
                                      IReadOnlyList<string> pk, IEnumerable<DataRow> rows)
        {
            int count = 0;

            foreach (var row in rows)
            {
                var changed = row.Table.Columns.Cast<DataColumn>()
                    .Where(c => !pk.Contains(c.ColumnName) && IsChanged(row, c))
                    .ToList();
                if (changed.Count == 0) continue;

                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;

                var sets = changed.Select((c, i) => $"{SqlHelper.Quote(c.ColumnName)} = @v{i}");
                var wheres = pk.Select((c, i) => $"{SqlHelper.Quote(c)} = @k{i}");
                cmd.CommandText = $"UPDATE {SqlHelper.Quote(table)} SET {string.Join(", ", sets)} " +
                                  $"WHERE {string.Join(" AND ", wheres)}";

                for (int i = 0; i < changed.Count; i++)
                    SqlHelper.AddValueParam(cmd, "v" + i, SqlHelper.NormalizeValue(row[changed[i]]));

                for (int i = 0; i < pk.Count; i++)
                    SqlHelper.AddValueParam(cmd, "k" + i, row[pk[i], DataRowVersion.Original]);

                if (cmd.ExecuteNonQuery() != 1)
                    throw new InvalidOperationException(
                        "Не удалось обновить строку: возможно, она уже удалена или изменена другим пользователем.");

                count++;
            }

            return count;
        }

        private static bool IsChanged(DataRow row, DataColumn column)
        {
            var original = row[column, DataRowVersion.Original];
            var current = row[column, DataRowVersion.Current];

            if (SqlHelper.IsEmpty(original) && SqlHelper.IsEmpty(current)) return false;
            return !Equals(original, current);
        }

        private static (string Where, List<(string Name, object Value)> Params) BuildRowsWhere(
            IReadOnlyList<string> pk, IEnumerable<DataRow> rows)
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
                    conds.Add($"{SqlHelper.Quote(pk[i])} = @{name}");
                }
                parts.Add("(" + string.Join(" AND ", conds) + ")");
                r++;
            }

            return (string.Join(" OR ", parts), ps);
        }
    }
}