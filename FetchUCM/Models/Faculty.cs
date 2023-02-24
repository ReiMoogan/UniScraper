namespace FetchUCM.Models;

public record Faculty
{
    public int ClassId { get; set; }
    public string? ProfessorEmail { get; set; }
}