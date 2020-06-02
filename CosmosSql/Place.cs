using LocationData.Google;
using Microsoft.Azure.Cosmos.Spatial;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace LocationData.CosmosSql
{
    public abstract class Place : Place<Point>, IGoogleLocation
    {
        [BsonIgnore]
        public Location GoogleLocation
        {
            get => new Location(Location.Position.Longitude, Location.Position.Latitude);
            set => Location = new Point(value.Longitude, value.Latitude);
        }

        [JsonProperty("location")]
        public override Point Location { get; set; }
    }
}
