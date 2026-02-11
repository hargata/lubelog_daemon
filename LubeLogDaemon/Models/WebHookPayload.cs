namespace LubeLogDaemon.Models
{
    public class WebHookPayload
    {
        public string Type { get; set; } = "";
        public string Timestamp
        {
            get { return DateTime.UtcNow.ToString("O"); }
        }
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// Legacy attributes below
        /// </summary>
        public string VehicleId { get; set; } = "";
        public string Username { get; set; } = "";
        public string Action { get; set; } = "";
    }
}
