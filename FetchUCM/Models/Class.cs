using Newtonsoft.Json;

namespace FetchUCM.Models
{
    public class Class : IDBClass
    {
        internal Class()
        {
            
        }
        
        [JsonProperty("id")] public int Id { get; private set; }
        [JsonProperty("term")] public string TermRaw { get; private set; }
        public int Term => int.Parse(TermRaw);
        [JsonProperty("termDesc")] public string TermDescription { get; private set; }
        [JsonProperty("partOfTerm")] public string PartOfTerm { get; private set; }
        // Why are they storing it as a string?
        [JsonProperty("courseReferenceNumber")] public string CourseReferenceNumberRaw { get; private set; }
        public int CourseReferenceNumber => int.Parse(CourseReferenceNumberRaw);
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
        [JsonProperty("campusDescription")] public string CampusDescription { get; private set; }
        [JsonProperty("scheduleTypeDescription")] public string ScheduleTypeDescription { get; private set; }
        [JsonProperty("courseTitle")] public string CourseTitle { get; private set; }
        [JsonProperty("creditHours")] public byte CreditHours { get; private set; }
        [JsonProperty("maximumEnrollment")] public short MaximumEnrollment { get; private set; }
        [JsonProperty("enrollment")] public short Enrollment { get; private set; }
        [JsonProperty("seatsAvailable")] public short SeatsAvailable { get; private set; }
        [JsonProperty("waitCapacity")] public short WaitCapacity { get; private set; }
        [JsonProperty("waitCount")] public short WaitCount { get; private set; }
        [JsonProperty("waitAvailable")] public short WaitAvailable { get; private set; }
        [JsonProperty("faculty")] public Professor[] Faculty { get; private set; }
        [JsonProperty("meetingsFaculty")] public Meeting[] MeetingsFaculty { get; private set; }
    }

    public interface IDBClass
    {
        [JsonProperty("id")] public int Id { get; }
        [JsonProperty("term")] public string TermRaw { get; }
        public int Term { get; }
        [JsonProperty("termDesc")] public string TermDescription { get; }
        [JsonProperty("partOfTerm")] public string PartOfTerm { get; }
        public int CourseReferenceNumber { get; }
        [JsonProperty("subject")] public string Subject { get; }
        [JsonProperty("courseNumber")] public string CourseNumberRaw { get; }
        [JsonProperty("sequenceNumber")] public string SequenceNumber { get; }
        [JsonProperty("campusDescription")] public string CampusDescription { get; }
        [JsonProperty("scheduleTypeDescription")] public string ScheduleTypeDescription { get; }
        [JsonProperty("courseTitle")] public string CourseTitle { get; }
        [JsonProperty("creditHours")] public byte CreditHours { get; }
        [JsonProperty("maximumEnrollment")] public short MaximumEnrollment { get; }
        [JsonProperty("enrollment")] public short Enrollment { get; }
        [JsonProperty("seatsAvailable")] public short SeatsAvailable { get; }
        [JsonProperty("waitCapacity")] public short WaitCapacity { get; }
        [JsonProperty("waitCount")] public short WaitCount { get; }
        [JsonProperty("waitAvailable")] public short WaitAvailable { get; }
    }
}