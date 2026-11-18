using csharp_DBCourseWork_sem5.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace csharp_DBCourseWork_sem5.Services
{
    public interface IPageService
    {
        void Open(string name, Frame frame);
    }

    public class PageService : IPageService
    {
        private static readonly Dictionary<string, Type> PageTypes = new()
        {
            ["Руководство пользователя"] = typeof(UserManualPage),
            ["О программе"] = typeof(AboutPage),
            ["Аналитика"] = typeof(AnalyticsPage),
        };

        private readonly IServiceProvider _provider;

        public PageService(IServiceProvider provider) => _provider = provider;

        public void Open(string name, Frame frame)
        {
            if (!PageTypes.TryGetValue(name, out var type))
                throw new ArgumentException($"Нет страницы «{name}»");

            if (frame.Content?.GetType() == type) return;

            frame.Navigate(_provider.GetRequiredService(type));
        }
    }
}