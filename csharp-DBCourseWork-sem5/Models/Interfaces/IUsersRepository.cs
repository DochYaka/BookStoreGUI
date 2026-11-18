using csharp_DBCourseWork_sem5.Models.Entities;

namespace csharp_DBCourseWork_sem5.Models.Interfaces
{
    public interface IUsersRepository
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByLoginAsync(string login);
        Task SaveUserAsync(User user);
    }
}
