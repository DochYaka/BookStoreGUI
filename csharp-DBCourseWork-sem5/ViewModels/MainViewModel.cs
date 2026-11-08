    using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using csharp_DBCourseWork_sem5.Models;
using csharp_DBCourseWork_sem5.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace csharp_DBCourseWork_sem5.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public event EventHandler? LoadDataFailed;
        public event EventHandler? ConnectionFailed;

        public ObservableCollection<TableInfo> Tables { get; } = new();

        [ObservableProperty]
        private DataView? _tableData;

        [ObservableProperty]
        private TableInfo? _selectedTable;

        [ObservableProperty]
        private string _rawSql;

        [ObservableProperty]
        private string _searchInput;

        [ObservableProperty]
        private DataRow[]? _highlightedRows;

        public MainViewModel()
        {
            LoadTables();
        }

        private void LoadTables()
        {
            try
            {
                using (BookStoreDbContext db = new BookStoreDbContext())
                {
                    var role = UserSession._currentRole;

                    var allowedTables = db.Rolepermissions.Where(r => r.Role == role).Select(r => r.TableName).ToList();

                    var sql = @"SELECT c.relname AS table_name, COALESCE(d.description, c.relname) AS display_name
                        FROM pg_class c
                        JOIN pg_namespace n ON n.oid = c.relnamespace
                        LEFT JOIN pg_description d ON d.objoid = c.oid AND d.objsubid = 0
                        WHERE c.relkind = 'r' AND n.nspname = 'public'
                        ORDER BY display_name";

                    using (var command = db.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = sql;
                        db.Database.OpenConnection();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tableName = reader.GetString(0);
                                string displayName = reader.GetString(1);

                                if (allowedTables.Contains(tableName))
                                {
                                    var table = new TableInfo()
                                    {
                                        Name = tableName,
                                        DisplayName = displayName,
                                    };

                                    Tables.Add(table);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoadDataFailed?.Invoke($"Не удалось загрузить список таблиц:\n{ex.Message}", EventArgs.Empty);
            }
        }

        partial void OnSelectedTableChanged(TableInfo? value)
        {
            if (value == null) return;

            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                using var db = new BookStoreDbContext();
                string sql = $"SELECT * FROM \"{value.Name}\" LIMIT 1000";

                db.Database.OpenConnection();
                long tOpen = sw.ElapsedMilliseconds;

                using var command = db.Database.GetDbConnection().CreateCommand();
                command.CommandText = sql;

                using var reader = command.ExecuteReader();
                long tQuery = sw.ElapsedMilliseconds;

                DataTable dt = new DataTable();
                dt.Load(reader);
                long tLoad = sw.ElapsedMilliseconds;

                TableData = dt.DefaultView;

                Application.Current.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.ContextIdle,
                    new Action(() =>
                    {
                        long tRender = sw.ElapsedMilliseconds;
                        System.Diagnostics.Debug.WriteLine(
                            $"[{value.Name}] подключение: {tOpen} мс | запрос: {tQuery - tOpen} мс | " +
                            $"загрузка: {tLoad - tQuery} мс | отрисовка: {tRender - tLoad} мс | строк: {dt.Rows.Count}");
                    }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ConnectionFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        [RelayCommand]
        private void Run()
        {
            try
            {
                TableData = ParseRawSqService.SqlQuerry(RawSql).DefaultView;
            }
            catch (Exception ex)
            {
                ConnectionFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        [RelayCommand]
        private void Export()
        {
            try
            {
                if (TableData == null)
                {
                    MessageBox.Show("Сначала выберите таблицу!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string prefix = SelectedTable?.Name ?? "Данные";
                ExportToExcel.Export(TableData, prefix);

                MessageBox.Show("Файл успешно сохранен в папку Загрузки!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Search()
        {
            try
            {
                if (TableData == null)
                {
                    MessageBox.Show("Сначала выберите таблицу!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var searcher = new SearchService();
                HighlightedRows = searcher.Search(TableData, SearchInput);
            }
            catch
            {
                HighlightedRows = null;
            }
        }
    }
}
