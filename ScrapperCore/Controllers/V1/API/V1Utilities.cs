using System;

namespace ScrapperCore.Controllers.V1.API;

public static class V1Utilities
{
    public static int DefaultTerm()
    {
        var now = DateTime.Now;
        var semester = now.Month switch
        {
            <= 4 and >= 1 => 10, // Jan to Apr: Fall
            <= 7 and >= 5 => 20, // May to Jul: Summer
            _ => 30 // Aug to Dec: Winter
        };
        return now.Year * 100 + semester;
    }
}