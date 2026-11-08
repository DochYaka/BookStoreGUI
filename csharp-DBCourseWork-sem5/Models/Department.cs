using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

/// <summary>
/// Отделы
/// </summary>
public partial class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<WorkBook> WorkBooks { get; set; } = new List<WorkBook>();
}
