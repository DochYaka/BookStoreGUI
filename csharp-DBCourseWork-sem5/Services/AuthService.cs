using csharp_DBCourseWork_sem5.Models;

namespace csharp_DBCourseWork_sem5.Services
{
    public class AuthService
    {
        public bool TryLogin(string login, string password)
        {
            using var db = new BookStoreDbContext();

            var user = db.Users.FirstOrDefault(u => u.Login == login);
            if (user == null) return false;

            string pass = PasswordHasherService.HashPassword(password, user.Salt);

            if (pass == user.PasswordHash)
            {
                UserSession._currentLogin = login;
                UserSession._currentRole = user.Role;
                UserSession._currentUserId = user.Id;

                return true;
            }
            return false;
        }
    }
}
