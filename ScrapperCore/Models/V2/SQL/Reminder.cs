namespace ScrapperCore.Models.V2.SQL;

public class Reminder
{
    public decimal UserId { get; set; }

    public int ClassId { get; set; }

    public int MinTrigger { get; set; }

    public bool? ForWaitlist { get; set; }

    public bool? Triggered { get; set; }

    public virtual Class Class { get; set; } = null!;
}
