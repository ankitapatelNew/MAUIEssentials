using Newtonsoft.Json;

namespace FirebaseEssentials.Platforms.Android
{
    public class AppPreferences
    {
        static readonly string NotificationKey = "NotificationKey";
        static readonly string SummaryIdKey = "SummaryIdKey";

        public static void SaveNotification(List<NotificationModel> notifications)
        {
            Preferences.Set(NotificationKey, JsonConvert.SerializeObject(notifications));
        }

        public static string GetNotifications()
        {
            return Preferences.Get(NotificationKey, string.Empty);
        }

        public static int GetSummaryNotifyId()
        {
            var summaryId = Preferences.Get(SummaryIdKey, 0);

            if (summaryId == 0)
            {
                summaryId = new Random().Next(700, 999);
                Preferences.Set(SummaryIdKey, summaryId);
            }
            return summaryId;
        }
    }

    public class NotificationModel
    {
        public int NotifiyId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
