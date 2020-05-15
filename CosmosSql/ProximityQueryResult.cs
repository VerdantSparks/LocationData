using Newtonsoft.Json;

namespace LocationData.CosmosSql {
    public class ProximityQueryResult : Place, IProximityQueryResult
    {
        [JsonProperty("distance")]
        public double Distance { get; set; }
    }
}
