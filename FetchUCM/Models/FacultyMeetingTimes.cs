using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FetchUCM.Models;

public record FacultyMeetingTimes
{
    [JsonProperty("courseReferenceNumber")] public string CourseReferenceNumberRaw { get; private set; } = null!;
    public int CourseReferenceNumber => int.Parse(CourseReferenceNumberRaw);
    [JsonProperty("term")] public string TermRaw { get; private set; } = null!;
    public int Term => int.Parse(TermRaw);
    [JsonProperty("faculty")] public IEnumerable<Professor> Faculty = Array.Empty<Professor>();
    [JsonProperty("meetingTime")] public MeetingTime MeetingTime = new();
}