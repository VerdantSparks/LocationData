namespace LocationData {
    public interface IProximityQueryResult : IPlace
    {
        double Distance { get; set; }
    }
}
