using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using csharp_DBCourseWork_sem5.Models;
using csharp_DBCourseWork_sem5.Models.Interfaces;
using csharp_DBCourseWork_sem5.Services;
using Npgsql;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

namespace csharp_DBCourseWork_sem5.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private static readonly HashSet<string> ReferenceTableNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "city", "street", "position", "profession", "department",
            "specialty", "qualification", "bank", "publisher", "author"
        };

        private readonly ITableCatalogRepository _catalog;
        private readonly ITableDataRepository _data;
        private readonly IRowValidator _validator;
        private readonly IDialogService _dialogs;
        private readonly IUserSession _session;
        private readonly IExportService _export;
        private readonly ISearchService _search;

        public event EventHandler? LoadDataFailed;
        public event EventHandler? ConnectionFailed;

        public ObservableCollection<TableInfo> ReferenceTables { get; } = new();
        public ObservableCollection<TableInfo> OtherTables { get; } = new();

        [ObservableProperty] private DataView? _tableData;

        [ObservableProperty] private TableInfo? _selectedTable;

        [ObservableProperty] private string _rawSql = string.Empty;

        [ObservableProperty] private string _searchInput = string.Empty;

        [ObservableProperty] private DataRow[]? _highlightedRows;

        [ObservableProperty] private TableInfo[]? _highlightedTables;

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

        public MainViewModel(
            ITableCatalogRepository catalog,
            ITableDataRepository data,
            IRowValidator validator,
            IDialogService dialogs,
            IUserSession session,
            IExportService export,
            ISearchService search)
        {
            _catalog = catalog;
            _data = data;
            _validator = validator;
            _dialogs = dialogs;
            _session = session;
            _export = export;
            _search = search;
        }

        public void Initialize() => LoadTables();

        private void LoadTables()
        {
            try
            {
                foreach (var table in _catalog.GetAllowedTables(_session.CurrentRole))
                {
                    if (ReferenceTableNames.Contains(table.Name))
                        ReferenceTables.Add(table);
                    else
                        OtherTables.Add(table);
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
                var dt = _data.Load(table.Name);

                TableData = dt.DefaultView;
                _loadedTable = table;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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

            var sw = Stopwatch.StartNew();
            try
            {
                var dt = _data.ExecuteRawSql(RawSql);
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
                    _notNullColumns = _catalog.GetNotNullColumns(_loadedTable.Name);
                    _primaryKey = _catalog.GetPrimaryKey(_loadedTable.Name);

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

            if (hasPending && !_dialogs.Confirm("Есть несохранённые изменения. Отменить их?", "Внимание"))
                return;

            IsEditMode = false;
            if (_loadedTable != null) LoadTable(_loadedTable);
        }

        [RelayCommand]
        private void AddNewRow()
        {
            if (TableData?.Table == null || !IsEditMode || _loadedTable == null) return;

            DataRow newRow = TableData.Table.NewRow();

            try
            {
                var defaults = _catalog.GetDefaultValues(_loadedTable.Name);

                foreach (var kvp in defaults)
                {
                    if (TableData.Table.Columns.Contains(kvp.Key))
                        newRow[kvp.Key] = kvp.Value;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки значений по умолчанию: {ex.Message}");
            }

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

            var errors = _validator.Validate(table, addedRows.Concat(modifiedRows), _notNullColumns);
            if (errors.Count > 0)
            {
                ShowMessage("Сохранение отменено:\n" + string.Join("\n", errors), isError: true);
                SelectedTabIndex = 1;
                return;
            }

            try
            {
                int total = _data.Save(_loadedTable.Name, _primaryKey, addedRows, modifiedRows);

                LoadTable(_loadedTable);
                if (IsEditMode) EnableEditing();

                ShowMessage($"Успешно сохранено строк: {total}", isError: false);
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

            bool confirmed = _dialogs.Confirm(
                $"Удалить выбранные строки ({rows.Count}) из «{_loadedTable.DisplayName}»?\n\n" +
                "Связанные записи будут удалены каскадом. Это действие необратимо.",
                "Подтверждение удаления");
            if (!confirmed) return;

            try
            {
                int deleted = _data.Delete(_loadedTable.Name, _primaryKey, rows);

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
            if (TableData == null)
            {
                _dialogs.Warning("Сначала выберите таблицу!");
                return;
            }

            try
            {
                string prefix = SelectedTable?.Name ?? "Данные";
                _export.Export(TableData, prefix);

                _dialogs.Info("Файл успешно сохранен в папку Загрузки!");
            }
            catch (Exception ex)
            {
                _dialogs.Error($"Ошибка при экспорте:\n{ex.Message}");
            }
        }

        [RelayCommand]
        private void Search()
        {
            try
            {
                HighlightedRows = _search.Search(TableData, SearchInput);
            }
            catch
            {
                HighlightedRows = null;
            }

            try
            {
                HighlightedTables = _search.Search(ReferenceTables.Concat(OtherTables), SearchInput);
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