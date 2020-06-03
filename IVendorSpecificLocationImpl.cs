namespace LocationData
{
    public interface IVendorSpecificLocationImpl<T>
    {
        T Location { get; set; }
    }
}
