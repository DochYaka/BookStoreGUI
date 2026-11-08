using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

/// <summary>
/// Сотрудники
/// </summary>
public partial class Employee
{
    public int Id { get; set; }

    public int WorkExperience { get; set; }

    public int? IndividualId { get; set; }

    public virtual Individual? Individual { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<WorkBook> WorkBooks { get; set; } = new List<WorkBook>();
}
