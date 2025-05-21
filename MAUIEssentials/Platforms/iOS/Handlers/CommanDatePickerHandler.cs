namespace MAUIEssentials.Platforms.iOS.Handlers
{
    public class CommanDatePickerHandler : DatePickerHandler
    {
        protected override MauiDatePicker CreatePlatformView()
        {
            var platformView = base.CreatePlatformView();

            try
            {
                var customDatePicker = VirtualView as CustomDatePicker;
                if (customDatePicker != null)
                {
                    if (!customDatePicker.IsBorder)
                    {
                        HideBorder(platformView);
                    }

                    platformView.Text = customDatePicker.Placeholder;
                    platformView.TextColor = customDatePicker.PlaceholderColor?.ToPlatform() ?? Colors.Gray.ToPlatform();

                    if (UIDevice.CurrentDevice.CheckSystemVersion(13, 2))
                    {
                        if (platformView.InputView is UIDatePicker picker)
                        {
                            picker.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
                        }
                    }

                    platformView.ShouldEndEditing += (UITextField textField) =>
                    {
                        if (customDatePicker == null) return true;

                        var format = !string.IsNullOrEmpty(customDatePicker.Format) ? customDatePicker.Format : "dd/MM/yyyy";

                        // Safe color conversion with null check
                        if (customDatePicker.TextColor != null)
                        {
                            platformView.TextColor = customDatePicker.TextColor.ToPlatform();
                        }

                        if (customDatePicker.Date != null)
                        {
                            platformView.Text = customDatePicker.Date.ToString(format);
                        }

                        customDatePicker.UpdateSelectedDate();
                        return true;
                    };
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }

            return platformView;
        }

        protected override void ConnectHandler(MauiDatePicker platformView)
        {
            try
            {
                base.ConnectHandler(platformView);

                var customDatePicker = VirtualView as CustomDatePicker;
                if (customDatePicker != null && !customDatePicker.IsBorder)
                {
                    HideBorder(platformView);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private static void HideBorder(MauiDatePicker platformView)
        {
            try
            {
                if (platformView != null)
                {
                    platformView.Layer.BorderWidth = 0;
                    platformView.BorderStyle = UITextBorderStyle.None;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }    
}
