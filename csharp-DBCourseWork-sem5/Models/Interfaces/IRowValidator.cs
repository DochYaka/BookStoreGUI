using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace csharp_DBCourseWork_sem5.Models.Interfaces
{
    public interface IRowValidator
    {
        IReadOnlyList<string> Validate(DataTable table, IEnumerable<DataRow> rows, ISet<string> notNullColumns);
    }
}
