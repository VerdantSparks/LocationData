using Newtonsoft.Json;

namespace LocationData.Google
{
    public class Location
    {
        [JsonProperty("lat")]
        private double Lat
        {
            set => Latitude = value;
        }

        [JsonProperty("lng")]
        private double Lng
        {
            set => Longitude = value;
        }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }
}
