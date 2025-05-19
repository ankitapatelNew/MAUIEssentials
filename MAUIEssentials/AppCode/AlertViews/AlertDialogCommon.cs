namespace MAUIEssentials.AppCode.AlertViews
{
    public static class AlertDialogCommon
    {
        public static async Task ShowAlert(this INavigation navigation, string title, string message)
        {
            await ShowAlert(navigation, title, message, new AlertConfig());
        }
        public static async Task ShowAlert(this INavigation navigation, string title, string message, string accept)
        {
            await ShowAlert(navigation, title, message, new AlertConfig(), accept);
        }

        public static async Task ShowAlert(this INavigation navigation, string title, string message, string accept, AlertConfig config)
        {
            await ShowAlert(navigation, title, message, config, accept);
        }

        public static async Task ShowAlert(this INavigation navigation, string title, string message, AlertConfig config)
        {
            await ShowAlert(navigation, title, message, config);
        }

        public static async Task<bool> ShowAlert(this INavigation navigation, string title, string message, string accept, string cancel)
        {
            return await ShowAlert(navigation, title, message, new AlertConfig(), accept, cancel);
        }

        public static async Task<bool> ShowAlert(this INavigation navigation, string title, string message, AlertConfig config, string accept = "", string cancel = "")
        {
            var tcs = new TaskCompletionSource<bool>();
            var alertView = new AlertView(title, message, config, accept, cancel);

            alertView.Result += (status) =>
            {
                tcs.SetResult(status);
            };

            await MopupService.Instance.PushAsync(alertView);
            return await tcs.Task;
        }

        public static async Task<string> ShowActionSheet(this INavigation navigation, string title, string cancel, ActionSheetConfig config, params string[] buttons)
        {
            var tcs = new TaskCompletionSource<string>();
            var alertView = new ActionSheetView(title, cancel, buttons, config);

            alertView.Result += (status) =>
            {
                tcs.SetResult(status);
            };
            await MopupService.Instance.PushAsync(alertView);
            return await tcs.Task;
        }

        public static async Task ShowSnackbar(this INavigation navigation, SnackbarConfig config)
        {
            try
            {
                await MopupService.Instance.PushAsync(new SnackbarView(config));
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }

    public class AlertConfig
    {
        public int CornerRadius { get; set; }
        public Thickness Margin { get; set; }

        public Color BackgroundColor { get; set; }
        public Color TitleColor { get; set; }
        public Color MessageColor { get; set; }
        public Color PositiveColor { get; set; }
        public Color PositiveButtonColor { get; set; }
        public Color NegativeColor { get; set; }
        public Color NegativeButtonColor { get; set; }
        public Color SeparatorColor { get; set; }

        public string TitleFontFamily { get; set; }
        public string MessageFontFamily { get; set; }
        public string PositiveButtonFontFamily { get; set; }
        public string NegativeButtonFontFamily { get; set; }

        public AlertConfig()
        {
            // default values here:
            Margin = DeviceInfo.Idiom == DeviceIdiom.Tablet ? new Thickness(100, 0) : new Thickness(40, 0);

            BackgroundColor = Colors.White;
            TitleColor = Colors.Black;
            MessageColor = Colors.Black;
            PositiveColor = "#2196F3".ToColor();
            PositiveButtonColor = Colors.Transparent;
            NegativeColor = "#9E9E9E".ToColor();
            NegativeButtonColor = Colors.Transparent;
            SeparatorColor = "#979797".ToColor();

            TitleFontFamily = string.Empty;
            MessageFontFamily = string.Empty;
            PositiveButtonFontFamily = string.Empty;
            NegativeButtonFontFamily = string.Empty;
        }
    }

    public class ActionSheetConfig
    {
        public Color CancelTextColor { get; set; }
        public Color CancelStartColor { get; set; }
        public Color CancelEndColor { get; set; }

        public Color TitleColor { get; set; }
        public Color ButtonsTextColor { get; set; }
        public Color ButtonsBackgroundColor { get; set; }
        public Color BorderColor { get; set; }

        public string TitleFontFamily { get; set; }
        public string CancelFontFamily { get; set; }
        public string ButtonsFontFamily { get; set; }

        public ActionSheetConfig()
        {
            TitleColor = Colors.Gray;
            BorderColor = "#dedede".ToColor();
            ButtonsTextColor = Colors.DeepSkyBlue;
            CancelTextColor = Colors.DeepSkyBlue;

            ButtonsBackgroundColor = Colors.White;
            CancelStartColor = Colors.White;
            CancelEndColor = Colors.White;

            TitleFontFamily = string.Empty;
            CancelFontFamily = string.Empty;
            ButtonsFontFamily = string.Empty;
        }
    }

    public class SnackbarConfig
    {
        public Color BackgroundColor { get; set; }
        public Color MessageTextColor { get; set; }
        public Color ButtonTextColor { get; set; }

        public string Message { get; set; }
        public string ButtonText { get; set; }

        public string MessageFontFamily { get; set; }
        public string ButtonFontFamily { get; set; }

        public int Timeout { get; set; }
        public double ButtonWidth { get; set; }
        public ICommand ButtonCommand { get; set; }

        public SnackbarConfig()
        {
            BackgroundColor = "#343434".ToColor();
            MessageTextColor = Colors.White;
            ButtonTextColor = "#E91E63".ToColor();

            Message = "This is snackbar";
            ButtonText = "OK";

            MessageFontFamily = string.Empty;
            ButtonFontFamily = string.Empty;
        }
    }
}