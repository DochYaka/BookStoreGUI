using System;
using System.Linq;
using System.Windows;

namespace csharp_DBCourseWork_sem5.Services
{
    public static class ThemeService
    {
        public static void ChangeTheme(bool isDarkTheme)
        {
            // Путь к новому словарю
            string themePath = isDarkTheme
                ? "Styles/DarkTheme.xaml"
                : "Styles/LightTheme.xaml";

            var newTheme = new ResourceDictionary
            {
                Source = new Uri(themePath, UriKind.Relative)
            };

            // Ищем словарь, в пути которого есть "Theme.xaml", чтобы удалить старый
            var oldTheme = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.ToString().Contains("Theme.xaml"));

            if (oldTheme != null)
            {
                // Находим индекс старой темы и заменяем её на новую
                int index = Application.Current.Resources.MergedDictionaries.IndexOf(oldTheme);
                Application.Current.Resources.MergedDictionaries[index] = newTheme;
            }
            else
            {
                // Если темы не было (например, только запустились), просто добавляем
                Application.Current.Resources.MergedDictionaries.Add(newTheme);
            }
        }
    }
}