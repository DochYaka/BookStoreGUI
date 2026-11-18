using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace csharp_DBCourseWork_sem5.Models.Interfaces
{
    public interface IExportService { void Export(DataView data, string prefix); }
}
