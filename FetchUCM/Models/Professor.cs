using Newtonsoft.Json;

namespace FetchUCM.Models;

public class Professor : IDBProfessor
{
    internal Professor()
    {
            
    }
        
    [JsonProperty("bannerId")] public string BannerIdRaw { get; private set; }
    public int BannerId => int.Parse(BannerIdRaw);
    public int Id => BannerId; // Renaming for DB
    [JsonProperty("displayName")] public string DisplayName { get; private set; }
    [JsonProperty("emailAddress")] public string Email { get; private set; }
        
    public string FirstName {
        get {
            var nameSplit = DisplayName.Split(',');
            var firstName = nameSplit[^1].Trim();
            return firstName;
        }
    }
        
    public string LastName {
        get {
            var nameSplit = DisplayName.Split(',');
            var lastName = nameSplit[0].Trim();
            return lastName;
        }
    }
        
    public float Rating => 0;
    public int NumRatings => 0;
}

public interface IDBProfessor
{
    public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    [JsonProperty("emailAddress")] public string Email { get; }
    public float Rating { get; }
    public int NumRatings { get; }
}