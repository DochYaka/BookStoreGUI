using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

/// <summary>
/// Банки
/// </summary>
public partial class Bank
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Bik { get; set; } = null!;

    public string Inn { get; set; } = null!;

    public string NumberOfCorrespondent { get; set; } = null!;

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}
