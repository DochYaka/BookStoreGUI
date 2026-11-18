using csharp_DBCourseWork_sem5.Models.Interfaces;
using System.Data;
using System.Windows;

namespace csharp_DBCourseWork_sem5.Services
{
    public class UserSessionAdapter : IUserSession
    {
        public string CurrentRole => UserSession._currentRole ?? string.Empty;
    }

    public class WpfDialogService : IDialogService
    {
        public bool Confirm(string text, string title) =>
            MessageBox.Show(text, title, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No)
                == MessageBoxResult.Yes;

        public void Info(string text, string title = "Успех") =>
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Information);

        public void Warning(string text, string title = "Внимание") =>
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Warning);

        public void Error(string text, string title = "Ошибка") =>
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public class ExcelExportService : IExportService
    {
        public void Export(DataView data, string prefix) => ExportToExcel.Export(data, prefix);
    }
}