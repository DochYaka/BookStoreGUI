using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using csharp_DBCourseWork_sem5.Models;
using csharp_DBCourseWork_sem5.Services;
using csharp_DBCourseWork_sem5.Services.Tables;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Documents;

namespace csharp_DBCourseWork_sem5.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public event EventHandler? LoadDataFailed;
        public event EventHandler? ConnectionFailed;

        public ObservableCollection<TableInfo> ReferenceTables { get; } = new();
        public ObservableCollection<TableInfo> OtherTables { get; } = new();

        [ObservableProperty] private DataView? _tableData;

        [ObservableProperty] private TableInfo? _selectedTable;

        [ObservableProperty] private string _rawSql = string.Empty;

        [ObservableProperty] private string _searchInput = string.Empty;

        [ObservableProperty] private DataRow[]? _highlightedRows;

        [ObservableProperty] private TableInfo[] _highlightedTables;

        [ObservableProperty] private IList<DataRowView> _selectedRows = new List<DataRowView>();

        [ObservableProperty] private string _messages = string.Empty;
        [ObservableProperty] private bool _isError;
        [ObservableProperty] private int _selectedTabIndex;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsReadOnlyMode))]
        [NotifyCanExecuteChangedFor(nameof(RunCommand))]
        [NotifyCanExecuteChangedFor(nameof(SaveChangesCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteRowsCommand))]
        private bool _isEditMode;

        public bool IsReadOnlyMode => !IsEditMode;

        private TableInfo? _loadedTable;

        private HashSet<string> _notNullColumns = new();

        private List<string> _primaryKey = new();

        public bool IsKeyColumn(string columnName) => _primaryKey.Contains(columnName);

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

                    var allowedTables = db.Rolepermissions
                        .Where(r => r.Role == role)
                        .Select(r => r.TableName)
                        .ToList();

                    var sql = @"SELECT c.relname AS table_name, COALESCE(d.description, c.relname) AS display_name
                        FROM pg_class c
                        JOIN pg_namespace n ON n.oid = c.relnamespace
                        LEFT JOIN pg_description d ON d.objoid = c.oid AND d.objsubid = 0
                        WHERE c.relkind IN ('r', 'v') AND n.nspname = 'public'
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
                                    if (allowedTables.Contains(tableName))
                                    {
                                        var table = new TableInfo() { Name = tableName, DisplayName = displayName };

                                        string[] referenceTables = { "city", "street", "position", "profession", "department", "specialty", "qualification", "bank", "publisher", "author" };

                                        if (referenceTables.Contains(tableName.ToLower()))
                                            ReferenceTables.Add(table);
                                        else
                                            OtherTables.Add(table);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        partial void OnSelectedTableChanged(TableInfo? value)
        {
            if (value == null) return;

            IsEditMode = false;
            HighlightedRows = null;
            LoadTable(value);
        }

        private void LoadTable(TableInfo table)
        {
            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                using var db = new BookStoreDbContext();
                string sql = $"SELECT * FROM \"{table.Name}\" LIMIT 1000";

                db.Database.OpenConnection();
                long tOpen = sw.ElapsedMilliseconds;

                using var command = db.Database.GetDbConnection().CreateCommand();
                command.CommandText = sql;

                using var reader = command.ExecuteReader();
                long tQuery = sw.ElapsedMilliseconds;

                DataTable dt = new DataTable();
                dt.Load(reader);

                foreach (DataColumn col in dt.Columns)
                    col.ReadOnly = false;

                long tLoad = sw.ElapsedMilliseconds;

                TableData = dt.DefaultView;
                _loadedTable = table;

                Application.Current.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.ContextIdle,
                    new Action(() =>
                    {
                        long tRender = sw.ElapsedMilliseconds;
                        System.Diagnostics.Debug.WriteLine(
                            $"[{table.Name}] подключение: {tOpen} мс | запрос: {tQuery - tOpen} мс | " +
                            $"загрузка: {tLoad - tQuery} мс | отрисовка: {tRender - tLoad} мс | строк: {dt.Rows.Count}");
                    }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                HandleError(ex);
            }
        }

        [RelayCommand(CanExecute = nameof(CanRun))]
        private void Run()
        {
            if (string.IsNullOrWhiteSpace(RawSql))
            {
                ShowMessage("Запрос пустой.", isError: true);
                SelectedTabIndex = 1;
                return;
            }

            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var dt = ParseRawSqlService.SqlQuerry(RawSql);
                TableData = dt.DefaultView;
                _loadedTable = null;

                ShowMessage($"Выполнено успешно.\nСтрок: {dt.Rows.Count}\nВремя: {sw.ElapsedMilliseconds} мс", isError: false);
                SelectedTabIndex = 0;
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private bool CanRun() => !IsEditMode;

        [RelayCommand]
        private void ToggleEdit()
        {
            if (!IsEditMode)
            {
                if (_loadedTable == null || TableData == null)
                {
                    ShowMessage("Сначала выберите таблицу в списке слева.", isError: true);
                    SelectedTabIndex = 1;
                    return;
                }

                try
                {
                    _notNullColumns = TableEditService.GetNotNullColumns(_loadedTable.Name);
                    _primaryKey = TableEditService.GetPrimaryKey(_loadedTable.Name);

                    if (_primaryKey.Count == 0)
                    {
                        ShowMessage($"Редактирование невозможно: у «{_loadedTable.DisplayName}» нет первичного ключа " +
                                    "(это представление или таблица без PK).", isError: true);
                        SelectedTabIndex = 1;
                        return;
                    }

                    EnableEditing();
                    IsEditMode = true;

                    ShowMessage("Режим редактирования. Измените значения в ячейках и нажмите «Сохранить». " +
                                "Поля первичного ключа менять нельзя.", isError: false);
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
                return;
            }

            bool hasPending = TableData?.Table?.Rows.Cast<DataRow>()
                .Any(r => r.RowState == DataRowState.Modified) == true;

            if (hasPending)
            {
                var answer = MessageBox.Show("Есть несохранённые изменения. Отменить их?", "Внимание",
                                             MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (answer != MessageBoxResult.Yes) return;
            }

            IsEditMode = false;
            if (_loadedTable != null) LoadTable(_loadedTable);
        }

        [RelayCommand]
        private void AddNewRow()
        {
            if (TableData?.Table == null || !IsEditMode) return;

            DataRow newRow = TableData.Table.NewRow();

            TableData.Table.Rows.Add(newRow);
        }

        [RelayCommand(CanExecute = nameof(IsEditMode))]
        private void SaveChanges()
        {
            var table = TableData?.Table;
            if (_loadedTable == null || table == null) return;

            var addedRows = table.Rows.Cast<DataRow>()
                .Where(r => r.RowState == DataRowState.Added)
                .ToList();

            var modifiedRows = table.Rows.Cast<DataRow>()
                .Where(r => r.RowState == DataRowState.Modified)
                .ToList();

            if (addedRows.Count == 0 && modifiedRows.Count == 0)
            {
                ShowMessage("Нет изменений для сохранения.", isError: false);
                SelectedTabIndex = 1;
                return;
            }

            var errors = new List<string>();
            var allRowsToSave = addedRows.Concat(modifiedRows).ToList();

            foreach (var row in allRowsToSave)
            {
                foreach (var col in _notNullColumns)
                {
                    if (col.Equals("id", StringComparison.OrdinalIgnoreCase) && row.RowState == DataRowState.Added)
                    {
                        continue;
                    }

                    if (table.Columns.Contains(col) && TableEditService.IsEmpty(row[col]))
                    {
                        errors.Add($"Строка {table.Rows.IndexOf(row) + 1}: поле «{col}» не может быть пустым");
                    }
                }
            }

            if (errors.Count > 0)
            {
                ShowMessage("Сохранение отменено:\n" + string.Join("\n", errors), isError: true);
                SelectedTabIndex = 1;
                return;
            }

            try
            {
                int totalCount = 0;

                if (addedRows.Count > 0)
                {
                    int inserted = TableEditService.InsertRows(_loadedTable.Name, addedRows);
                    totalCount += inserted;
                }

                if (modifiedRows.Count > 0)
                {
                    int updated = TableEditService.UpdateRows(_loadedTable.Name, _primaryKey, modifiedRows);
                    totalCount += updated;
                }

                LoadTable(_loadedTable);
                if (IsEditMode) EnableEditing();

                ShowMessage($"Успешно сохранено строк: {totalCount}", isError: false);
                SelectedTabIndex = 1;
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        [RelayCommand(CanExecute = nameof(IsEditMode))]
        private void DeleteRows()
        {
            if (_loadedTable == null || TableData?.Table == null) return;

            var rows = SelectedRows.Select(r => r.Row)
                                   .Where(r => r.RowState != DataRowState.Detached)
                                   .ToList();

            if (rows.Count == 0)
            {
                ShowMessage("Выберите строки для удаления (клик по серому заголовку слева, Ctrl/Shift для нескольких).",
                            isError: false);
                SelectedTabIndex = 1;
                return;
            }

            var answer = MessageBox.Show(
                $"Удалить выбранные строки ({rows.Count}) из «{_loadedTable.DisplayName}»?\n\n" +
                "Связанные записи будут удалены каскадом. Это действие необратимо.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Warning,
                MessageBoxResult.No);
            if (answer != MessageBoxResult.Yes) return;

            try
            {
                int deleted = TableEditService.DeleteRows(_loadedTable.Name, _primaryKey, rows);

                foreach (var row in rows)
                {
                    row.Delete();
                    row.AcceptChanges();
                }

                ShowMessage($"Удалено строк: {deleted}", isError: false);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private void EnableEditing()
        {
            if (TableData == null) return;

            TableData.AllowEdit = true;
            TableData.AllowNew = false;
            TableData.AllowDelete = false;

            foreach (DataColumn col in TableData.Table!.Columns)
                col.ReadOnly = false;
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
            var searcher = new SearchService();

            try
            {
                HighlightedRows = searcher.Search(TableData, SearchInput);
            }
            catch
            {
                HighlightedRows = null;
            }

            try
            {
                var allTables = ReferenceTables.Concat(OtherTables).ToList();
                HighlightedTables = searcher.Search(allTables, SearchInput);
            }
            catch
            {
                HighlightedTables = null;
            }

            var firstMatch = HighlightedTables?.FirstOrDefault();
            if (firstMatch != null && !ReferenceEquals(firstMatch, SelectedTable))
            {
                SelectedTable = firstMatch;
            }
        }

        private void ShowMessage(string text, bool isError)
        {
            IsError = isError;
            Messages = text;
        }

        private void HandleError(Exception ex)
        {
            ShowMessage(FormatError(ex), isError: true);
            SelectedTabIndex = 1;

            bool isConnectionProblem = ex is NpgsqlException
                                       && ex is not PostgresException
                                       && ex.InnerException is not PostgresException;
            if (isConnectionProblem)
                ConnectionFailed?.Invoke(this, EventArgs.Empty);
        }

        private static string FormatError(Exception ex)
        {
            var pg = ex as PostgresException ?? ex.InnerException as PostgresException;
            if (pg == null)
                return $"Ошибка: {ex.Message}";

            var text = $"Ошибка SQL ({pg.SqlState}): {pg.MessageText}";
            if (pg.Position > 0) text += $"\nПозиция в запросе: {pg.Position}";
            if (!string.IsNullOrEmpty(pg.Detail)) text += $"\nДетали: {pg.Detail}";
            if (!string.IsNullOrEmpty(pg.Hint)) text += $"\nПодсказка: {pg.Hint}";
            return text;
        }
    }
}