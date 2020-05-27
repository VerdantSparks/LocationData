namespace LocationData
{
    public interface IPlaceConverter<in T>
    {
        IPlace Convert(T input);
    }
}
