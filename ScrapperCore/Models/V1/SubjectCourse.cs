using Newtonsoft.Json;

namespace ScrapperCore.Models.V1;

public record SubjectCourse
{
    // This is a horrible backwards-compatible naming scheme.
    [JsonProperty("id")] public string CRN { get; init; }
    [JsonProperty("course_name")] public string SimpleName { get; init; }
    [JsonProperty("term")] public int Term { get; init; }
    [JsonProperty("course_description")] public string CourseName { get; init; }
    [JsonProperty("course_subject")] public string Subject { get; init; }
}