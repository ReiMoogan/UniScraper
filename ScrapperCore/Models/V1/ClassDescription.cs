using Newtonsoft.Json;

namespace ScrapperCore.Models.V1;

public record ClassDescription
{
    [JsonProperty("name")] public string SimpleName { get; init; } = null!;
    [JsonProperty("description")] public string CourseName { get; init; } = null!;
}