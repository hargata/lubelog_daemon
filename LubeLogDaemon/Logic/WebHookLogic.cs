using LubeLogDaemon.External;
using LubeLogDaemon.Models;
using LubeLogDaemon.Models.Integrations;
using System.Text;
using System.Text.Json;

namespace LubeLogDaemon.Logic
{
    public interface IWebHookLogic
    {
        Task RefreshReminders();
        Task ForwardWebHookPayload(WebHookPayload payload);
        bool WriteDaemonConfig(DaemonConfig inputConfig);
    }
    public class WebHookLogic: IWebHookLogic
    {
        private ICachedReminders _reminderCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private string _lubeloggerUrl;
        private string _apiKey;
        private List<string> _webHookForwards;
        private List<string> _urgenciesTracked;
        private List<NotificationConfig> _notificationConfigs;
        private ILogger<WebHookLogic> _logger;
        public WebHookLogic(ICachedReminders reminderCache, IHttpClientFactory httpClientFactory, IConfiguration _config, ILogger<WebHookLogic> logger)
        {
            _reminderCache = reminderCache;
            _httpClientFactory = httpClientFactory;
            _lubeloggerUrl = _config[nameof(DaemonConfig.InstanceUrl)] ?? string.Empty;
            _apiKey = _config[nameof(DaemonConfig.ApiKey)] ?? string.Empty;
            _webHookForwards = _config.GetSection(nameof(DaemonConfig.WebHookForwards)).Get<List<string>>() ?? new List<string>();
            _urgenciesTracked = _config.GetSection(nameof(DaemonConfig.UrgenciesTracked)).Get<List<string>>() ?? new List<string>();
            _notificationConfigs = _config.GetSection(nameof(DaemonConfig.NotificationConfigs)).Get<List<NotificationConfig>>() ?? new List<NotificationConfig>();
            _logger = logger;
        }
        public async Task RefreshReminders()
        {
            //get reminders from LubeLogger
            var httpClient = _httpClientFactory.CreateClient();
            var result = await httpClient.GetFromJsonAsync<List<Reminder>>($"{_lubeloggerUrl}/api/vehicle/reminders/all?apiKey={_apiKey}");
            if (result != null)
            {
                foreach (Reminder reminder in result)
                {
                    if (!_reminderCache.CheckIfReminderExists(reminder) && _urgenciesTracked.Contains(reminder.Urgency.ToLower()))
                    {
                        await SendReminderNotification(reminder);
                        _reminderCache.AddReminder(reminder);
                    }
                }
            }
        }
        private async Task SendReminderNotification(Reminder reminder)
        {
            _logger.LogInformation($"Send Notification for Id {reminder.Id}");
            var httpClient = _httpClientFactory.CreateClient();
            var vehicleInfo = await httpClient.GetFromJsonAsync<List<VehicleInfo>>($"{_lubeloggerUrl}/api/vehicle/info?apiKey={_apiKey}&vehicleId={reminder.VehicleId}");
            var notificationTitle = string.Empty;
            if (vehicleInfo != null) {
                var vehicleData = vehicleInfo.First().VehicleData;
                notificationTitle = $"{vehicleData.Year} {vehicleData.Make} {vehicleData.Model} ({GetVehicleIdentifier(vehicleData)})";
            }
            foreach (NotificationConfig notificationConfig in _notificationConfigs)
            {
                switch (notificationConfig.ConfigurationType)
                {
                    case NotificationType.Email:
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, $"{_lubeloggerUrl}/api/vehicle/reminders/send?apiKey={_apiKey}&id={reminder.Id}");
                            await httpClient.SendAsync(request);
                        }
                    break;
                    case NotificationType.WebHook:
                        {
                            int priority = 0;
                            if (notificationConfig.Priorities.Count() == _urgenciesTracked.Count())
                            {
                                //map priorities to urgencies
                                int urgencyIndex = _urgenciesTracked.IndexOf(reminder.Urgency.ToLower());
                                priority = notificationConfig.Priorities[urgencyIndex];
                            }
                            var request = new HttpRequestMessage(HttpMethod.Post, notificationConfig.InstanceUrl.Replace("{vehicleId}", reminder.VehicleId.ToString()));
                            ReminderWebHook reminderPayload = new ReminderWebHook
                            {
                                Id = reminder.Id,
                                VehicleId = reminder.VehicleId,
                                Description = reminder.Description,
                                Urgency = reminder.Urgency,
                                Title = notificationTitle,
                                Priority = priority
                            };
                            if (notificationConfig.Headers.Any())
                            {
                                foreach (var header in notificationConfig.Headers)
                                {
                                    request.Headers.Add(header.Key, header.Value);
                                }
                            }
                            if (notificationConfig.TitleInHeader)
                            {
                                request.Headers.Add(notificationConfig.TitleHeader, notificationTitle);
                            }
                            if (notificationConfig.PriorityInHeader)
                            {
                                request.Headers.Add(notificationConfig.PriorityHeader, priority.ToString());
                            }
                            request.Content = new StringContent(JsonSerializer.Serialize(reminderPayload), Encoding.UTF8, "application/json");
                            await httpClient.SendAsync(request);
                        }
                    break;
                    case NotificationType.Ntfy:
                        {
                            int priority = 0;
                            if (notificationConfig.Priorities.Count() == _urgenciesTracked.Count())
                            {
                                //map priorities to urgencies
                                int urgencyIndex = _urgenciesTracked.IndexOf(reminder.Urgency.ToLower());
                                priority = notificationConfig.Priorities[urgencyIndex];
                            }
                            var request = new HttpRequestMessage(HttpMethod.Post, notificationConfig.InstanceUrl.Replace("{vehicleId}", reminder.VehicleId.ToString()));
                            if (notificationConfig.Headers.Any())
                            {
                                foreach (var header in notificationConfig.Headers)
                                {
                                    request.Headers.Add(header.Key, header.Value);
                                }
                            }
                            if (notificationConfig.TitleInHeader)
                            {
                                request.Headers.Add(notificationConfig.TitleHeader, notificationTitle);
                            }
                            if (notificationConfig.PriorityInHeader)
                            {
                                request.Headers.Add(notificationConfig.PriorityHeader, priority.ToString());
                            }
                            request.Headers.Add("Click", $"{_lubeloggerUrl}/Vehicle/Index?vehicleId={reminder.VehicleId}&tab=reminder");
                            request.Content = new StringContent(GetOneLineReminderDescription(reminder), Encoding.UTF8, "text/plain");
                            await httpClient.SendAsync(request);
                        }
                    break;
                    case NotificationType.Gotify:
                        {
                            int priority = 0;
                            if (notificationConfig.Priorities.Count() == _urgenciesTracked.Count())
                            {
                                //map priorities to urgencies
                                int urgencyIndex = _urgenciesTracked.IndexOf(reminder.Urgency.ToLower());
                                priority = notificationConfig.Priorities[urgencyIndex];
                            }
                            var request = new HttpRequestMessage(HttpMethod.Post, notificationConfig.InstanceUrl.Replace("{vehicleId}", reminder.VehicleId.ToString()));
                            if (notificationConfig.Headers.Any())
                            {
                                foreach (var header in notificationConfig.Headers)
                                {
                                    request.Headers.Add(header.Key, header.Value);
                                }
                            }
                            if (notificationConfig.TitleInHeader)
                            {
                                request.Headers.Add(notificationConfig.TitleHeader, notificationTitle);
                            }
                            if (notificationConfig.PriorityInHeader)
                            {
                                request.Headers.Add(notificationConfig.PriorityHeader, priority.ToString());
                            }
                            var reminderPayload = new GotifyPayload
                            {
                                Title = notificationTitle,
                                Message = GetOneLineReminderDescription(reminder),
                                Priority = priority,
                                PayloadExtra = new GotifyPayloadExtra { 
                                    NotificationExtras = new GotifyNotificationExtra {  
                                        NotificationUrl = new GotifyURL { 
                                            Url = $"{_lubeloggerUrl}/Vehicle/Index?vehicleId={reminder.VehicleId}&tab=reminder"
                                        } 
                                    } 
                                }
                            };
                            request.Content = new StringContent(JsonSerializer.Serialize(reminderPayload), Encoding.UTF8, "application/json");
                            await httpClient.SendAsync(request);
                        }
                    break;
                    case NotificationType.Discord: 
                        {
                            var request = new HttpRequestMessage(HttpMethod.Post, notificationConfig.InstanceUrl);
                            var reminderPayload = new DiscordPayload
                            {
                                Content = GetOneLineReminderDescription(reminder),
                                Embeds = new List<DiscordPayloadEmbed>
                                {
                                    new DiscordPayloadEmbed
                                    {
                                        Author = new DiscordPayloadEmbedAuthor
                                        {
                                            Url = $"{_lubeloggerUrl}"
                                        },
                                        Title = notificationTitle,
                                        Url = $"{_lubeloggerUrl}/Vehicle/Index?vehicleId={reminder.VehicleId}&tab=reminder",
                                        Description = GetOneLineReminderDescription(reminder),
                                        Color = GetDiscordColorByReminder(reminder)
                                    }
                                }
                            };
                            request.Content = new StringContent(JsonSerializer.Serialize(reminderPayload), Encoding.UTF8, "application/json");
                            await httpClient.SendAsync(request);
                        }
                    break;
                }
            }
        }
        public async Task ForwardWebHookPayload(WebHookPayload payload)
        {
            var httpClient = _httpClientFactory.CreateClient();
            foreach(string webHookForward in _webHookForwards)
            {
                if (!string.IsNullOrWhiteSpace(webHookForward))
                {
                    await httpClient.PostAsJsonAsync(webHookForward, payload);
                }
            }
        }
        private string GetVehicleIdentifier(Vehicle vehicle)
        {
            if (vehicle.VehicleIdentifier == "LicensePlate")
            {
                return vehicle.LicensePlate;
            }
            else
            {
                if (vehicle.ExtraFields.Any(x => x.Name == vehicle.VehicleIdentifier))
                {
                    return vehicle.ExtraFields?.FirstOrDefault(x => x.Name == vehicle.VehicleIdentifier)?.Value ?? "N/A";
                }
                else
                {
                    return "N/A";
                }
            }
        }
        private string GetOneLineReminderDescription(Reminder reminder)
        {
            switch (reminder.Urgency.ToLower())
            {
                case "noturgent":
                    return $"Not Urgent - {reminder.Description}";
                case "urgent":                               
                    return $"Urgent - {reminder.Description}";
                case "veryurgent":                           
                    return $"Very Urgent - {reminder.Description}";
                case "pastdue":                              
                    return $"Past Due - {reminder.Description}";
            }
            return string.Empty;
        }
        private string GetDiscordColorByReminder(Reminder reminder)
        {
            switch (reminder.Urgency.ToLower())
            {
                case "noturgent":
                    return "1673044";
                case "urgent":
                    return "16761095";
                case "veryurgent":
                    return "14431557";
                case "pastdue":
                    return "7107965";
            }
            return string.Empty;
        }
        public bool WriteDaemonConfig(DaemonConfig inputConfig)
        {
            try
            {
                if (!Directory.Exists("config"))
                {
                    Directory.CreateDirectory("config");
                }
                File.WriteAllText("config/daemonConfig.json", JsonSerializer.Serialize(inputConfig));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return false;
        }
    }
}