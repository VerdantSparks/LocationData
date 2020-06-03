using Newtonsoft.Json;

namespace LocationData.Petopia
{
    public class VetHospital<T> : VetHospital, IVendorSpecificLocationImpl<T>
    {
        [JsonProperty("location")]
        public T Location { get; set; }
    }
}
