using System.Text.Json.Serialization;

namespace LubeLogDaemon.Models.Integrations
{
    public class ApprisePayload
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;
        [JsonPropertyName("type")]
        public string Type { get
            {
                switch (Priority)
                {
                    case 1:
                    default:
                        return "info";
                    case 2:
                        return "success";
                    case 3:
                        return "warning";
                    case 4:
                        return "failure";
                }
            } }
        public int Priority { get; set; }
    }
}
