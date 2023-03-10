namespace ScrapperCore.Models.V2.SQL;

public class Description
{
    public int Id { get; set; }

    public string CourseNumber { get; set; } = null!;

    public int TermStart { get; set; }

    public int TermEnd { get; set; }

    public string Department { get; set; } = null!;

    public string DepartmentCode { get; set; } = null!;

    public string? CourseDescription { get; set; }
}
