using System;
using Newtonsoft.Json;

namespace FetchUCM.Models
{
    public class MeetingTime
    {
        internal MeetingTime()
        {
            
        }
        
        [JsonProperty("beginTime")] public string BeginTimeRaw { get; private set; }
        [JsonProperty("endTime")] public string EndTimeRaw { get; private set; }
        [JsonProperty("startDate")] public string BeginDateRaw { get; private set; }
        [JsonProperty("endDate")] public string EndDateRaw { get; private set; }
        
        [JsonProperty("building")] public string Building { get; private set; }
        [JsonProperty("buildingDescription")] public string BuildingDescription { get; private set; }
        [JsonProperty("campus")] public string Campus { get; private set; }
        [JsonProperty("campusDescription")] public string CampusDescription { get; private set; }
        [JsonProperty("room")] public string Room { get; private set; }
        [JsonProperty("creditHourSession")] public float CreditHourSession { get; private set; }
        [JsonProperty("hoursWeek")] public float HoursPerWeek { get; private set; }

        [Flags] public enum Days : byte { Base = 0, Sunday = 1, Monday = 2, Tuesday = 4, Wednesday = 8, Thursday = 16, Friday = 32, Saturday = 64 }
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

        public enum MeetingType : byte { Lecture = 1, Discussion = 2, Lab = 3, Fieldwork = 4, Seminar = 5, IndividualStudy = 6, Tutorial = 7, Studio = 8, Practicum = 9, Exam = 10 }
        [JsonProperty("meetingType")] public string MeetingTypeRaw { get; private set; }
        public MeetingType Type {
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
                    _ => throw new InvalidOperationException($"Could not match {MeetingTypeRaw} to a MeetingType!")
                };
            }
        }
    }
}