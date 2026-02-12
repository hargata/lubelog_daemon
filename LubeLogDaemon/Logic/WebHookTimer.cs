using LubeLogDaemon.Models;

namespace LubeLogDaemon.Logic
{
    public class WebHookTimer: BackgroundService
    {
        private readonly TimeSpan _period = TimeSpan.FromMinutes(1);
        private TimeSpan _targetTime;
        private readonly ILogger<WebHookTimer> _logger;
        private IWebHookLogic _logic;
        private DateTime _nextRunTime;
        public WebHookTimer(IWebHookLogic logic, IConfiguration _config, ILogger<WebHookTimer> logger)
        {
            _logic = logic;
            _logger = logger;
            var targetHour = int.Parse(_config[nameof(DaemonConfig.HourToCheck)] ?? "0");
            _targetTime = new TimeSpan(targetHour, 0, 0);
            _nextRunTime = DateTime.UtcNow.Date + _targetTime;
            if (DateTime.UtcNow > _nextRunTime)
            {
                _nextRunTime = _nextRunTime.AddDays(1);
            }
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);
            try
            {
                while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
                {
                    _logger.LogInformation($"Current UTC Time: {DateTime.UtcNow.ToString()} Next Run Time: {_nextRunTime.ToString()}");
                    if (DateTime.UtcNow > _nextRunTime)
                    {
                        _nextRunTime = _nextRunTime.AddDays(1);
                        _logger.LogInformation("Refreshing Reminders");
                        await _logic.RefreshReminders();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // This exception is expected when the stoppingToken is canceled
                _logger.LogInformation("Background Task is stopping due to cancellation.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the background task.");
            }
        }
    }
}