using csharp_DBCourseWork_sem5.Models;
using csharp_DBCourseWork_sem5.Services;
using csharp_DBCourseWork_sem5.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace csharp_DBCourseWork_sem5
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (DataContext is MainViewModel vm)
            {
                vm.LoadDataFailed += OnLoadDataFailed;
                vm.ConnectionFailed += OnConnectionFailed;

                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(MainViewModel.HighlightedRows))
                        dgData.Items.Refresh();
                };
            }

            Closing += (s, e) => new LoginWindow().Show();
        }

        private void dgData_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (DataContext is not MainViewModel vm) return;

            if (vm.HighlightedRows == null || vm.HighlightedRows.Length == 0)
            {
                e.Row.Background = System.Windows.Media.Brushes.Transparent;
                return;
            }

            if (e.Row.Item is System.Data.DataRowView rowView)
            {
                if (vm.HighlightedRows.Contains(rowView.Row))
                    e.Row.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom("#50FFFFFF")!;
                else
                    e.Row.Background = System.Windows.Media.Brushes.Transparent;
            }
        }

        private void OnLoadDataFailed(object? sender, System.EventArgs e)
        {
            MessageBox.Show($"Ошибка вывода таблиц", "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnConnectionFailed(object? sender, System.EventArgs e)
        {
            MessageBox.Show("Нет соединения с БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel vm && e.NewValue is TableInfo selectedTable)
                vm.SelectedTable = selectedTable;
        }

        private void dgData_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "RowError" || e.PropertyName == "HasErrors")
            {
                e.Cancel = true;
                return;
            }

            e.Column.MinWidth = 20;
            e.Column.Width = DataGridLength.Auto;
        }

        private void rtbRawSql_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                TextRange textRange = new TextRange(rtbRawSql.Document.ContentStart, rtbRawSql.Document.ContentEnd);
                vm.RawSql = textRange.Text.Trim();
            }
        }

        /// <summary>
        /// Открыть страницу
        /// </summary>
        private void OpenPage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem item) return;
            if (item.Header is not string name) return;

            fmPage.Visibility = Visibility.Visible;

            OpenPageService.OpenPage(name, fmPage);
        }

        /// <summary>
        /// Закрыть окно
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (fmPage.Content != null)
            {
                fmPage.Content = null;
                fmPage.Visibility = Visibility.Collapsed;
            }
            else Close();
        }
    }
}