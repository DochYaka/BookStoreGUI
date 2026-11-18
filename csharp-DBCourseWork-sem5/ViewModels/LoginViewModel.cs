using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using csharp_DBCourseWork_sem5.Services;
using csharp_DBCourseWork_sem5.Services.Users;

namespace csharp_DBCourseWork_sem5.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        public event EventHandler? LoginSucceeded;
        public event EventHandler? LoginFailed;
        public event EventHandler? DbConnectFailed;
        public event EventHandler? UnknownError;

        [ObservableProperty]
        private string _userName = "";

        [ObservableProperty]
        private string _password = "";

        public LoginViewModel(AuthService authService, DatabaseWarmupService warmupService)
        {
            _authService = authService;

            _ = warmupService.WarmupAsync();
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            LoginResult res;
            try
            {
                res = await _authService.TryLogin(UserName, Password);
            }
            catch
            {
                UnknownError?.Invoke(this, EventArgs.Empty);
                return;
            }

            switch (res)
            {
                case LoginResult.Success:
                    LoginSucceeded?.Invoke(this, EventArgs.Empty);
                    break;
                case LoginResult.InvalidCredentials:
                    LoginFailed?.Invoke(this, EventArgs.Empty);
                    break;
                case LoginResult.DatabaseUnavailable:
                    DbConnectFailed?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    UnknownError?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }
    }
}