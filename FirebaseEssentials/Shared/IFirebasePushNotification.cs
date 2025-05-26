using System;

namespace FirebaseEssentials.Shared
{
    public enum FirebasePushNotificationErrorType
	{
		Unknown,
		PermissionDenied,
		RegistrationFailed,
		UnregistrationFailed,
		Success
	}

	public delegate void FirebasePushNotificationTokenEventHandler(object source, FirebasePushNotificationTokenEventArgs e);

	public class FirebasePushNotificationTokenEventArgs : EventArgs
	{
		public string Token { get; }
		public bool IsNewToken { get; }

		public FirebasePushNotificationTokenEventArgs(string token, bool isNewToken)
		{
			Token = token;
			IsNewToken = isNewToken;
		}
	}

	public delegate void FirebasePushNotificationErrorEventHandler(object source, FirebasePushNotificationErrorEventArgs e);

	public class FirebasePushNotificationErrorEventArgs : EventArgs
	{
		public FirebasePushNotificationErrorType Type;
		public string Message { get; }

		public FirebasePushNotificationErrorEventArgs(FirebasePushNotificationErrorType type, string message)
		{
			Type = type;
			Message = message;
		}
	}

	public delegate void FirebasePushNotificationDataEventHandler(object source, FirebasePushNotificationDataEventArgs e);

	public class FirebasePushNotificationDataEventArgs : EventArgs
	{
		public IDictionary<string, object> Data { get; }

		public FirebasePushNotificationDataEventArgs(IDictionary<string, object> data)
		{
			Data = data;
		}
	}

	public delegate void FirebasePushNotificationResponseEventHandler(object source, FirebasePushNotificationResponseEventArgs e);

	public class FirebasePushNotificationResponseEventArgs : EventArgs
	{
		public string Identifier { get; }

		public IDictionary<string, object> Data { get; }

		public NotificationCategoryType Type { get; }

		public FirebasePushNotificationResponseEventArgs(IDictionary<string, object> data, string identifier = "", NotificationCategoryType type = NotificationCategoryType.Default)
		{
			Identifier = identifier;
			Data = data;
			Type = type;
		}
	}

	public interface IFirebasePushNotification
	{
		NotificationUserCategory[] GetUserNotificationCategories();

		string[] SubscribedTopics { get; }

		void Subscribe(string[] topics);

		void Subscribe(string topic);

		void Unsubscribe(string topic);

		void Unsubscribe(string[] topics);

		void UnsubscribeAll();

		void RegisterForPushNotifications();

		void UnregisterForPushNotifications();

		IPushNotificationHandler NotificationHandler { get; set; }

		event FirebasePushNotificationTokenEventHandler OnTokenRefresh;

		event FirebasePushNotificationResponseEventHandler OnNotificationOpened;

		event FirebasePushNotificationResponseEventHandler OnNotificationAction;

		event FirebasePushNotificationDataEventHandler OnNotificationReceived;

		event FirebasePushNotificationDataEventHandler OnNotificationDeleted;

		event FirebasePushNotificationErrorEventHandler OnNotificationStatus;

		string Token { get; }

		void SendDeviceGroupMessage(IDictionary<string, string> parameters, string groupKey, string messageId, int timeOfLive);

		void ClearAllNotifications();

		void RemoveNotification(int id);

		void RemoveNotification(string tag, int id);

		Task<string> GetTokenAsync();

		bool CanShowNotification { get; set; }

		bool NotificationPermission { get; set; }
	}
}
