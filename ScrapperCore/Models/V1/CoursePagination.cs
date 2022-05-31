using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace ScrapperCore.Models.V1;

public class CoursePagination
{
    internal CoursePagination(IEnumerable<Class> results)
    {
        var classes = results.ToImmutableList();
        Results = classes;
        Count = classes.Count;
    }
    
    [JsonPropertyName("count")] public int Count { get; init; }
    // Honestly do not know what these two are, since they're always null. Hm.
    [JsonPropertyName("next")] public object Next { get; init; }
    [JsonPropertyName("previous")] public object Previous { get; init; }
    [JsonPropertyName("results")] public ImmutableList<Class> Results { get; init; }
}