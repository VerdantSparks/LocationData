namespace LocationData
{
    public interface IPlaceConverter<T,in T2>
    {
        IPlace<T> Convert(T2 input);
    }
}
