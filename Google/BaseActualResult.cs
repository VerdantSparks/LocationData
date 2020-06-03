using Newtonsoft.Json;

namespace LocationData.Google
{
    public class BaseActualResult
    {
        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("plus_code")]
        public PlusCode PlusCode { get; set; }

        [JsonProperty("types")]
        public string[] Types { get; set; }
    }
}
