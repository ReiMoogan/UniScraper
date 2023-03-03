using Newtonsoft.Json;
using static Newtonsoft.Json.NullValueHandling;

namespace FetchRateMyProf.Models;

internal class Query
{
    [JsonProperty("text")] public required string Text { get; init; }
    [JsonProperty("schoolID")] public required string SchoolId { get; init; }
    [JsonProperty("fallback")] public bool Fallback { get; init; } = true;
    [JsonProperty("departmentID")] public string? DepartmentId { get; init; }
}

public class ProfessorQuery : IRMPQuery
{
    public ProfessorQuery(string text, string schoolId, int first = 8, string? after = null)
    {
        Query = new Query
        {
            Text = text,
            SchoolId = schoolId
        };

        SchoolId = schoolId;
        First = first;
        After = after;
    }
    
    [JsonProperty("query")] internal Query Query { get; }
    [JsonIgnore] public string Text => Query.Text;
    [JsonProperty("schoolID")] public string SchoolId { get; }
    [JsonProperty("first")] public int First { get; }
    [JsonProperty("after", NullValueHandling = Ignore)] public string? After { get; set; }
}

public class RatingQuery : IRMPQuery
{
    public RatingQuery(string id, int first = 10, string? after = null)
    {
        Id = id;
        First = first;
        After = after;
    }
    
    [JsonProperty("id")] public string Id { get; }
    [JsonProperty("first")] public int First { get; }
    [JsonProperty("after", NullValueHandling = Ignore)] public string? After { get; set; }
}

public interface IRMPQuery
{
    [JsonProperty("first")] public int First { get; }
    [JsonProperty("after", NullValueHandling = Ignore)] public string? After { get; set; }
}