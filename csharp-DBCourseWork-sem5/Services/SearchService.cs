using System.Data;

namespace csharp_DBCourseWork_sem5.Services
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
    }
}
