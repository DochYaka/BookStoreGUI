using csharp_DBCourseWork_sem5.Models;
using csharp_DBCourseWork_sem5.Models.Interfaces;
using csharp_DBCourseWork_sem5.Services;
using csharp_DBCourseWork_sem5.Services.Users;
using Npgsql;
using System.Data.Common;

namespace csharp_DBCourseWork_sem5.Services
{
    public class AuthService
    {
        private readonly IUsersRepository _usersRepository;

        public AuthService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<LoginResult> TryLogin(string login, string password)
        {
            try
            {
                var user = await _usersRepository.GetUserByLoginAsync(login);
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
