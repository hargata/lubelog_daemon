using LubeLogDaemon.Models;

namespace LubeLogDaemon.External
{
    /// <summary>
    /// In Memory Storage for reminders
    /// </summary>
    public interface ICachedReminders
    {
        void AddReminder(Reminder reminder);
        bool CheckIfReminderExists(Reminder reminder);
        void RemoveExpiredReminderIds();
        void ClearReminders();
        List<Reminder> GetReminders();
    }
    public class CachedReminders: ICachedReminders
    {
        private List<Reminder> _cachedReminders { get; set; } = new List<Reminder>();
        public void AddReminder(Reminder reminder)
        {
            if (!CheckIfReminderExists(reminder))
            {
                reminder.DateAdded = DateTime.UtcNow;
                _cachedReminders.Add(reminder);
            }
        }
        public bool CheckIfReminderExists(Reminder reminder)
        {
            RemoveExpiredReminderIds();
            return _cachedReminders.Any(x => x.Id == reminder.Id && x.Urgency == reminder.Urgency);
        }
        public void RemoveExpiredReminderIds()
        {
            //remove any reminders that have been cached for more than 1 day.
            _cachedReminders.RemoveAll(x=> x.DateAdded < DateTime.UtcNow.AddDays(-1));
        }
        public void ClearReminders()
        {
            _cachedReminders = new List<Reminder>();
        }
        public List<Reminder> GetReminders()
        {
            return _cachedReminders;
        }
    }
}
