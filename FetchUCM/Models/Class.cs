using System;
using Newtonsoft.Json;

namespace FetchUCM.Models
{
    public class Class
    {
        public enum Activity { Lecture, Discussion, Lab, Fieldwork, Seminar, IndividualStudy, Tutorial, Studio, Practice }
        [Flags] public enum Days : byte { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }
        
        // LECT, DISC, LAB, FLDW, SEM, INI, TUT, STDO, PRA
        internal Class(short crn, string classID)
        {
            
        }
        
        // Why are they storing it as a string?
        [JsonProperty("courseReferenceNumber")] public string CourseReferenceNumberRaw
        {
            set => CourseReferenceNumber = short.Parse(value);
        }
        public short CourseReferenceNumber { get; private set; }
        [JsonProperty("subject")] public string Subject { get; private set; }
        [JsonProperty("courseNumber")] public string CourseNumberRaw { get; private set; }
        [JsonProperty("sequenceNumber")] public string SequenceNumber { get; private set; }
        
        public string CourseNumber
        {
            get => $"{Subject}-{CourseNumberRaw}-{SequenceNumber}";
            set
            {
                var split = value.Split('-');
                if (split.Length >= 1) Subject = split[0];
                CourseNumberRaw = split[1];
                SequenceNumber = split[2];
            }
        }
        
        [JsonProperty("courseTitle")] public string CourseTitle { get; private set; }
        [JsonProperty("creditHours")] public byte Units { get; private set; }
        [JsonProperty("scheduleTypeDescription")] public string TypeRaw { get; private set; }
        public Activity Type { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        
        // Holds both date and time, or neither (null).
        public DateTime Exam { get; private set; }
        
        public short MaxEnrollment { get; private set; }
        public short SeatsAvailable { get; private set; }
        public short ActiveEnrollment { get; private set; }
        
        public short WaitCapacity { get; private set; }
        public short WaitCount { get; private set; }
        public short WaitAvailable { get; private set; }
    }
}