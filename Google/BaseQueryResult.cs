using Newtonsoft.Json;

namespace LocationData.Google
{
    public abstract class BaseQueryResult
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
