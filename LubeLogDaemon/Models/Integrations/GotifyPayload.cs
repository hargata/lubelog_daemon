using System.Text.Json.Serialization;

namespace LubeLogDaemon.Models.Integrations
{
    public class GotifyPayload
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyName("priority")]
        public int Priority { get; set; }
        [JsonPropertyName("extras")]
        public GotifyPayloadExtra PayloadExtra { get; set; } = new GotifyPayloadExtra();
    }
    public class GotifyPayloadExtra
    {
        [JsonPropertyName("client::notification")]
        public GotifyNotificationExtra NotificationExtras { get; set; } = new GotifyNotificationExtra();
    }
    public class GotifyNotificationExtra
    {
        [JsonPropertyName("click")]
        public GotifyURL NotificationUrl { get; set; } = new GotifyURL();
    }
    public class GotifyURL
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}