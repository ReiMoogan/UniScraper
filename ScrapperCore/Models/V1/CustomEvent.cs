using Newtonsoft.Json;

namespace ScrapperCore.Models.V1;

public class CustomEvent
{
    [JsonProperty("event_name")] public string? EventName { get; init; }
    [JsonProperty("start_time")] public string? StartTime { get; init; }
    [JsonProperty("end_time")] public string? EndTime { get; init; }
    [JsonProperty("days")] public string? Days { get; init; }
}