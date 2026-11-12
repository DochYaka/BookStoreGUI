using csharp_DBCourseWork_sem5.Services;
using csharp_DBCourseWork_sem5.Services.Themes;
using System.Windows;

namespace csharp_DBCourseWork_sem5
{
    public partial class SettingsWindow : Window
    {
        private AppSettings _settings;

        public SettingsWindow()
        {
            InitializeComponent();
            _settings = AppSettings.Load();

            chkDarkTheme.IsChecked = _settings.IsDarkTheme;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _settings.IsDarkTheme = chkDarkTheme.IsChecked == true;
            _settings.Save();

            ThemeService.ChangeTheme(_settings.IsDarkTheme);
            this.Close();
        }
    }
}