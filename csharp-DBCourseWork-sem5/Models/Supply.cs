using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

/// <summary>
/// Поставки
/// </summary>
public partial class Supply
{
    public int Id { get; set; }

    public DateOnly DeliveryDate { get; set; }

    public int Quantity { get; set; }

    public int SupplierId { get; set; }

    public int CopyId { get; set; }

    public virtual Copy Copy { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;
}
