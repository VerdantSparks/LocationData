namespace LocationData {
    public class GeoPoint
    {
        public readonly double Latitude;
        public readonly double Longitude;
        public GeoPoint(double lon, double lat)
        {
            Longitude = lon;
            Latitude = lat;
        }
    }
}
