using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace FetchUCM.Models;

public record Description : IDBDescription
{
    internal Description()
    {
        
    }
    
    [JsonProperty("id")] public int Id { get; private set; }
    [JsonProperty("termEffective")] public string TermRaw { get; private set; } = null!;
    public int Term => int.Parse(TermRaw);
    [JsonProperty("termStart")] public string TermStartRaw { get; private set; } = null!;
    public int TermStart => int.Parse(TermStartRaw);
    [JsonProperty("termEnd")] public string TermEndRaw { get; private set; } = null!;
    public int TermEnd => int.Parse(TermEndRaw);
    [JsonProperty("subject")] public string Subject { get; private set; } = null!;
    [JsonProperty("courseNumber")] public string CourseNumberRaw { get; private set; } = null!;
    public string CourseNumber
    {
        get => $"{Subject}-{CourseNumberRaw}";
        set
        {
            var split = value.Split('-');
            if (split.Length >= 1) Subject = split[0];
            if (split.Length >= 2) CourseNumberRaw = split[1];
        }
    }
    [JsonProperty("department")] public string Department { get; private set; } = null!;
    [JsonProperty("departmentCode")] public string DepartmentCode { get; private set; } = null!;
    [JsonProperty("courseDescription")] public string? CourseDescriptionRaw { get; private set; }
    public string? CourseDescription
    {
        get
        {
            if (CourseDescriptionRaw == null)
                return null;

            var writer = new StringWriter();
            WebUtility.HtmlDecode(CourseDescriptionRaw, writer);
            return writer.ToString();
        }
    }
}

public record CourseExtendedAttributes
{
    public string CourseNumber { get; init; } = null!;
    public string? CourseDescription { get; init; }

    public override string ToString()
    {
        return CourseNumber;
    }
}

public interface IDBDescription
{
    [JsonProperty("id")] public int Id { get; }
    public int TermStart { get; }
    public int TermEnd { get; }
    public string CourseNumber { get; }
    [JsonProperty("department")] public string Department { get; }
    [JsonProperty("departmentCode")] public string DepartmentCode { get; }
    public string? CourseDescription { get; }
}