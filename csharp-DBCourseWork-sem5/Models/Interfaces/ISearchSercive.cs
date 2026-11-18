using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace csharp_DBCourseWork_sem5.Models.Interfaces
{
    public interface ISearchService
    {
        DataRow[]? Search(DataView? data, string query);
        TableInfo[]? Search(IEnumerable<TableInfo> tables, string query);
    }
}
