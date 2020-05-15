using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using Newtonsoft.Json;

namespace LocationData.MongoDb
{
    public class Place : LocationData.Place
    {
        [BsonIgnore]
        public override GeoPoint Location
        {
            get => new GeoPoint(BackingLocation.Coordinates.Longitude,
                                BackingLocation.Coordinates.Latitude);
            set =>
                BackingLocation = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(value.Longitude, value.Latitude));
        }

        [JsonProperty("location")]
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> BackingLocation { get; set; }
    }
}
