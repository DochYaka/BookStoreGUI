using Microsoft.Extensions.DependencyInjection;

namespace csharp_DBCourseWork_sem5.Services
{
    public interface INavigationService
    {
        void ShowLogin();
        void ShowMain();
        void ShowChangePassword();
    }

    public class WindowNavigationService : INavigationService
    {
        private readonly IServiceProvider _provider;

        public WindowNavigationService(IServiceProvider provider) => _provider = provider;

        public void ShowLogin() => _provider.GetRequiredService<LoginWindow>().Show();

        public void ShowMain() => _provider.GetRequiredService<MainWindow>().Show();

        public void ShowChangePassword() => _provider.GetRequiredService<ChangePasswordWindow>().ShowDialog();
    }
}