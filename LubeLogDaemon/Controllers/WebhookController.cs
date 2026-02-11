using LubeLogDaemon.External;
using LubeLogDaemon.Logic;
using LubeLogDaemon.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LubeLogDaemon.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private ICachedReminders _reminderCache;
        private IWebHookLogic _logic;
        private ILogger<WebhookController> _logger;
        public WebhookController(ICachedReminders reminderCache, IWebHookLogic logic, ILogger<WebhookController> logger)
        {
            _reminderCache = reminderCache;
            _logic = logic;
            _logger = logger;
        }
        [Route("ingest")]
        [HttpPost]
        public async Task<IActionResult> Ingest(WebHookPayload payload)
        {
            var stringifiedPayload = JsonSerializer.Serialize(payload);
            await _logic.RefreshReminders();
            await _logic.ForwardWebHookPayload(payload);
            _logger.LogInformation(stringifiedPayload);
            return Ok();
        }
        [Route("reminders")]
        [HttpGet]
        public List<Reminder> Reminders()
        {
            return _reminderCache.GetReminders();
        }
        [Route("reminders/clear")]
        [HttpDelete]
        public IActionResult ClearReminders()
        {
            _reminderCache.ClearReminders();
            return Ok();
        }
        [Route("config")]
        [HttpPost]
        public IActionResult SaveConfig(DaemonConfig config)
        {
            if (_logic.WriteDaemonConfig(config))
            {
                return Ok();
            }
            return Problem();
        }
    }
}