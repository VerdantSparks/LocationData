namespace LocationData.Petopia
{
    public class VetHospital : MongoDb.Place
    {
        public string Doctor { get; set; }
        public string Phone { get; set; }
        public string LicenseNumber { get; set; }
    }
}
