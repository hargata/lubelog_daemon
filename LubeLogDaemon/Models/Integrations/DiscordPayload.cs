using System.Text.Json.Serialization;

namespace LubeLogDaemon.Models.Integrations
{
    public class DiscordPayload
    {
        public string Username { get { return "LubeLogger"; } }
        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get { return "https://hargata.github.io/hargata/lubelogger_logo_small.png"; } }
        public string Content { get; set; } = string.Empty;
        [JsonPropertyName("embeds")]
        public List<DiscordPayloadEmbed> Embeds { get; set; } = new List<DiscordPayloadEmbed>();
    }
    public class DiscordPayloadEmbed
    {
        [JsonPropertyName("author")]
        public DiscordPayloadEmbedAuthor Author { get; set; } = new DiscordPayloadEmbedAuthor();
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;
    }
    public class DiscordPayloadEmbedAuthor
    {
        [JsonPropertyName("name")]
        public string Name { get { return "LubeLogger"; } }
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
        [JsonPropertyName("icon_url")]
        public string IconUrl { get { return "https://hargata.github.io/hargata/lubelogger_logo_small.png"; } }
    }
}