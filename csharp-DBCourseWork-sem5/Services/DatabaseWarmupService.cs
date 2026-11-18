using csharp_DBCourseWork_sem5.Models;
using Microsoft.EntityFrameworkCore;

namespace csharp_DBCourseWork_sem5.Services
{
    public class DatabaseWarmupService
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public DatabaseWarmupService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

        public async Task WarmupAsync()
        {
            try
            {
                await using var db = await _factory.CreateDbContextAsync();
                await db.Users.AnyAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DB warmup failed: {ex}");
            }
        }
    }
}