using csharp_DBCourseWork_sem5.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Data;

namespace csharp_DBCourseWork_sem5.Services
{
    public static class AnalyticsService
    {
        private static readonly ConcurrentDictionary<string, (DateTime At, DataTable Table)> _cache = new();
        private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(5);

        public static void ClearCache() => _cache.Clear();

        public static Task<DataTable> QueryAsync(string sql, params (string Name, object Value)[] parameters)
        {
            string key = sql + "|" + string.Join(";", parameters.Select(p => $"{p.Name}={p.Value}"));

            if (_cache.TryGetValue(key, out var hit) && DateTime.UtcNow - hit.At < CacheLifetime)
                return Task.FromResult(hit.Table);

            return Task.Run(() =>
            {
                using var db = new BookStoreDbContext();
                db.Database.OpenConnection();

                using var command = db.Database.GetDbConnection().CreateCommand();
                command.CommandText = sql;
                command.CommandTimeout = 30;

                foreach (var (name, value) in parameters)
                {
                    var p = command.CreateParameter();
                    p.ParameterName = name;
                    p.Value = value ?? DBNull.Value;
                    command.Parameters.Add(p);
                }

                using var reader = command.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);

                _cache[key] = (DateTime.UtcNow, table);
                return table;
            });
        }
    }
}