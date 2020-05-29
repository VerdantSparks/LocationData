using Newtonsoft.Json;

namespace LocationData.Google.Geocoding
{
    public class Geometry : Google.Geometry
    {
        [JsonProperty("location_type")]
        public string LocationType { get; set; }
    }
}
