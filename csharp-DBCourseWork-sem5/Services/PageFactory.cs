using System.Windows.Controls;

namespace csharp_DBCourseWork_sem5.Services
{
    public static class PageFactory
    {
        public static Page Create(string name, Frame frame) => name switch
        {
            "Руководство пользователя" => new UserManualPage(),
            "О программе" => new AboutPage(),
            _ => throw new ArgumentException($"Нет страницы «{name}»")
        };
    }
}
