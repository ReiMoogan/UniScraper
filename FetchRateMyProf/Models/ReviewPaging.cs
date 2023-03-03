using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FetchRateMyProf.Models;

internal record ReviewPaging : IPageable<Review>
{
    [JsonProperty("data")] private Data Data { get; init; } = null!;


    [JsonIgnore] public bool HasNextPage => Data.Node.Ratings.PageInfo.HasNextPage;
    [JsonIgnore] public string EndCursor => Data.Node.Ratings.PageInfo.EndCursor;
    [JsonIgnore] public IEnumerable<Review> Items => Data.Node.Ratings.Edges.Select(o => o.Node);
}

internal record Data(
    [JsonProperty("node")] Node Node
);

internal record Node(
    [JsonProperty("ratings")] Ratings Ratings
);

internal record Ratings(
    [JsonProperty("edges")] Edges<Review>[] Edges,
    [JsonProperty("pageInfo")] PageInfo PageInfo
);

internal record Edges<T>(
    [JsonProperty("cursor")] string Cursor,
    [JsonProperty("node")] T Node
);

internal record PageInfo(
    [JsonProperty("endCursor")] string EndCursor,
    [JsonProperty("hasNextPage")] bool HasNextPage
);

