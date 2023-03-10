namespace ScrapperCore.Models.V2.SQL;

public class LinkedSection
{
    public int Parent { get; set; }

    public int Child { get; set; }

    public virtual Class ChildNavigation { get; set; } = null!;

    public virtual Class ParentNavigation { get; set; } = null!;
}
