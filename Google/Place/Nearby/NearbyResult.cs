using Newtonsoft.Json;

namespace LocationData.Google.Place.Nearby
{
    public class NearbyResult : BasePlaceResult
    {
        [JsonProperty("results")]
        public Result[] Results { get; set; }
    }
}
