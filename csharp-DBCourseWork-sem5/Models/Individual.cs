using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

/// <summary>
/// Физические лица
/// </summary>
public partial class Individual
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? PassportSeries { get; set; }

    public string? PassportNumber { get; set; }

    public DateOnly? DateIssue { get; set; }

    public string? IssuedBy { get; set; }

    public string HouseNumber { get; set; } = null!;

    public int? CityId { get; set; }

    public int? StreetId { get; set; }

    public virtual City? City { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Customer IdNavigation { get; set; } = null!;

    public virtual Street? Street { get; set; }
}
