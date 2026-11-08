using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

/// <summary>
/// Заказы
/// </summary>
public partial class Order
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = null!;

    public DateOnly OrderDate { get; set; }

    public int Quantity { get; set; }

    public int CustomerId { get; set; }

    public int EmployeeId { get; set; }

    public int CopyId { get; set; }

    public virtual Copy Copy { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;
}
