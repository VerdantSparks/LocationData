using System;
using LocationData.Google.Place.Place;
using Newtonsoft.Json;

namespace LocationData.Google.Place.Nearby
{
    public class Result : BaseActualResult
    {
        [JsonProperty("adr_address")]
        public string AdrAddress { get; set; }

        [JsonProperty("business_status")]
        public string BusinessStatus { get; set; }

        [JsonProperty("formatted_phone_number")]
        public string FormattedPhoneNumber { get; set; }

        [JsonProperty("icon")]
        public Uri Icon { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("international_phone_number")]
        public string InternationalPhoneNumber { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("opening_hours")]
        public Place.OpeningHours OpeningHours { get; set; }

        [JsonProperty("photos")]
        public Photo[] Photos { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("reviews")]
        public Review[] Reviews { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("user_ratings_total")]
        public long UserRatingsTotal { get; set; }

        [JsonProperty("utc_offset")]
        public long UtcOffset { get; set; }

        [JsonProperty("vicinity")]
        public string Vicinity { get; set; }

        [JsonProperty("website")]
        public Uri Website { get; set; }
    }
}
