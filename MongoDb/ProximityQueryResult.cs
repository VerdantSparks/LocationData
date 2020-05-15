using Newtonsoft.Json;

namespace LocationData.MongoDb {
    public class ProximityQueryResult : Place, IProximityQueryResult
    {
        [JsonProperty("distance")]
        public double Distance { get; set; }
    }
}
