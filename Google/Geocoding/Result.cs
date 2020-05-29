using Newtonsoft.Json;

namespace LocationData.Google.Geocoding
{
    public class Result : BaseActualResult
    {
        [JsonProperty("access_points")]
        public AccessPoint[] AccessPoints { get; set; }
    }
}
