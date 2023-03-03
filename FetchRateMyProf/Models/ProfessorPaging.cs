using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FetchRateMyProf.Models;

internal record ProfessorPaging : IPageable<Professor>
{
    [JsonProperty("data")] public ProfessorData Data { get; init; } = null!;
    
    [JsonIgnore] public bool HasNextPage => Data.Search.Teachers.PageInfo.HasNextPage;
    [JsonIgnore] public string EndCursor => Data.Search.Teachers.PageInfo.EndCursor;
    [JsonIgnore] public IEnumerable<Professor> Items => Data.Search.Teachers.Edges.Select(o => o.Node);
};

internal record ProfessorData([JsonProperty("search")] ProfessorSearch Search);

internal record ProfessorSearch([JsonProperty("teachers")] Teachers Teachers);

internal record Teachers(
    [JsonProperty("edges")] Edges<Professor>[] Edges,
    [JsonProperty("pageInfo")] PageInfo PageInfo,
    [JsonProperty("resultCount")] int ResultCount
);