namespace ScrapperCore.Models.V2.SQL;

public class Faculty
{
    public int ClassId { get; set; }

    public string ProfessorEmail { get; set; } = null!;

    public virtual Class Class { get; set; } = null!;

    public virtual Professor ProfessorEmailNavigation { get; set; } = null!;
}
