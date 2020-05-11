using MongoDB.Driver.GeoJsonObjectModel;
using Newtonsoft.Json;

namespace LocationData.MongoDb
{
    public class Place : LocationData.Place
    {
        [JsonProperty("location")]
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; }
    }

    public class ProximityQueryResult : Place
    {
        [JsonProperty("distance")]
        public double Distance { get; set; }
    }
}
