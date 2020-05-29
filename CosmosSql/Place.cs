using LocationData.Google;
using Microsoft.Azure.Cosmos.Spatial;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace LocationData.CosmosSql
{
    public abstract class Place : LocationData.Place
    {
        [BsonIgnore]
        public override Location Location
        {
            get => new Location(BackingLocation.Position.Longitude, BackingLocation.Position.Latitude);
            set => BackingLocation = new Point(value.Longitude, value.Latitude);
        }


        [JsonProperty("location")]
        protected Point BackingLocation { get; set; }
    }
}
