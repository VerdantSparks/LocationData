using Newtonsoft.Json;

namespace LocationData.Google.Place.Place
{
    public class Period
    {
        [JsonProperty("open")]
        public Open Open { get; set; }
    }
}
