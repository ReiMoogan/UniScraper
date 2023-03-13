using System.Collections.Generic;
using HotChocolate.Data;

namespace ScrapperCore.Models.V2.SQL;

public class Class
{
    public int Id { get; set; }

    public int Term { get; set; }

    public int CourseReferenceNumber { get; set; }

    public string CourseNumber { get; set; } = null!;

    public string? CampusDescription { get; set; }

    public string? CourseTitle { get; set; }

    public byte CreditHours { get; set; }

    public short MaximumEnrollment { get; set; }

    public short Enrollment { get; set; }

    public short SeatsAvailable { get; set; }

    public short? WaitCapacity { get; set; }

    public short? WaitAvailable { get; set; }

    [UseFiltering]
    public virtual ICollection<LinkedSection> LinkedSections { get; set; } = null!;
    
    [UseFiltering]
    public virtual ICollection<Meeting> Meetings { get; set; } = null!;

    [UseFiltering] 
    public virtual ICollection<Faculty> Faculty { get; set; } = null!;
}
