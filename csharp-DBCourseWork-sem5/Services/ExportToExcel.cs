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

            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            string fileName = $"{fileNamePrefix}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";
            string fullPath = Path.Combine(downloadPath, fileName);

            using var workBook = new XLWorkbook();

            workBook.Worksheets.Add(tableData.Table, "Данные");

            workBook.SaveAs(fullPath);
        }
    }
}