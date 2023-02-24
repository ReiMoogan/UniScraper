using Newtonsoft.Json;

namespace ScrapperCore.Models.V1;

public record ScheduleInfo
{
    [JsonProperty("number_of_days")] public int NumberOfDays { get; init; }
    [JsonProperty("earliest")] public int Earliest { get; init; }
    [JsonProperty("latest")] public int Latest { get; init; }
    [JsonProperty("gaps")] public int Gaps { get; init; }
    [JsonProperty("hasConflictingFinals")] public bool HasConflictingFinals { get; init; }
}