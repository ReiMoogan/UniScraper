using System;

namespace ScrapperCore.Models.V2.SQL;

public class Stat
{
    public string TableName { get; set; } = null!;

    public DateTime LastUpdate { get; set; }
}
