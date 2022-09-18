using Newtonsoft.Json;

namespace FetchUCM.Models;

public record Term
{
    internal Term()
    {
            
    }
        
    [JsonProperty("code")] public string CodeRaw { get; private set; }
    public int Code => int.Parse(CodeRaw);
    [JsonProperty("description")] public string Description { get; private set; }
}