using csharp_DBCourseWork_sem5.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Data;

namespace csharp_DBCourseWork_sem5.Services
{
    public interface IAnalyticsService
    {
        Task<DataTable> QueryAsync(string sql, params (string Name, object Value)[] parameters);
        void ClearCache();
    }

    public class AnalyticsService : IAnalyticsService
    {
        private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(5);

        private readonly IDbContextFactory<AppDbContext> _factory;

        // Кэш теперь принадлежит экземпляру (сервис регистрируется как singleton)
        private readonly ConcurrentDictionary<string, (DateTime At, DataTable Table)> _cache = new();

        public AnalyticsService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

        public void ClearCache() => _cache.Clear();

        public Task<DataTable> QueryAsync(string sql, params (string Name, object Value)[] parameters)
        {
            string key = sql + "|" + string.Join(";", parameters.Select(p => $"{p.Name}={p.Value}"));

            if (_cache.TryGetValue(key, out var hit) && DateTime.UtcNow - hit.At < CacheLifetime)
                return Task.FromResult(hit.Table);

            return Task.Run(() =>
            {
                using var db = _factory.CreateDbContext();
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