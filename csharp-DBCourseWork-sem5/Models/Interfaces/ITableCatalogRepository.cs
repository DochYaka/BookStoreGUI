using System;
using System.Collections.Generic;
using System.Text;

namespace csharp_DBCourseWork_sem5.Models.Interfaces
{
    public interface ITableCatalogRepository
    {
        IReadOnlyList<TableInfo> GetAllowedTables(string role);
        HashSet<string> GetNotNullColumns(string tableName);
        List<string> GetPrimaryKey(string tableName);
        Dictionary<string, object?> GetDefaultValues(string tableName);
    }
}
