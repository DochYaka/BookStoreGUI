using csharp_DBCourseWork_sem5.Pages;
using System.Windows.Controls;

namespace csharp_DBCourseWork_sem5.Services.Pages
{
    public static class PageFactory
    {
        public static Page Create(string name, Frame frame) => name switch
        {
            "Руководство пользователя" => new UserManualPage(),
            "О программе" => new AboutPage(),
            "Аналитика" => new AnalyticsPage(),
            _ => throw new ArgumentException($"Нет страницы «{name}»")
        };
    }
}
