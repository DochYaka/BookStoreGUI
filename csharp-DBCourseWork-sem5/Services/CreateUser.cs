using csharp_DBCourseWork_sem5.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace csharp_DBCourseWork_sem5.Services
{
    public static class CreateUser
    {
        public static async void Create()
        {
            await using (BookStoreDbContext bookstore = new BookStoreDbContext())
            {
                string salt = PasswordHasherService.GenerateSalt();
                string hash = PasswordHasherService.HashPassword("MomYaka", salt);

                var newUser = new User()
                {
                    Login = "MomYaka",
                    PasswordHash = hash,
                    Salt = salt,
                    Role = "sales_manager_role"
                };

                bookstore.Users.Add(newUser);
                bookstore.SaveChanges();
            }
        }
    }
}
