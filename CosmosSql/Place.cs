using Microsoft.Azure.Cosmos.Spatial;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace LocationData.CosmosSql
{
    public class Place : LocationData.Place
    {
        [BsonIgnore]
        public override GeoPoint Location
        {
            get => new GeoPoint(BackingLocation.Position.Longitude, BackingLocation.Position.Latitude);
            set => BackingLocation = new Point(value.Longitude, value.Latitude);
        }


        [JsonProperty("location")]
        protected Point BackingLocation { get; set; }
    }
}
