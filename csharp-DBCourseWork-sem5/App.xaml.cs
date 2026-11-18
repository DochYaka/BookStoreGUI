using csharp_DBCourseWork_sem5.Models;
using csharp_DBCourseWork_sem5.Models.Interfaces;
using csharp_DBCourseWork_sem5.Models.Repositories;
using csharp_DBCourseWork_sem5.Pages;
using csharp_DBCourseWork_sem5.Services;
using csharp_DBCourseWork_sem5.Services.Tables;
using csharp_DBCourseWork_sem5.Services.Themes;
using csharp_DBCourseWork_sem5.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace csharp_DBCourseWork_sem5
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        private const string ConnectionString =
            "Host=localhost;Database=BookStore_db;Username=postgres;Password=your_secure_password";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ApplyTheme();

            try
            {
                var services = new ServiceCollection();
                ConfigureServices(services);

                Services = services.BuildServiceProvider(new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true
                });

                Services.GetRequiredService<INavigationService>().ShowLogin();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска приложения:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void ApplyTheme()
        {
            try
            {
                var settings = AppSettings.Load();
                string themePath = settings.IsDarkTheme ? "Styles/DarkTheme.xaml" : "Styles/LightTheme.xaml";

                var oldTheme = Resources.MergedDictionaries
                    .FirstOrDefault(d => d.Source != null && d.Source.ToString().Contains("Theme.xaml"));

                if (oldTheme != null)
                    Resources.MergedDictionaries.Remove(oldTheme);

                Resources.MergedDictionaries.Add(
                    new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки темы: {ex.Message}");
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // База данных: фабрика
            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseNpgsql(ConnectionString));

            // Репозитории
            services.AddSingleton<ITableCatalogRepository, TableCatalogRepository>();
            services.AddSingleton<ITableDataRepository, TableDataRepository>();

            // Сервисы
            services.AddSingleton<IRowValidator, RowValidator>();
            services.AddSingleton<ISearchService, SearchService>();
            services.AddSingleton<IExportService, ExcelExportService>();
            services.AddSingleton<IDialogService, WpfDialogService>();
            services.AddSingleton<IUserSession, UserSessionAdapter>();
            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddSingleton<INavigationService, WindowNavigationService>();
            services.AddSingleton<DatabaseWarmupService>();
            services.AddTransient<AuthService>();
            services.AddSingleton<IAnalyticsService, AnalyticsService>();
            services.AddSingleton<IPageService, PageService>();

            // ViewModel и окна
            services.AddTransient<LoginViewModel>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();
            services.AddTransient<UserManualPage>();
            services.AddTransient<AboutPage>();
            services.AddTransient<AnalyticsPage>();
            services.AddTransient<AnalyticsViewModel>();
            services.AddTransient<ChangePasswordWindow>();
        }
    }
}