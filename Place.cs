using LocationData.Google;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace LocationData {
    public abstract class Place : IPlace
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        public string GooglePlaceId { get; set; }
        public string City { get; set; }
        public string Description { get; set; }

        public virtual Location Location { get; set; }
    }
}
