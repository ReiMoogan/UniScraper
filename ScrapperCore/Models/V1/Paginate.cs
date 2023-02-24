using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ScrapperCore.Models.V1;

public class Paginate<T>
{
    public Paginate(IEnumerable<T> results)
    {
        Results = results;
    }

    [JsonProperty("count")] public int Count => Results.Count();
    // Honestly do not know what these two are, since they're always null. Hm.
    [JsonProperty("next")] public object? Next { get; init; }
    [JsonProperty("previous")] public object? Previous { get; init; }
    [JsonProperty("results")] public IEnumerable<T> Results { get; init; }
}