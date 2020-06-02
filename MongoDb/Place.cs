using LocationData.CosmosSql;
using LocationData.Google;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using Newtonsoft.Json;

namespace LocationData.MongoDb
{
    public abstract class Place : Place<GeoJsonPoint<GeoJson2DGeographicCoordinates>>, IGoogleLocation
    {
        [BsonIgnore]
        public Location GoogleLocation
        {
            get => new Location(Location.Coordinates.Longitude, Location.Coordinates.Latitude);
            set
            {
                var lon = value.Longitude;
                var lat = value.Latitude;

                var loc = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(lon, lat));

                Location = loc;
            }
        }

        [JsonProperty("location")]
        public override GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; }
    }
}
