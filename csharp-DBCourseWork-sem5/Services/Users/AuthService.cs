using csharp_DBCourseWork_sem5.Models;
using csharp_DBCourseWork_sem5.Services;
using csharp_DBCourseWork_sem5.Services.Users;
using Npgsql;
using System.Data.Common;

namespace csharp_DBCourseWork_sem5.Services
{
    public class AuthService
    {
        public LoginResult TryLogin(string login, string password)
        {
            try
            {
                using var db = new BookStoreDbContext();

                var user = db.Users.FirstOrDefault(u => u.Login == login);
                if (user == null) return LoginResult.InvalidCredentials;

                string pass = PasswordHasherService.HashPassword(password, user.Salt);

                if (pass == user.PasswordHash)
                {
                    UserSession._currentLogin = login;
                    UserSession._currentRole = user.Role;
                    UserSession._currentUserId = user.Id;

                    return LoginResult.Success;
             
                }
            }
            catch (NpgsqlException ex)
            {
                return LoginResult.DatabaseUnavailable;
            }
            catch (DbException ex)
            {
                return LoginResult.UnknownError;
            }
            return LoginResult.UnknownError;
        }
    }
}
