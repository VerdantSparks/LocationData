using LocationData.Google;
using MongoDB.Bson.Serialization.Attributes;

namespace LocationData.Petopia
{
    public abstract class GoogleVetHospital<T> : VetHospital<T>, IGoogleLocation
    {
        [BsonIgnore]
        public abstract Location GoogleLocation { get; set; }

        public string GooglePlaceId { get; set; }
    }
}
