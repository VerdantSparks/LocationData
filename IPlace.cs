using LocationData.CosmosSql;

namespace LocationData
{
    public abstract class VetClinic : Place {

    }

    public interface IPlace
    {
        string Id { get; set; }
        string Name { get; set; }
        string Address { get; set; }
        GeoPoint Location { get; set; }
    }
}

