using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

/// <summary>
/// Поставщики
/// </summary>
public partial class Supplier
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int LegalId { get; set; }

    public int? BankId { get; set; }

    public virtual Bank? Bank { get; set; }

    public virtual Legalentity Legal { get; set; } = null!;

    public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();
}
