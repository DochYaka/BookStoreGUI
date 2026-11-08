using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

/// <summary>
/// Юридические лица
/// </summary>
public partial class Legalentity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string CurrentAccountNumber { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string HouseNumber { get; set; } = null!;

    public int? CityId { get; set; }

    public int? StreetId { get; set; }

    public virtual City? City { get; set; }

    public virtual Customer IdNavigation { get; set; } = null!;

    public virtual Street? Street { get; set; }

    public virtual Supplier? Supplier { get; set; }
}
