using Newtonsoft.Json;

namespace FetchUCM.Models;

internal interface IPageable<out T>
{
    [JsonProperty("success")] public bool Success { get; }
    [JsonProperty("totalCount")] public int TotalCount { get; }
    [JsonProperty("pageOffset")] public int PageOffset { get; }
    [JsonProperty("pageMaxSize")] public int PageMaxSize { get; }
    [JsonProperty("sectionsFetchedCount")] public int SectionsFetchedCount { get; }
    [JsonProperty("data")] public T[] Items { get; }
}