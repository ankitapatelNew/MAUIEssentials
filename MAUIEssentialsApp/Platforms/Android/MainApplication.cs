using Android.App;
using Android.OS;
using Android.Runtime;
using Firebase;
using FirebaseEssentials.Android;
using FirebaseEssentials.Platforms.Android;
using FirebaseEssentials.Shared;

namespace MAUIEssentialsApp;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
		// Fix for directory creation race condition
		Task.Run(() => {
            var overrideDir = new Java.IO.File(Path.Combine(FilesDir.AbsolutePath, ".__override__", "arm64-v8a"));
            if (!overrideDir.Exists())
            {
                overrideDir.Mkdirs();
            }
        }).Wait(1000); // Wait max 1 second 
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override void OnCreate()
    {
		try
		{
			base.OnCreate();
			
			CrossFirebaseEssentials.Analytics = new FirebaseAnalyticsManager();
			CrossFirebaseEssentials.Crashlytics = new FirebaseCrashlyticsManager();
			CrossFirebaseEssentials.Notifications = new FirebasePushNotificationManager();

			//Set the default notification channel for your app when running Android Oreo
			if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
				{
					//Change for your default notification channel id here
					FirebasePushNotificationManager.DefaultNotificationChannelId = "DefaultChannel";

					//Change for your default notification channel name here
					FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
				}
			FirebasePushNotificationManager.DefaultNotificationChannelImportance = NotificationImportance.Max;

			FirebaseApp.InitializeApp(this);
			FirebaseEssentialsManager.Initialize(this);

			//Handle notification when app is closed here
			CrossFirebaseEssentials.Notifications.OnNotificationReceived += (s, p) =>
			{
				System.Diagnostics.Debug.WriteLine("Notification: " + p.Data);
			};
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
    }
}
