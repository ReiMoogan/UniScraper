using System;
using Newtonsoft.Json;

namespace FetchRateMyProf.Models;

public class Review
{
    internal Review()
    {
            
    }
        
    [JsonProperty("attendance")] public string? Attendance { get; private set; }
    [JsonProperty("rEasy")] public float Easy { get; private set; }
    [JsonProperty("rClarity")] public float Clarity { get; private set; }
    [JsonProperty("rHelpful")] public float Helpful { get; private set; }
    [JsonProperty("rOverall")] public float Overall { get; private set; }
    [JsonProperty("rComments")] public string? Comments { get; private set; }
    [JsonProperty("teacherGrade")] public string? GradeReceived { get; private set; }
    [JsonProperty("sId")] public int SchoolId { get; private set; }
    [JsonProperty("rTimestamp")] public long Timestamp { get; private set; }
    [JsonIgnore] public DateTimeOffset TimeCreated => DateTimeOffset.FromUnixTimeMilliseconds(Timestamp);
    [JsonProperty("rTextBookUser")] public string? TextbookUse { get; private set; }
    [JsonProperty("rWouldTakeAgain")] public string? WouldTakeAgain { get; private set; }
    [JsonProperty("takenForCredit")] public string? TakenForCredit { get; private set; }
    [JsonProperty("teacherRatingTags")] public string?[] TeacherRatingTags { get; private set; }
}