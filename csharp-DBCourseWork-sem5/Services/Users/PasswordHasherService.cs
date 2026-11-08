using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Security.Cryptography;
using System.Text;

namespace csharp_DBCourseWork_sem5.Services
{
    public static class PasswordHasherService
    {
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);

                return Convert.ToHexString(hashBytes);
            }
        }
    }
}
