using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ScrapperCore.Models.V1;

public class ScheduleSearchQuery
{
    [JsonProperty("custom_events")] public IEnumerable<CustomEvent> CustomEvents { get; init; } = Array.Empty<CustomEvent>();
    [JsonProperty("course_list")] public IEnumerable<string> CourseList { get; init; } = Array.Empty<string>();
    [JsonProperty("term")] public string? Term { get; init; }
    [JsonProperty("earliest_time")] public string? EarliestTime { get; init; } // Either "any", null, or a time (integer)
    [JsonProperty("latest_time")] public string? LatestTime { get; init; } // Same as above
    [JsonProperty("gaps")] public SortOrder Gaps { get; init; }
    [JsonProperty("days")] public SortOrder Days { get; init; }
    [JsonProperty("search_full")] public bool SearchFull { get; init; }
    [JsonProperty("bad_crns")] public IEnumerable<int> BadCRNs { get; init; } = Array.Empty<int>();
}

[JsonConverter(typeof(StringEnumConverter))]
public enum SortOrder
{
    [EnumMember(Value = "asc")] Ascending,
    [EnumMember(Value = "desc")] Descending
}