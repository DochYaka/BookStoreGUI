using ClosedXML.Excel;
using System.Data;
using System.IO;

namespace csharp_DBCourseWork_sem5.Services
{
    public static class ExportToExcel
    {
        public static void Export(DataView? tableData, string fileNamePrefix = "Экспорт")
        {
            if (tableData == null || tableData.Table == null || tableData.Table.Rows.Count == 0)
                throw new InvalidOperationException("Нет данных для экспорта.");

            string fullPath = BuildPath(fileNamePrefix);

            using var workBook = new XLWorkbook();
            workBook.Worksheets.Add(tableData.Table, "Данные");
            workBook.SaveAs(fullPath);
        }

        public static void ExportAnalytics(
            string fileNamePrefix,
            DateTime? dateFrom,
            DateTime? dateTo,
            IReadOnlyList<(string Label, string Value)> kpi,
            IReadOnlyList<(string Label, string Value)> byMonth,
            IReadOnlyList<(string Label, string Value)> topBooks,
            IReadOnlyList<(string Label, string Value)> byCustomerType)
        {
            string fullPath = BuildPath(fileNamePrefix);

            using var workBook = new XLWorkbook();

            var ws = workBook.Worksheets.Add("Аналитика");

            int row = 1;
            ws.Cell(row, 1).Value = "Период";
            ws.Cell(row, 2).Value = $"{dateFrom:dd.MM.yyyy} — {dateTo:dd.MM.yyyy}";
            ws.Range(row, 1, row, 2).Style.Font.Bold = true;
            row += 2;

            ws.Cell(row, 1).Value = "Показатель";
            ws.Cell(row, 2).Value = "Значение";
            ws.Range(row, 1, row, 2).Style.Font.Bold = true;
            row++;

            foreach (var (label, value) in kpi)
            {
                ws.Cell(row, 1).Value = label;
                ws.Cell(row, 2).Value = value;
                row++;
            }

            row++;
            WriteBlock(ws, ref row, "Выручка по месяцам", byMonth);
            row++;
            WriteBlock(ws, ref row, "Топ-10 книг по выручке", topBooks);
            row++;
            WriteBlock(ws, ref row, "Выручка по типам клиентов", byCustomerType);

            ws.Columns(1, 2).AdjustToContents();
            workBook.SaveAs(fullPath);
        }

        private static void WriteBlock(IXLWorksheet ws, ref int row, string title,
            IReadOnlyList<(string Label, string Value)> data)
        {
            ws.Cell(row, 1).Value = title;
            ws.Range(row, 1, row, 2).Style.Font.Bold = true;
            row++;

            ws.Cell(row, 1).Value = "Наименование";
            ws.Cell(row, 2).Value = "Значение";
            ws.Range(row, 1, row, 2).Style.Font.Bold = true;
            row++;

            foreach (var (label, value) in data)
            {
                ws.Cell(row, 1).Value = label;
                ws.Cell(row, 2).Value = value;
                row++;
            }
        }

        private static string BuildPath(string fileNamePrefix)
        {
            string downloadPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            string fileName = $"{fileNamePrefix}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";
            return Path.Combine(downloadPath, fileName);
        }
    }
}