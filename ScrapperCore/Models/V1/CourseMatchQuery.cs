using System.Collections.Generic;
using Newtonsoft.Json;

namespace ScrapperCore.Models.V1;

public class CourseMatchQuery
{
    [JsonProperty("course_list")] public IEnumerable<string>? CourseList { get; init; }
    [JsonProperty("term")] public string? Term { get; init; }
}