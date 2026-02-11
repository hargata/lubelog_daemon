namespace LubeLogDaemon.Models
{
    public class VehicleInfo
    {
        public Vehicle VehicleData { get; set; } = new Vehicle();
    }
    public class Vehicle
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public List<ExtraField> ExtraFields { get; set; } = new List<ExtraField>();
        public string VehicleIdentifier { get; set; } = "LicensePlate";
    }
    public class ExtraField
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}