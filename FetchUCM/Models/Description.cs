using Newtonsoft.Json;

namespace FetchUCM.Models;

public class Description
{
    internal Description()
    {
        
    }
    
    [JsonProperty("id")] public int Id { get; private set; }
    [JsonProperty("termEffective")] public string TermRaw { get; private set; }
    public int Term => int.Parse(TermRaw);
    [JsonProperty("termStart")] public string TermStartRaw { get; private set; }
    public int TermStart => int.Parse(TermStartRaw);
    [JsonProperty("termEnd")] public string TermEndRaw { get; private set; }
    public int TermEnd => int.Parse(TermEndRaw);
    [JsonProperty("subject")] public string Subject { get; private set; }
    [JsonProperty("courseNumber")] public string CourseNumberRaw { get; private set; }
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
    [JsonProperty("department")] public string Department { get; private set; }
    [JsonProperty("departmentCode")] public string DepartmentCode { get; private set; }
    [JsonProperty("courseDescription")] public string CourseDescription { get; private set; }
}