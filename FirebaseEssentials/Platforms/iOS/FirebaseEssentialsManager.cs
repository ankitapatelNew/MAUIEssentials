using Firebase.Core;
using Firebase.Crashlytics;
using FirebaseEssentials.Shared;
using Foundation;
using UIKit;

namespace FirebaseEssentials.iOS
{
    public static class FirebaseEssentialsManager
	{
		public static void Initialize(UIApplication app, NSDictionary options)
		{
			if (App.DefaultInstance == null) {
				App.Configure();
			}

			if (CrossFirebaseEssentials.Notifications != null) {
				FirebasePushNotificationManager.Initialize(options);
			}

			if (CrossFirebaseEssentials.Crashlytics != null) {
				Crashlytics.SharedInstance.Init();
			}
		}
	}
}
