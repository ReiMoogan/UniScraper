using Newtonsoft.Json;

namespace FetchRateMyProf.Models;

public class Professor
{
    [JsonConstructor]
    internal Professor(string department, string firstName, string lastName, string id, int numRatings, float averageRatings, float averageDifficulty, float wouldTakeAgainPercent)
    {
        Department = department;
        FirstName = firstName;
        LastName = lastName;
        Id = id;
        NumRatings = numRatings;
        AverageRatings = averageRatings;
        AverageDifficulty = averageDifficulty;
        WouldTakeAgainPercent = wouldTakeAgainPercent;
    }

    [JsonProperty("department")] public string Department { get; private set; }
    [JsonProperty("firstName")] public string FirstName { get; private set; }
    [JsonProperty("lastName")] public string LastName { get; private set; }
    [JsonProperty("id")] public string Id { get; private set; }
    [JsonProperty("numRatings")] public int NumRatings { get; private set; }
    [JsonProperty("avgRating")] public float AverageRatings { get; private set; }
    [JsonProperty("avgDifficulty")] public float AverageDifficulty { get; private set; }
    [JsonProperty("wouldTakeAgainPercent")] public float WouldTakeAgainPercent { get; private set; }
    
    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }
}