using csharp_DBCourseWork_sem5.Models;
using csharp_DBCourseWork_sem5.Services;
using System;
using System.Windows;

namespace csharp_DBCourseWork_sem5
{
    public partial class ChangePasswordWindow : Window
    {
        public ChangePasswordWindow()
        {
            InitializeComponent();
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string oldPass = txtOldPassword.Password;
            string newPass = txtNewPassword.Password;
            string confirmPass = txtConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(oldPass) || string.IsNullOrWhiteSpace(newPass))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Новый пароль и подтверждение не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var db = new BookStoreDbContext();

                var user = db.Users.FirstOrDefault(u => u.Login == login);
                if (user == null)
                {
                    MessageBox.Show("Пользователь с таким логином не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string oldHash = PasswordHasherService.HashPassword(oldPass, user.Salt);
                if (oldHash != user.PasswordHash)
                {
                    MessageBox.Show("Старый пароль введен неверно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string newSalt = PasswordHasherService.GenerateSalt();
                string newHash = PasswordHasherService.HashPassword(newPass, newSalt);

                user.Salt = newSalt;
                user.PasswordHash = newHash;
                db.SaveChanges();

                MessageBox.Show("Пароль успешно изменен! Теперь вы можете войти с новым паролем.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}