using Newtonsoft.Json;

namespace FetchUCM.Models
{
    public class Professor
    {
        internal Professor()
        {
            
        }
        
        [JsonProperty("bannerId")] public string BannerIdRaw { get; private set; }
        public int BannerId => short.Parse(BannerIdRaw);
        [JsonProperty("displayName")] public string DisplayName { get; private set; }
        [JsonProperty("emailAddress")] public string Email { get; private set; }
    }
}