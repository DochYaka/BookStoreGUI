using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

/// <summary>
/// Клиенты
/// </summary>
public partial class Customer
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual Individual? Individual { get; set; }

    public virtual Legalentity? Legalentity { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
