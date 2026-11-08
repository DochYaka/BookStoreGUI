using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

public partial class EmployeeInfo
{
    public int? Id { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? PassportSeries { get; set; }

    public string? PassportNumber { get; set; }

    public int? WorkExperience { get; set; }
}
