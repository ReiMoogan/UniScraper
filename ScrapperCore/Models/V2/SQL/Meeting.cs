namespace ScrapperCore.Models.V2.SQL;

public class Meeting
{
    public int ClassId { get; set; }

    public string? BeginTime { get; set; }

    public string? EndTime { get; set; }

    public string? BeginDate { get; set; }

    public string? EndDate { get; set; }

    public string? Building { get; set; }

    public string? BuildingDescription { get; set; }

    public string? Campus { get; set; }

    public string? CampusDescription { get; set; }

    public string? Room { get; set; }

    public float CreditHourSession { get; set; }

    public float HoursPerWeek { get; set; }

    public byte InSession { get; set; }

    public byte MeetingType { get; set; }

    public virtual Class Class { get; set; } = null!;
}
