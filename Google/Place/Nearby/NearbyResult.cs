using System;
using LocationData.Google.Place.Place;
using Newtonsoft.Json;

namespace LocationData.Google.Place.Nearby
{
    public class NearbyResult : BasePlaceResult
    {
        [JsonProperty("opening_hours")]
        public OpeningHours OpeningHours { get; set; }
    }
}
