using System.Collections.Generic;
using Newtonsoft.Json;

namespace FetchUCM.Models;

public record FacultyMeetingTimes
{
    [JsonProperty("courseReferenceNumber")] public string CourseReferenceNumberRaw { get; private set; }
    public int CourseReferenceNumber => int.Parse(CourseReferenceNumberRaw);
    [JsonProperty("term")] public string TermRaw { get; private set; }
    public int Term => int.Parse(TermRaw);
    [JsonProperty("faculty")] public IEnumerable<Professor> Faculty;
    [JsonProperty("meetingTime")] public MeetingTime MeetingTime;
}