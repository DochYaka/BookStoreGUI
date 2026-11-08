using csharp_DBCourseWork_sem5.Models;
using Microsoft.EntityFrameworkCore;

namespace csharp_DBCourseWork_sem5.Services
{
    public class DatabaseWarmupService
    {
        public async Task WarmupAsync()
        {
            try
            {
                await using var db = new BookStoreDbContext();
                await db.Users.AnyAsync();
            }
            catch (Exception ex)
            { 
                System.Diagnostics.Debug.WriteLine($"DB warmup failed: {ex}");
            }
        }
    }
}