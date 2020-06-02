using LocationData.Google;
using Microsoft.Azure.Cosmos.Spatial;
using MongoDB.Bson.Serialization.Attributes;

namespace LocationData.Petopia
{
    public class GoogleCosmosVetHospital : GoogleVetHospital<Point>
    {
        [BsonIgnore]
        public override Location GoogleLocation
        {
            get => new Location(Location.Position.Longitude, Location.Position.Latitude);
            set => Location = new Point(value.Longitude, value.Latitude);
        }
    }
}
