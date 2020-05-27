namespace LocationData
{
    public interface IPlace
    {
        string Id { get; set; }
        string Name { get; set; }
        string Address { get; set; }
        string GooglePlaceId { get; set; }
        string City { get; set; }
        string Description { get; set; }
        GeoPoint Location { get; set; }
    }
}

