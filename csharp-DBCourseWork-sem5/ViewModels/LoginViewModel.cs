using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using csharp_DBCourseWork_sem5.Services;

namespace csharp_DBCourseWork_sem5.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private AuthService _authService;
        private readonly DatabaseWarmupService _warmupService;

        public event EventHandler? LoginSucceeded;
        public event EventHandler? LoginFailed;

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
            if (_authService.TryLogin(UserName, Password))
                LoginSucceeded?.Invoke(this, EventArgs.Empty);
            else
                LoginFailed?.Invoke(this, EventArgs.Empty);
        }


    }
}
