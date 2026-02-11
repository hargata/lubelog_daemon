using System.Text.Json.Serialization;

namespace LubeLogDaemon.Models
{
    public class DaemonConfig
    {
        [JsonPropertyName("instanceUrl")]
        public string InstanceUrl { get; set; } = string.Empty;
        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; } = string.Empty;
        [JsonPropertyName("daysToCache")]
        public string DaysToCache { get; set; } = "1";
        [JsonPropertyName("urgenciesTracked")]
        public List<string> UrgenciesTracked { get; set; } = new List<string>();
        [JsonPropertyName("webHookForwards")]
        public List<string> WebHookForwards { get; set; } = new List<string>();
        [JsonPropertyName("notificationConfigs")]
        public List<NotificationConfig> NotificationConfigs { get; set; } = new List<NotificationConfig>();
        [JsonPropertyName("checkDateReminders")]
        public string CheckDateReminders { get; set; } = "False";
        [JsonPropertyName("hourToCheck")]
        public string HourToCheck { get; set; } = "0";
    }
    public enum NotificationType
    {
        Email = 0, //internal lubelogger reminder sender
        WebHook = 1,
        Ntfy = 2,
        Gotify = 3,
        Discord = 4
    }
    public class NotificationConfig
    {
        [JsonPropertyName("configurationType")]
        public NotificationType ConfigurationType { get; set; }
        [JsonPropertyName("headers")]
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        [JsonPropertyName("instanceUrl")]
        public string InstanceUrl { get; set; } = string.Empty;
        [JsonPropertyName("priorities")]
        public List<int> Priorities { get; set; } = new List<int>();
        [JsonPropertyName("titleInHeader")]
        public bool TitleInHeader { get; set; }
        [JsonPropertyName("titleHeader")]
        public string TitleHeader { get; set; } = string.Empty;
        [JsonPropertyName("priorityInHeader")]
        public bool PriorityInHeader { get; set; }
        [JsonPropertyName("priorityHeader")]
        public string PriorityHeader { get; set; } = string.Empty;
    }
}