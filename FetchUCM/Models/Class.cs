using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace FetchUCM.Models;

public record Class : IDBClass
{
    internal Class()
    {
            
    }

    [JsonIgnore] public int Id => Term * 10000 + CourseReferenceNumber;
    [JsonProperty("term")] public string TermRaw { get; private set; } = null!;
    [JsonIgnore] public int Term => int.Parse(TermRaw);
    [JsonProperty("termDesc")] public string? TermDescription { get; private set; }
    [JsonProperty("partOfTerm")] public string? PartOfTerm { get; private set; }
    // Why are they storing it as a string?
    [JsonProperty("courseReferenceNumber")] public string CourseReferenceNumberRaw { get; private set; } = null!;
    public int CourseReferenceNumber => int.Parse(CourseReferenceNumberRaw);
    [JsonProperty("subject")] public string Subject { get; private set; } = null!;
    [JsonProperty("courseNumber")] public string CourseNumberRaw { get; private set; } = null!;
    [JsonProperty("sequenceNumber")] public string SequenceNumber { get; private set; } = null!;
    public string CourseNumber
    {
        get => $"{Subject}-{CourseNumberRaw}-{SequenceNumber}";
        set
        {
            var split = value.Split('-');
            if (split.Length >= 1) Subject = split[0];
            if (split.Length >= 2) CourseNumberRaw = split[1];
            if (split.Length >= 3) SequenceNumber = split[2];
        }
    }
    [JsonProperty("campusDescription")] public string CampusDescription { get; private set; } = null!;
    [JsonProperty("scheduleTypeDescription")] public string? ScheduleTypeDescription { get; private set; }
    [JsonProperty("courseTitle")] public string CourseTitleRaw { get; private set; } = null!;
    public string CourseTitle
    {
        get
        {
            var writer = new StringWriter();
            WebUtility.HtmlDecode(CourseTitleRaw, writer);
            return writer.ToString();
        }
    }
    [JsonProperty("creditHours")] public byte CreditHours { get; private set; }
    [JsonProperty("maximumEnrollment")] public short MaximumEnrollment { get; private set; }
    [JsonProperty("enrollment")] public short Enrollment { get; private set; }
    [JsonProperty("seatsAvailable")] public short SeatsAvailable { get; private set; }
    [JsonProperty("waitCapacity")] public short? WaitCapacity { get; private set; }
    [JsonProperty("waitCount")] public short? WaitCount { get; private set; }
    [JsonProperty("waitAvailable")] public short? WaitAvailable { get; private set; }
    [JsonProperty("faculty")] public Professor[] Faculty { get; private set; } = Array.Empty<Professor>();
    [JsonProperty("meetingsFaculty")] public Meeting[] MeetingsFaculty { get; private set; } = Array.Empty<Meeting>();
}

public interface IDBClass
{
    public int Id { get; }
    public int Term { get; }
    public int CourseReferenceNumber { get; }
    public string CourseNumber { get; }
    [JsonProperty("campusDescription")] public string CampusDescription { get; }
    public string CourseTitle { get; }
    [JsonProperty("creditHours")] public byte CreditHours { get; }
    [JsonProperty("maximumEnrollment")] public short MaximumEnrollment { get; }
    [JsonProperty("enrollment")] public short Enrollment { get; }
    [JsonProperty("seatsAvailable")] public short SeatsAvailable { get; }
    [JsonProperty("waitCapacity")] public short? WaitCapacity { get; }
    [JsonProperty("waitAvailable")] public short? WaitAvailable { get; }
}