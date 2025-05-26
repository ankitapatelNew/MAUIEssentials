using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials;

[Activity(
    Exported = true,
    Theme = "@style/Maui.SplashTheme",
    LaunchMode = LaunchMode.SingleTop,
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public event Action<int, Result, Intent> ActivityResult;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        try
        {
            base.OnCreate(savedInstanceState);

            Platform.Init(this, savedInstanceState);

            ScreenSize.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);
            ScreenSize.ScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);

            RequestedOrientation = ScreenOrientation.Portrait;
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        try
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        try
        {
            base.OnActivityResult(requestCode, resultCode, data);
            ActivityResult?.Invoke(requestCode, resultCode, data);
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }

    public override void OnBackPressed()
    {
        try
        {
            base.OnBackPressed();
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }

    protected override void OnNewIntent(Intent? intent)
    {
        try
        {
            base.OnNewIntent(intent);
            string? action = intent?.Action;
            string? strLink = intent?.DataString;

            if (Intent.ActionView != action || string.IsNullOrWhiteSpace(strLink))
                return;

            var link = new Uri(strLink);
            App.Current?.SendOnAppLinkRequestReceived(link);
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }

    public override void OnTrimMemory([GeneratedEnum] TrimMemory level)
    {
        try
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            base.OnTrimMemory(level);
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }
}
