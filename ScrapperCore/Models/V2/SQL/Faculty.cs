using HotChocolate.Data;

namespace ScrapperCore.Models.V2.SQL;

public class Faculty
{
    public int Id { get; set; }
    
    public int ClassId { get; set; }

    public string ProfessorEmail { get; set; } = null!;

    [UseFiltering]
    public virtual Class Class { get; set; } = null!;

    [UseFiltering]
    public virtual Professor Professor { get; set; } = null!;
}
