namespace LubeLogDaemon.Models
{
    public class DaemonConfig
    {
        public string InstanceUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public List<string> UrgenciesTracked { get; set; } = new List<string>();
        public List<string> WebHookForwards { get; set; } = new List<string>();
        public List<NotificationConfig> NotificationConfigs { get; set; } = new List<NotificationConfig>();
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
        public NotificationType ConfigurationType { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string InstanceUrl { get; set; } = string.Empty;
        public List<int> Priorities { get; set; } = new List<int>();
        public bool TitleInHeader { get; set; }
        public string TitleHeader { get; set; } = string.Empty;
        public bool PriorityInHeader { get; set; }
        public string PriorityHeader { get; set; } = string.Empty;
    }
}