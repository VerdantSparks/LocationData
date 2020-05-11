using Microsoft.Azure.Cosmos.Spatial;
using Newtonsoft.Json;

namespace LocationData.CosmosSql
{
    public class Place : LocationData.Place
    {
        [JsonProperty("location")]
        public Point Location { get; set; }
    }

    public class ProximityQueryResult : Place
    {
        [JsonProperty("distance")]
        public double Distance { get; set; }
    }
}
