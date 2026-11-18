using csharp_DBCourseWork_sem5.Models.Entities;
using csharp_DBCourseWork_sem5.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace csharp_DBCourseWork_sem5.Models.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public UsersRepository(IDbContextFactory<AppDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task SaveUserAsync(User user)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.Entry(user).State = user.Id == default ? EntityState.Added : EntityState.Modified;
            await db.SaveChangesAsync();
        }
    }
}