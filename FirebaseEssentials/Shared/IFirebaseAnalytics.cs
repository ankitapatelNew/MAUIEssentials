namespace FirebaseEssentials.Shared
{
    public interface IFirebaseAnalytics
	{
		void LogEvent(string eventId);

		void LogEvent(string eventId, string paramName, string value);

		void LogEvent(string eventId, IDictionary<string, string> parameters);

		void SetUserId(string userId);

		void TrackScreen(string screenName);
	}
}
