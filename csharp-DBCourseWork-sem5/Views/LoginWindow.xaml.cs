using csharp_DBCourseWork_sem5.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using csharp_DBCourseWork_sem5.Services.Pages;

namespace csharp_DBCourseWork_sem5
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            if (DataContext is LoginViewModel vm)
            {
                vm.LoginSucceeded += OnLoginSucceeded;
                vm.LoginFailed += OnLoginFailed;
                vm.DbConnectFailed += OnDbConnectFailed;
                vm.UnknownError += OnUnknownError;
            }

            fmPage.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Открыть страницу
        /// </summary>
        private void OpenPage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem item) return;
            if (item.Header is not string name) return;
            
            fmPage.Visibility = Visibility.Visible;
            
            OpenPageService.OpenPage(name, fmPage);
        }


        private void OnLoginSucceeded(object? sender, System.EventArgs e)
        {
            new MainWindow().Show();
            Window.GetWindow(this)?.Close();
        }

        private void OnDbConnectFailed(object? sender, System.EventArgs e)
            => MessageBox.Show("База данных недоступна. Проверьте подключение к серверу и повторите попытку.");

        private void OnUnknownError(object? sender, System.EventArgs e)
            => MessageBox.Show("Неизвестная ошибка");

        private void OnLoginFailed(object? sender, System.EventArgs e)
            => MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = ((PasswordBox)sender).Password;
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var changeWindow = new ChangePasswordWindow();
            changeWindow.ShowDialog();
        }

        /// <summary>
        /// Смена цвета для Border
        /// </summary>
        private void brdLogin_MouseEnter(object sender, MouseEventArgs e)
        {
            brdLogin.Background = new SolidColorBrush(Color.FromArgb(0xB0, 0xFF, 0xFF, 0xFF));
        }

        /// <summary>
        /// Смена цвета для Border
        /// </summary>
        private void brdLogin_MouseLeave(object sender, MouseEventArgs e)
        {
            brdLogin.Background = new SolidColorBrush(Color.FromArgb(0x80, 0xFF, 0xFF, 0xFF));
        }

        /// <summary>
        /// Закрыть страницу/окно
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (fmPage.Content == null)
            {
                Application.Current.Shutdown();
                return;
            }

            if (fmPage.Content != null)
            {
                fmPage.Content = null;
                fmPage.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
    }
}
