namespace ScrapperCore.Models.V2.SQL;

public class Professor
{
    public string Email { get; set; } = null!;

    public string? RmpId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? Department { get; set; }

    public int NumRatings { get; set; }

    public float Rating { get; set; }

    public float Difficulty { get; set; }

    public float WouldTakeAgainPercent { get; set; }

    public string FullName { get; set; } = null!;
}
