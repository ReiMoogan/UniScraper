using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ScrapperCore.Models.V1;

public class CourseMatchQuery
{
    [JsonProperty("course_list"), Required] public IEnumerable<string>? CourseList { get; init; }
    [JsonProperty("term"), Required] public string? Term { get; init; }
}