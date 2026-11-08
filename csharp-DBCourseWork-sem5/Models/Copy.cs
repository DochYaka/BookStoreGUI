using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

/// <summary>
/// Экземпляры книг
/// </summary>
public partial class Copy
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public decimal PurchasePrice { get; set; }

    public int? Quantity { get; set; }

    public decimal SalePrice { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();
}
