namespace FirebaseEssentials.Shared
{
    public static class CrossFirebaseEssentials
	{
		public static IFirebasePushNotification Notifications { get; set; }

		public static IFirebaseAnalytics Analytics { get; set; }

		public static IFirebaseCrashlytics Crashlytics { get; set; }

		public static IFirebaseAuth Authentication { get; set; }

        public const string StatusKey = "status";

        public const string DateTimeAffectedForSyncKey = "datetimeaffectedforsync";

        public const string SyncVersionKey = "syncversion";

        public const string SyncToOwnersValue = "synctoowners";
    }
}
