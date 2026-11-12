using csharp_DBCourseWork_sem5.Models;
using System.Data;
using System.Windows.Controls;

namespace csharp_DBCourseWork_sem5.Services.Tables
{
    public class SearchService
    {
        public DataRow[] Search(DataView? tableData, string searchInput)
        {
            if (tableData == null || string.IsNullOrWhiteSpace(searchInput))
                return Array.Empty<DataRow>();

            string search = searchInput.Trim();

            return tableData.Table.Rows.Cast<DataRow>()
                .Where(row => row.ItemArray.Any(field => field != null && field.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)))
                .ToArray();
        }

        public TableInfo[] Search(List<TableInfo> tables, string searchInput)
        {
            if (string.IsNullOrEmpty(searchInput))
                return Array.Empty<TableInfo>();

            string search = searchInput.Trim();

            return tables.Where(t => t.DisplayName.Contains(search)).ToArray();
        }
    }
}
