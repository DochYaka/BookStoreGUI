using csharp_DBCourseWork_sem5.Models;
using csharp_DBCourseWork_sem5.Models.Interfaces;
using System.Data;

namespace csharp_DBCourseWork_sem5.Services.Tables
{
    public class SearchService : ISearchService
    {
        public DataRow[] Search(DataView? tableData, string searchInput)
        {
            if (tableData?.Table == null || string.IsNullOrWhiteSpace(searchInput))
                return Array.Empty<DataRow>();

            string search = searchInput.Trim();

            return tableData.Table.Rows.Cast<DataRow>()
                .Where(row => row.ItemArray.Any(field =>
                    field != null && field.ToString()!.Contains(search, StringComparison.OrdinalIgnoreCase)))
                .ToArray();
        }

        public TableInfo[] Search(IEnumerable<TableInfo> tables, string searchInput)
        {
            if (string.IsNullOrWhiteSpace(searchInput))
                return Array.Empty<TableInfo>();

            string search = searchInput.Trim();

            return tables
                .Where(t => t.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }
    }
}