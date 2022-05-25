using System;
using System.Security.Cryptography;
using System.Text;
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
    [JsonProperty("emailAddress")] public string EmailRaw { get; private set; }

    public string Email
    {
        get
        {
            if (EmailRaw != null)
                return EmailRaw;
            
            // Time to pray that display names are consistent, since Banner's IDs are unreliable af.
            var hashFunction = MD5.Create();
            var hashedName = hashFunction.ComputeHash(Encoding.UTF8.GetBytes(DisplayName));
            var appendNumber = BitConverter.ToUInt16(hashedName, 0);
            return FirstName[..1].ToLower() + LastName.ToLower() + appendNumber + ".fake@ucmerced.edu";
        }
    }

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
    public string Email { get; }
    /// For compatibility with our database, unused by Banner.
    public float Rating { get; }
    /// For compatibility with our database, unused by Banner.
    public int NumRatings { get; }
}