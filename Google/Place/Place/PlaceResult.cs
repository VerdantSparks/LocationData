using Newtonsoft.Json;

namespace LocationData.Google.Place.Place
{
    public class PlaceResult : BasePlaceResult
    {
        [JsonProperty("result")]
        public Result Result { get; set; }
    }
}
