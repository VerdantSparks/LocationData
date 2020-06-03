using LocationData.Google;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace LocationData.Petopia
{
    public class GoogleMongoVetHospital : GoogleVetHospital<GeoJsonPoint<GeoJson2DGeographicCoordinates>>
    {
        [BsonIgnore]
        public override Location GoogleLocation
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
    }
}
