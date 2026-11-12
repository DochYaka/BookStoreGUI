using csharp_DBCourseWork_sem5.ViewModels;
using System.Windows.Controls;

namespace csharp_DBCourseWork_sem5.Pages
{
    public partial class AnalyticsPage : Page
    {
        private bool _initialized;

        public AnalyticsPage()
        {
            InitializeComponent();

            var vm = new AnalyticsViewModel();
            DataContext = vm;

            Loaded += (_, _) =>
            {
                if (_initialized) return;
                _initialized = true;
                vm.InitializeCommand.Execute(null);
            };
        }
    }
}