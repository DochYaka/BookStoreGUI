using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using csharp_DBCourseWork_sem5.Services;
using csharp_DBCourseWork_sem5.Services.Users;

namespace csharp_DBCourseWork_sem5.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private AuthService _authService;
        private readonly DatabaseWarmupService _warmupService;

        public event EventHandler? LoginSucceeded;
        public event EventHandler? LoginFailed;
        public event EventHandler? DbConnectFailed;
        public event EventHandler? UnknownError;

        [ObservableProperty]
        private string _userName = "";

        [ObservableProperty]
        private string _password = "";

        public LoginViewModel()
        {
            _authService = new AuthService();
            _warmupService = new DatabaseWarmupService();
            _ = _warmupService.WarmupAsync();
        }

        [RelayCommand]
        private void Login()
        {
            var res = _authService.TryLogin(UserName, Password);

            switch (res)
            {
                case LoginResult.Success:
                    LoginSucceeded.Invoke(this, EventArgs.Empty);
                    break;
                case LoginResult.InvalidCredentials:
                    LoginFailed.Invoke(this, EventArgs.Empty);
                    break;
                case LoginResult.DatabaseUnavailable:
                    DbConnectFailed.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    UnknownError.Invoke(this, EventArgs.Empty);
                    break;
            }
        }


    }
}
