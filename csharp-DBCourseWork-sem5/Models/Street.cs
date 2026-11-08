using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

/// <summary>
/// Улицы
/// </summary>
public partial class Street
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Individual> Individuals { get; set; } = new List<Individual>();

    public virtual ICollection<Legalentity> Legalentities { get; set; } = new List<Legalentity>();
}
