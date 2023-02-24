using Newtonsoft.Json;

namespace FetchUCM.Models;

public record Meeting
{
    [JsonConstructor]
    internal Meeting(Professor[] faculty, MeetingTime time)
    {
        Faculty = faculty;
        Time = time;
    }

    [JsonProperty("faculty")] public Professor[] Faculty { get; private set; }
    [JsonProperty("meetingTime")] public MeetingTime Time { get; private set; }
}