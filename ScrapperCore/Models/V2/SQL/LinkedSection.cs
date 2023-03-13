using HotChocolate.Data;

namespace ScrapperCore.Models.V2.SQL;

public class LinkedSection
{
    public int Id { get; set; }
    
    public int Parent { get; set; }

    public int Child { get; set; }

    [UseFiltering]
    public virtual Class ChildNavigation { get; set; } = null!;

    [UseFiltering]
    public virtual Class ParentNavigation { get; set; } = null!;
}
