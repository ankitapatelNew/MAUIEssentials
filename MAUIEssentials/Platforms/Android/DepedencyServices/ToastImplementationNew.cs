using CommunityToolkit.Maui.Core;
using FirebaseEssentials.Shared;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.Platforms.Android.DepedencyServices
{
    public class ToastImplementationNew : DisposableBase, IToastPopUp
    {
        private static global::Android.Widget.Toast? toast;

        public void ShowMessage(string message, ToastDuration toastLength = ToastDuration.Short, Action callback = null)
        {
            if (string.IsNullOrEmpty(message) || !App.IsRunning)
            {
                return;
            }

            var context = Platform.AppContext;
            var activity = Platform.CurrentActivity;

            var length = toastLength == ToastDuration.Short ? global::Android.Widget.ToastLength.Short : global::Android.Widget.ToastLength.Long;
            var delay = length == global::Android.Widget.ToastLength.Long ? TimeSpan.FromSeconds(3.5) : TimeSpan.FromSeconds(2);

            activity?.RunOnUiThread(() =>
            {
                // To dismiss existing toast, otherwise, the screen will be populated with it if the user do so
                toast?.Cancel();

                Thread thread = new Thread(() =>
                {
                    Thread.Sleep(delay);

                    activity.RunOnUiThread(() =>
                    {
                        callback?.Invoke();
                    });
                });

                toast = global::Android.Widget.Toast.MakeText(context, message, length);
                toast?.Show();
                thread.Start();
            });
        }
    }
}
