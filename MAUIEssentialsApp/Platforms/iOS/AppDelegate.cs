using FirebaseEssentials.iOS;
using FirebaseEssentials.Shared;
using Foundation;
using MAUIEssentials.AppCode.Helpers;
using UIKit;

namespace MAUIEssentialsApp;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
	{
		ScreenSize.ScreenHeight = UIScreen.MainScreen.Bounds.Height;
		ScreenSize.ScreenWidth = UIScreen.MainScreen.Bounds.Width;

		CrossFirebaseEssentials.Notifications = new FirebasePushNotificationManager();
		CrossFirebaseEssentials.Analytics = new FirebaseAnalyticsManager();
		CrossFirebaseEssentials.Crashlytics = new FirebaseCrashlyticsManager();

		FirebaseEssentialsManager.Initialize(application, launchOptions);

		return base.FinishedLaunching(application, launchOptions);
	}

    [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
    public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
    {
        FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
    }

    [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
    public void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
    {
        FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
    }
    // To receive notifications in foregroung on iOS 9 and below.
    // To receive notifications in background in any iOS version
    [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
    public void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
    {
        // If you are receiving a notification message while your app is in the background,
        // this callback will not be fired 'till the user taps on the notification launching the application.

        // If you disable method swizzling, you'll need to call this method. 
        // This lets FCM track message delivery and analytics, which is performed
        // automatically with method swizzling enabled.
        FirebasePushNotificationManager.DidReceiveMessage(userInfo);
        // Do your magic to handle the notification data
        Console.WriteLine(userInfo);
        completionHandler(UIBackgroundFetchResult.NewData);
    }

    public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
    {
        if (url == null || url.AbsoluteUrl == null)
        {
            return true;
        }
        else
        {
            var absoluteUrl = url.AbsoluteUrl.ToString().ToLower();
            //App.Instance.OpenAppLink(new Uri(absoluteUrl));
            // App.Current?.SendOnAppLinkRequestReceived(new Uri(absoluteUrl));
        }
        return true;
    }
}
