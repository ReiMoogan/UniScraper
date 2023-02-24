using Newtonsoft.Json;

namespace ScrapperCore.Models.V1;

public record SubjectCourse
{
    // This is a horrible backwards-compatible naming scheme.
    [JsonProperty("id")] public string CRN { get; init; } = null!;
    [JsonProperty("course_name")] public string SimpleName { get; init; } = null!;
    [JsonProperty("term")] public int Term { get; init; }
    [JsonProperty("course_description")] public string CourseName { get; init; } = null!;
    [JsonProperty("course_subject")] public string Subject { get; init; } = null!;
}