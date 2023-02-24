using Newtonsoft.Json;

namespace ScrapperCore.Models.V1;

public class CustomEvent
{
    [JsonProperty("event_name")] public string? EventName { get; init; }
    [JsonProperty("start_time")] public int StartTime { get; init; }
    [JsonProperty("end_time")] public int EndTime { get; init; }
    [JsonProperty("days")] public string? Days { get; init; }
    [JsonProperty("final_days")] public string? FinalDays { get; init; }
}