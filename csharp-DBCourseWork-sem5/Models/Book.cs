using System;
using System.Collections.Generic;

namespace csharp_DBCourseWork_sem5.Models;

/// <summary>
/// Книги
/// </summary>
public partial class Book
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Edition { get; set; }

    public int Year { get; set; }

    public string? Annotation { get; set; }

    public virtual ICollection<Copy> Copies { get; set; } = new List<Copy>();

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Publisher> PublishingHouses { get; set; } = new List<Publisher>();
}
