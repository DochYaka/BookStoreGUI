using csharp_DBCourseWork_sem5.Services.Themes;
using System;
using System.Windows;

namespace csharp_DBCourseWork_sem5
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var settings = AppSettings.Load();
            string themePath = settings.IsDarkTheme ? "Styles/DarkTheme.xaml" : "Styles/LightTheme.xaml";
            try
            {
                var oldTheme = Resources.MergedDictionaries
                    .FirstOrDefault(d => d.Source != null && d.Source.ToString().Contains("Theme.xaml"));

                if (oldTheme != null)
                    Resources.MergedDictionaries.Remove(oldTheme);

                var newTheme = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };
                Resources.MergedDictionaries.Add(newTheme);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки темы: {ex.Message}");
            }
        }
    }
}