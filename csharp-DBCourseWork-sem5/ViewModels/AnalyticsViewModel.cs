using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using csharp_DBCourseWork_sem5.Services;
using Npgsql;
using OxyPlot;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows;

namespace csharp_DBCourseWork_sem5.ViewModels
{
    public class BarRow
    {
        public string Label { get; init; } = string.Empty;
        public string ValueText { get; init; } = string.Empty;
        public double Percent { get; init; }
        public double Value { get; init; }
    }

    public partial class AnalyticsViewModel : ObservableObject
    {
        private static readonly CultureInfo Ru = new("ru-RU");

        private bool _suspend;
        private int _version;

        [ObservableProperty] private DateTime? _dateFrom;
        [ObservableProperty] private DateTime? _dateTo;

        [ObservableProperty] private string _revenue = "-";
        [ObservableProperty] private string _orders = "-";
        [ObservableProperty] private string _avgCheck = "-";
        [ObservableProperty] private string _profit = "-";

        public ObservableCollection<BarRow> ByMonth { get; } = new();
        public ObservableCollection<BarRow> TopBooks { get; } = new();
        public ObservableCollection<BarRow> ByCustomerType { get; } = new();

        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private bool _isEmpty;

        [ObservableProperty] private PlotModel? _monthPlot;
        [ObservableProperty] private PlotModel? _topBooksPlot;

    [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string _errorText = string.Empty;

        public bool HasError => !string.IsNullOrEmpty(ErrorText);

        [RelayCommand]
        private async Task InitializeAsync()
        {
            _suspend = true;
            try
            {
                var to = DateTime.Today;
                var from = to.AddYears(-1);

                var range = await AnalyticsService.QueryAsync(AnalyticsQueries.OrdersRange);
                if (range.Rows.Count > 0 && range.Rows[0][0] != DBNull.Value && range.Rows[0][1] != DBNull.Value)
                {
                    from = ToDate(range.Rows[0][0]);
                    to = ToDate(range.Rows[0][1]);
                }

                DateFrom = from;
                DateTo = to;
            }
            catch (Exception ex)
            {
                ErrorText = FormatError(ex);
            }
            finally
            {
                _suspend = false;
            }

            await LoadAsync();
        }

        partial void OnDateFromChanged(DateTime? value) => _ = LoadAsync();
        partial void OnDateToChanged(DateTime? value) => _ = LoadAsync();

        [RelayCommand]
        private void Refresh()
        {
            AnalyticsService.ClearCache();
            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            if (_suspend) return;

            if (DateFrom == null || DateTo == null)
            {
                ErrorText = "Укажите начало и конец периода.";
                return;
            }
            if (DateFrom > DateTo)
            {
                ErrorText = "Начало периода позже его конца.";
                return;
            }

            ErrorText = string.Empty;
            int version = ++_version;
            IsLoading = true;

            try
            {
                var p = new (string, object)[]
                {
                    ("from", DateOnly.FromDateTime(DateFrom.Value)),
                    ("to",   DateOnly.FromDateTime(DateTo.Value)),
                };

                var kpiT = AnalyticsService.QueryAsync(AnalyticsQueries.SalesKpi, p);
                var monthsT = AnalyticsService.QueryAsync(AnalyticsQueries.SalesByMonth, p);
                var booksT = AnalyticsService.QueryAsync(AnalyticsQueries.TopBooks, p);
                var typesT = AnalyticsService.QueryAsync(AnalyticsQueries.RevenueByCustomerType, p);

                await Task.WhenAll(kpiT, monthsT, booksT, typesT);
                if (version != _version) return;

                var k = kpiT.Result.Rows[0];
                decimal revenue = Dec(k[0]);
                long orders = Lng(k[1]);

                Revenue = Rub(revenue);
                Orders = orders.ToString("N0", Ru);
                AvgCheck = Rub(orders > 0 ? revenue / orders : 0);
                Profit = Rub(Dec(k[2]));
                IsEmpty = orders == 0;

                Fill(ByMonth,
                    monthsT.Result.Rows.Cast<DataRow>().TakeLast(12)
                        .Select(r => (ToDate(r[0]).ToString("MM.yyyy"), Dbl(r[1]))).ToList(),
                    asShare: false);

                Fill(TopBooks,
                    booksT.Result.Rows.Cast<DataRow>()
                        .Select(r => (Convert.ToString(r[1]) ?? "", Dbl(r[2]))).ToList(),
                    asShare: false);

                Fill(ByCustomerType,
                    typesT.Result.Rows.Cast<DataRow>()
                        .Select(r => (Convert.ToString(r[0]) ?? "", Dbl(r[1]))).ToList(),
                    asShare: true);
                MonthPlot = AnalyticsPlotBuilder.BuildLine(
                    "Выручка по месяцам",
                    ByMonth.Select(b => b.Label).ToList(),
                    ByMonth.Select(b => b.Value).ToList(),
                    xTitle: "Месяц",
                    yTitle: "Выручка, ₽",
                    seriesTitle: "Выручка");

                TopBooksPlot = AnalyticsPlotBuilder.BuildBar(
                    "Топ-10 книг по выручке",
                    TopBooks.Select(b => b.Label).ToList(),
                    TopBooks.Select(b => b.Value).ToList(),
                    xTitle: "Выручка, ₽",
                    yTitle: "Книга");
            }
            catch (Exception ex)
            {
                if (version == _version) ErrorText = FormatError(ex);
            }
            finally
            {
                if (version == _version) IsLoading = false;
            }
        }

        private static void Fill(ObservableCollection<BarRow> target, List<(string Label, double Value)> data, bool asShare)
        {
            target.Clear();
            if (data.Count == 0) return;

            double max = data.Max(d => d.Value);
            double total = data.Sum(d => d.Value);

            foreach (var (label, value) in data)
            {
                double percent = asShare
                    ? (total > 0 ? value / total * 100 : 0)
                    : (max > 0 ? value / max * 100 : 0);

                target.Add(new BarRow
                {
                    Label = label,
                    Percent = percent,
                    Value = value,
                    ValueText = asShare
                        ? $"{Rub((decimal)value)} · {percent:0.#}%"
                        : Rub((decimal)value),
                });
            }
        }

        [RelayCommand]
        private void Export()
        {
            try
            {
                var kpi = new List<(string, string)>
        {
            ("Выручка", Revenue),
            ("Заказов", Orders),
            ("Средний чек", AvgCheck),
            ("Прибыль", Profit),
        };

                var byMonth = ByMonth.Select(b => (b.Label, b.ValueText)).ToList();
                var topBooks = TopBooks.Select(b => (b.Label, b.ValueText)).ToList();
                var byType = ByCustomerType.Select(b => (b.Label, b.ValueText)).ToList();

                ExportToExcel.ExportAnalytics(
                    "Аналитика", DateFrom, DateTo, kpi, byMonth, topBooks, byType);

                MessageBox.Show("Файл сохранён в папку Загрузки!", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private static string Rub(decimal v) => v.ToString("N0", Ru) + " ₽";

        private static decimal Dec(object? v) => v == null || v == DBNull.Value ? 0m : Convert.ToDecimal(v);
        private static double Dbl(object? v) => v == null || v == DBNull.Value ? 0d : Convert.ToDouble(v);
        private static long Lng(object? v) => v == null || v == DBNull.Value ? 0L : Convert.ToInt64(v);
        private static DateTime ToDate(object v) => v is DateOnly d ? d.ToDateTime(TimeOnly.MinValue) : Convert.ToDateTime(v);

        private static string FormatError(Exception ex)
        {
            if (ex is PostgresException pg)
                return $"Ошибка запроса к БД: {pg.MessageText}";
            if (ex is NpgsqlException)
                return $"Нет соединения с БД: {ex.Message}";
            return $"Ошибка: {ex.Message}";
        }
    }

}