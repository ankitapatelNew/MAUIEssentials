using Foundation;
using MAUIEssentials.AppCode.Helpers;
using UIKit;

namespace MAUIEssentials;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
	{
		ScreenSize.ScreenHeight = UIScreen.MainScreen.Bounds.Height;
		ScreenSize.ScreenWidth = UIScreen.MainScreen.Bounds.Width;

		return base.FinishedLaunching(application, launchOptions);
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
            App.Current?.SendOnAppLinkRequestReceived(new Uri(absoluteUrl));
        }
        return true;
    }
}
