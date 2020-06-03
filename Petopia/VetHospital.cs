using Newtonsoft.Json;

namespace LocationData.Petopia
{
    public class VetHospital : Place
    {
        [JsonProperty("doctor")]
        public string Doctor { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("license_number")]
        public string LicenseNumber { get; set; }
    }
}
