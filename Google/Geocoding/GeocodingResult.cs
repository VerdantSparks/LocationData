using Newtonsoft.Json;

namespace LocationData.Google.Geocoding
{
    public class GeocodingResult : BaseQueryResult
    {
        [JsonProperty("results")]
        public Result[] Results { get; set; }
    }
}
