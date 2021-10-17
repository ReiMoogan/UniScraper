using Newtonsoft.Json;

namespace FetchRateMyProf.Models
{
    public interface IPageable<T>
    {
        [JsonProperty("remaining")] public int Remaining { get; }
        public T[] Items { get; }
    }
}