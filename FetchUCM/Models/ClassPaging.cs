using Newtonsoft.Json;

namespace FetchUCM.Models
{
    internal class ClassPaging : IPageable<Class>
    {
        [JsonProperty("success")] public bool Success { get; private set; }
        [JsonProperty("totalCount")] public int TotalCount { get; private set; }
        [JsonProperty("pageOffset")] public int PageOffset { get; private set; }
        [JsonProperty("pageMaxSize")] public int PageMaxSize { get; private set; }
        [JsonProperty("sectionsFetchedCount")] public int SectionsFetchedCount { get; private set; }
        [JsonProperty("data")] public Class[] Items { get; private set; }
    }
}