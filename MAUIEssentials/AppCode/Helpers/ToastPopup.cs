using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace MAUIEssentials.AppCode.Helpers
{
    public static class ToastPopup
	{
        private static IToastPopUp? instance;
		public static IToastPopUp Instance {
			get {
                if(instance == null)
                    instance = new ToastImplementation();
                return instance;
			}
		}
	}

	public interface IToastPopUp
	{
		void ShowMessage(string message, ToastDuration toastDuration = ToastDuration.Short, Action callback = null);
	}

    public class ToastImplementation : IToastPopUp
    {
        public async void ShowMessage(string message, ToastDuration toastDuration, Action callback = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            var pages = await NavigationServices.GetAllPagesInStack();
            if (pages.Any() || Shell.Current != null)
            {
                try
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        try
                        {
                            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                            var toast = Toast.Make(message, toastDuration, 14);
                            await toast.Show(cancellationTokenSource.Token);
                        }
                        catch (Exception ex)
                        {
                            ex.LogException();
                        }
                    });
                    callback?.Invoke();
                }
                catch (Exception ex)
                {
                    ex.LogException();
                }
            }
        }
    }
}