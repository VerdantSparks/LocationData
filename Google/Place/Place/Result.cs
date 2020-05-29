using System;
using Newtonsoft.Json;

namespace LocationData.Google.Place.Place
{
    public class Result : Nearby.Result
    {
        [JsonProperty("opening_hours")]
        public new OpeningHours OpeningHours { get; set; }
    }
}
