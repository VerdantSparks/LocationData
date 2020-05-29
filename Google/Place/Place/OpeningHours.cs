using Newtonsoft.Json;

namespace LocationData.Google.Place.Place
{
    public class OpeningHours : LocationData.Google.Place.Nearby.OpeningHours
    {
        [JsonProperty("periods")]
        public Period[] Periods { get; set; }

        [JsonProperty("weekday_text")]
        public string[] WeekdayText { get; set; }
    }
}
