namespace LocationData.Google
{
    public interface IGoogleLocation
    {
        Location GoogleLocation { get; set; }
        string GooglePlaceId { get; set; }
    }
}
