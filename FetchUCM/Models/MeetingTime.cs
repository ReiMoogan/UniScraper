using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace FetchUCM.Models;

public enum MeetingType : byte { Lecture = 1, Discussion = 2, Lab = 3, Fieldwork = 4, Seminar = 5, IndividualStudy = 6, Tutorial = 7, Studio = 8, Practicum = 9, Exam = 10, Project = 11, Internship = 12 }
[Flags] public enum Days : byte { Base = 0, Sunday = 1, Monday = 2, Tuesday = 4, Wednesday = 8, Thursday = 16, Friday = 32, Saturday = 64 }
    
public class MeetingTime : IDBMeetingTime
{
    internal MeetingTime()
    {
            
    }
        
    [JsonProperty("beginTime")] public string BeginTime { get; private set; }
    [JsonProperty("endTime")] public string EndTime { get; private set; }
    [JsonProperty("startDate")] public string BeginDate { get; private set; }
    [JsonProperty("endDate")] public string EndDate { get; private set; }
        
    [JsonProperty("building")] public string Building { get; private set; }
    [JsonProperty("buildingDescription")] public string BuildingDescriptionRaw { get; private set; }
    public string BuildingDescription
    {
        get
        {
            var writer = new StringWriter();
            WebUtility.HtmlDecode(BuildingDescriptionRaw, writer);
            return writer.ToString();
        }
    }
    [JsonProperty("campus")] public string Campus { get; private set; }
    [JsonProperty("campusDescription")] public string CampusDescription { get; private set; }
    [JsonProperty("room")] public string Room { get; private set; }
    [JsonProperty("creditHourSession")] public float CreditHourSession { get; private set; }
    [JsonProperty("hoursWeek")] public byte HoursPerWeek { get; private set; }
        
    // WTF IS THIS
    [JsonProperty("sunday")] public bool Sunday { get; private set; }
    [JsonProperty("monday")] public bool Monday { get; private set; }
    [JsonProperty("tuesday")] public bool Tuesday { get; private set; }
    [JsonProperty("wednesday")] public bool Wednesday { get; private set; }
    [JsonProperty("thursday")] public bool Thursday { get; private set; }
    [JsonProperty("friday")] public bool Friday { get; private set; }
    [JsonProperty("saturday")] public bool Saturday { get; private set; }
    public Days InSession
    {
        get
        {
            var temp = Days.Base;
            if (Sunday) temp |= Days.Sunday;
            if (Monday) temp |= Days.Monday;
            if (Tuesday) temp |= Days.Tuesday;
            if (Wednesday) temp |= Days.Wednesday;
            if (Thursday) temp |= Days.Thursday;
            if (Friday) temp |= Days.Friday;
            if (Saturday) temp |= Days.Saturday;
            return temp;
        }
    }
        
    [JsonProperty("meetingType")] public string MeetingTypeRaw { get; private set; }
    [JsonProperty("meetingTypeDescription")] public string MeetingTypeDescription { get; private set; }
    public MeetingType MeetingType {
        get
        {
            return MeetingTypeRaw switch
            {
                "LECT" => MeetingType.Lecture,
                "DISC" => MeetingType.Discussion,
                "LAB" => MeetingType.Lab,
                "FLDW" => MeetingType.Fieldwork,
                "SEM" => MeetingType.Seminar,
                "INI" => MeetingType.IndividualStudy,
                "TUT" => MeetingType.Tutorial,
                "STDO" => MeetingType.Studio,
                "PRA" => MeetingType.Practicum,
                "EXAM" => MeetingType.Exam,
                "PROJ" => MeetingType.Project,
                "INT" => MeetingType.Internship,
                _ => throw new InvalidOperationException($"Could not match {MeetingTypeRaw} to a MeetingType! Guess: ({MeetingTypeDescription})")
            };
        }
    }
}

public interface IDBMeetingTime
{
    [JsonProperty("beginTime")] public string BeginTime { get; }
    [JsonProperty("endTime")] public string EndTime { get; }
    [JsonProperty("startDate")] public string BeginDate { get; }
    [JsonProperty("endDate")] public string EndDate { get; }
    [JsonProperty("building")] public string Building { get; }
    public string BuildingDescription { get; }
    [JsonProperty("campus")] public string Campus { get; }
    [JsonProperty("campusDescription")] public string CampusDescription { get; }
    [JsonProperty("room")] public string Room { get; }
    [JsonProperty("creditHourSession")] public float CreditHourSession { get; }
    [JsonProperty("hoursWeek")] public byte HoursPerWeek { get; }
    public Days InSession { get; }
    public MeetingType MeetingType { get; }
}

public class DBMeetingTime : IDBMeetingTime
{
    public DBMeetingTime(int classId, IDBMeetingTime db)
    {
        ClassId = classId;
        BeginTime = db.BeginTime;
        EndTime = db.EndTime;
        BeginDate = db.BeginDate;
        EndDate = db.EndDate;
        Building = db.Building;
        BuildingDescription = db.BuildingDescription;
        Campus = db.Campus;
        CampusDescription = db.CampusDescription;
        Room = db.Room;
        CreditHourSession = db.CreditHourSession;
        HoursPerWeek = db.HoursPerWeek;
        InSession = db.InSession;
        MeetingType = db.MeetingType;
    }
        
    public int ClassId { get; }
    public string BeginTime { get; }
    public string EndTime { get; }
    public string BeginDate { get; }
    public string EndDate { get; }
    public string Building { get; }
    public string BuildingDescription { get; }
    public string Campus { get; }
    public string CampusDescription { get; }
    public string Room { get; }
    public float CreditHourSession { get; }
    public byte HoursPerWeek { get; }
    public Days InSession { get; }
    public MeetingType MeetingType { get; }
}