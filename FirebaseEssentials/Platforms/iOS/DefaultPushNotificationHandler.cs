using System.Diagnostics;
using FirebaseEssentials.Shared;

namespace FirebaseEssentials.iOS
{
    public class DefaultPushNotificationHandler : IPushNotificationHandler
    {
        public const string DomainTag = "DefaultPushNotificationHandler";

        public void OnError(string error)
        {
            Debug.WriteLine($"{DomainTag} - OnError - {error}");
        }

        public void OnOpened(NotificationResponse response)
        {
            Debug.WriteLine($"{DomainTag} - OnOpened");
        }

        public void OnReceived(IDictionary<string, object> parameters)
        {
            Debug.WriteLine($"{DomainTag} - OnReceived");
        }
    }
}
