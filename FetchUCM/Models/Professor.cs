using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace FetchUCM.Models;

public record Professor : IDBProfessor
{
    internal Professor()
    {

    }

    [JsonProperty("displayName")] public string DisplayName { get; private set; } = null!;
    [JsonProperty("emailAddress")] public string? EmailRaw { get; private set; }

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
            
            var writer = new StringWriter();
            WebUtility.HtmlDecode(firstName, writer);
            return writer.ToString();
        }
    }
        
    public string LastName {
        get {
            var nameSplit = DisplayName.Split(',');
            var lastName = nameSplit[0].Trim();
            
            var writer = new StringWriter();
            WebUtility.HtmlDecode(lastName, writer);
            return writer.ToString();
        }
    }
        
    public float Rating => 0;
    public int NumRatings => 0;
    public float Difficulty => 0;
}

public interface IDBProfessor
{
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    /// For compatibility with our database, unused by Banner.
    public float Rating { get; }
    /// For compatibility with our database, unused by Banner.
    public int NumRatings { get; }
    /// For compatibility with our database, unused by Banner.
    public float Difficulty { get; }
}