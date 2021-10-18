using Newtonsoft.Json;

namespace FetchRateMyProf.Models
{
    internal interface IPageable<out T>
    {
        [JsonProperty("remaining")] public int Remaining { get; }
        public T[] Items { get; }
    }
}