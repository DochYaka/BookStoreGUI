using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

/// <summary>
/// Рабочие книги
/// </summary>
public partial class WorkBook
{
    public int Id { get; set; }

    public DateOnly DateOfEmployment { get; set; }

    public DateOnly? DateOfDismissal { get; set; }

    public DateOnly? DateOfTransfer { get; set; }

    public DateOnly BasisDate { get; set; }

    public string BasisNumber { get; set; } = null!;

    public string GroundsForTermination { get; set; } = null!;

    public int? PositionId { get; set; }

    public int? ProfessionId { get; set; }

    public int? StructuralUnitId { get; set; }

    public int? SpecialtyId { get; set; }

    public int? QualificationId { get; set; }

    public int? EmployeeId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Position? Position { get; set; }

    public virtual Profession? Profession { get; set; }

    public virtual Qualification? Qualification { get; set; }

    public virtual Specialty? Specialty { get; set; }

    public virtual Department? StructuralUnit { get; set; }
}
