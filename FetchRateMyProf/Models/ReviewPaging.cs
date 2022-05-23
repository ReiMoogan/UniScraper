using Newtonsoft.Json;

namespace FetchRateMyProf.Models;

internal class ReviewPaging : IPageable<Review>
{
    [JsonConstructor]
    internal ReviewPaging(Review[] items, int remaining)
    {
        Items = items;
        Remaining = remaining;
    }

    [JsonProperty("reviews")] public Review[] Items { get; private set; }
    [JsonProperty("remaining")] public int Remaining { get; private set; }
}