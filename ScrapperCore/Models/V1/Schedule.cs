using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ScrapperCore.Models.V1;

public class Schedule
{
    [JsonProperty("schedule")] public Dictionary<string, IEnumerable<Class>> Schedules { get; init; } = null!;
    [JsonProperty("info")] public ScheduleInfo Info { get; init; } = null!;
    [JsonProperty("custom_events")] public IEnumerable<CustomEvent> CustomEvents { get; init; } = Array.Empty<CustomEvent>();
}