using Newtonsoft.Json;

namespace FetchRateMyProf.Models;

internal class ProfessorPaging : IPageable<Professor>
{
    [JsonConstructor]
    internal ProfessorPaging(Professor[] items, int searchResultsTotal, int remaining)
    {
        Items = items;
        SearchResultsTotal = searchResultsTotal;
        Remaining = remaining;
    }

    [JsonProperty("professors")] public Professor[] Items { get; private set; }
    [JsonProperty("searchResultsTotal")] public int SearchResultsTotal { get; private set; }
    [JsonProperty("remaining")] public int Remaining { get; private set; }
}