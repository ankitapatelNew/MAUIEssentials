using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace MAUIEssentials.AppCode.Helpers
{
    public class ClipboardHelper
    {
        public static async Task ShowToastWithClipboard(string text)
        {
            try
            {
                // Copy text to clipboard
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Clipboard.Default.SetTextAsync(text);
                });
                var toast = Toast.Make(text, ToastDuration.Long, 14);
                await toast.Show();
            }
            catch (Exception ex)
            {
                await ShowToastWithClipboard($"ShowToastWithClipboard {ex.LogException}");
                ex.LogException();
            }
        }
    }
}