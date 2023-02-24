using Newtonsoft.Json;

namespace FetchUCM.Models;

public record Term
{
    [JsonConstructor]
    internal Term(string codeRaw, string description)
    {
        CodeRaw = codeRaw;
        Description = description;
    }
        
    [JsonProperty("code")] public string CodeRaw { get; private set; }
    public int Code => int.Parse(CodeRaw);
    [JsonProperty("description")] public string Description { get; private set; }
}