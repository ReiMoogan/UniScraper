using Newtonsoft.Json;

namespace FetchUCM.Models;

public record Meeting
{
    internal Meeting()
    {
            
    }

    [JsonProperty("faculty")] public Professor[] Faculty { get; private set; }
    [JsonProperty("meetingTime")] public MeetingTime Time { get; private set; }
}