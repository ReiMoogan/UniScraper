using Newtonsoft.Json;

namespace ScrapperCore.Models.V1;

public class Class
{
    [JsonProperty("crn")] public string? Crn { get; init; }
    [JsonProperty("subject")] public string? Subject { get; init; }
    [JsonProperty("course_id")] public string? CourseId { get; init; }
    [JsonProperty("course_name")] public string? CourseName { get; init; }
    [JsonProperty("units")] public int Units { get; init; }
    [JsonProperty("type")] public string? Type { get; init; }
    [JsonProperty("days")] public string? Days { get; init; }
    [JsonProperty("hours")] public string? Hours { get; init; }
    [JsonProperty("room")] public string? Room { get; init; }
    [JsonProperty("dates")] public string? Dates { get; init; }
    [JsonProperty("instructor")] public string? Instructor { get; init; }
    [JsonProperty("lecture_crn")] public string? LectureCrn { get; init; }
    [JsonProperty("attached_crn")] public string? AttachedCrn { get; init; }
    [JsonProperty("term")] public string? Term { get; init; }
    [JsonProperty("capacity")] public int Capacity { get; init; }
    [JsonProperty("enrolled")] public int Enrolled { get; init; }
    [JsonProperty("available")] public int Available { get; init; }
    [JsonProperty("final_type")] public string? FinalType { get; init; }
    [JsonProperty("final_days")] public string? FinalDays { get; init; }
    [JsonProperty("final_hours")] public string? FinalHours { get; init; }
    [JsonProperty("final_room")] public string? FinalRoom { get; init; }
    [JsonProperty("final_dates")] public string? FinalDates { get; init; }
    [JsonProperty("simple_name")] public string? SimpleName { get; init; }
    [JsonProperty("linked_courses")] public string? LinkedCourses { get; init; }
    // Apparently this is equivalent to LectureCrn... WHY????
    [JsonProperty("lecture")] public string? Lecture => LectureCrn;
    // Honestly have no idea what this is supposed to represent, they're all NULL as of 202230 on the old courses API.
    [JsonProperty("discussion")] public string? Discussion { get; init; }
}