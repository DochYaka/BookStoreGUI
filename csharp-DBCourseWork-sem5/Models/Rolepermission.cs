using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

public partial class Rolepermission
{
    public string Role { get; set; } = null!;

    public string TableName { get; set; } = null!;
}
