using csharp_DBCourseWork_sem5.Models;
using csharp_DBCourseWork_sem5.Services.Pages;
using csharp_DBCourseWork_sem5.Services.Tables;
using csharp_DBCourseWork_sem5.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

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

                    if (e.PropertyName == nameof(MainViewModel.Messages))
                        tcOutput.SelectedIndex = vm.IsError ? 1 : 0;
                };
            }

            Closing += (s, e) => new LoginWindow().Show();
        }

        private void OnLoadDataFailed(object? sender, EventArgs e)
            => MessageBox.Show($"Ошибка вывода таблиц", "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);

        private void OnConnectionFailed(object? sender, EventArgs e)
            => MessageBox.Show("Нет соединения с БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel vm && e.NewValue is TableInfo selectedTable)
                vm.SelectedTable = selectedTable;
        }

        private void rtbRawSql_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                TextRange textRange = new TextRange(rtbRawSql.Document.ContentStart, rtbRawSql.Document.ContentEnd);
                vm.RawSql = textRange.Text.Trim();
            }
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
                vm.SelectedRows = dgData.SelectedItems.OfType<System.Data.DataRowView>().ToList();
        }

        private void dgData_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "RowError" || e.PropertyName == "HasErrors")
            {
                e.Cancel = true;
                return;
            }
            
            e.Column.Header = ColumnTranslator.GetDisplayName(e.PropertyName);

            e.Column.MinWidth = 20;
            e.Column.Width = DataGridLength.Auto;
            e.Column.IsReadOnly = false;
        }

        private void dgData_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(
                $"BeginningEdit: {e.Row.Item?.GetType().Name}, " +
                $"state: {(e.Row.Item as System.Data.DataRowView)?.Row.RowState}");

            if (e.Row.Item is System.Data.DataRowView rv && rv.Row.RowState is System.Data.DataRowState.Unchanged or System.Data.DataRowState.Modified)
                e.Cancel = true;
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

        private void OpenPage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement el) return;
            if (el.Tag is not string name || string.IsNullOrWhiteSpace(name)) return;

            fmPage.Visibility = Visibility.Visible;
            OpenPageService.OpenPage(name, fmPage);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (fmPage.Content != null)
            {
                fmPage.Content = null;
                fmPage.Visibility = Visibility.Collapsed;
            }
            else Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            dgData.CommitEdit(DataGridEditingUnit.Cell, true);
            dgData.CommitEdit(DataGridEditingUnit.Row, true);
        }
    
        private void InputRequest_Click(object sender, RoutedEventArgs e)
        {
            string queries =
                "-- 1. Книги дороже 500 рублей\n" +
                "SELECT name, year, sale_price FROM copy c\r\nJOIN book b ON c.book_id = b.id\r\n WHERE sale_price > 500 ORDER BY sale_price DESC;\n\n" +
                "-- 2. Количество книг у каждого автора\n" +
                "SELECT a.last_name, a.first_name, COUNT(ab.book_id) \r\nFROM author a \r\nLEFT JOIN author_book ab ON a.id = ab.author_id \r\nGROUP BY a.id, a.last_name, a.first_name \r\nORDER BY COUNT(ab.book_id) DESC;\r\n\n\n" +
                "-- 3. Сумма продаж по клиентам\n" +
                "SELECT p.name, COUNT(pb.book_id) \r\nFROM publisher  p\r\nLEFT JOIN publisher_book pb ON p.id = pb.publishing_house_id \r\nGROUP BY p.id, p.name \r\nORDER BY COUNT(pb.book_id) DESC;\r\n";

            rtbRawSql.Document = new FlowDocument(new Paragraph(new Run(queries)));
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
    }
}