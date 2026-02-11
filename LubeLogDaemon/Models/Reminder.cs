namespace LubeLogDaemon.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Urgency { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; }
    }
    public class ReminderWebHook: Reminder
    {
        public string Title { get; set; } = string.Empty;
        public int Priority { get; set; }
    }
}
