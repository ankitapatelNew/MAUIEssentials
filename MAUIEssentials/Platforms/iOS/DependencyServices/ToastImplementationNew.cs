using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using FirebaseEssentials.Shared;
using Foundation;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using UIKit;
using Font = Microsoft.Maui.Font;

namespace MAUIEssentials.Platforms.iOS.DependencyServices
{
    public class ToastImplementationNew: DisposableBase, IToastPopUp
    {
        const double LongDelay = 3.5;
        const double ShortDelay = 2.0;

        NSTimer? _lastAlertDelay;
        UIAlertController? _lastAlert;

        public void ShowMessage(string message, ToastDuration toastLength, Action callback = null)
        {

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = AppColorResources.black.ToColor(),
                TextColor = AppColorResources.white.ToColor(),
                CornerRadius = new CornerRadius(15),
                Font = Font.OfSize(Application.Current?.Resources?["FontRegular"]?.ToString()?.ToString(), 14),
            };
            string text = message;
            TimeSpan duration = TimeSpan.FromSeconds(3);
            var snackbar = Snackbar.Make(text, duration: duration, visualOptions: snackbarOptions);
            snackbar.Show();
        }

    }
}
