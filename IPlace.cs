using Microsoft.Azure.Cosmos.Spatial;
using Newtonsoft.Json;

namespace LocationData
{
    public interface IPlace
    {
        [JsonProperty("id")]
        string Id { get; set; }

        [JsonProperty("name")]
        string Name { get; set; }

        [JsonProperty("address")]
        string Address { get; set; }

        [JsonProperty("location")]
        Point Location { get; set; }
    }
}
