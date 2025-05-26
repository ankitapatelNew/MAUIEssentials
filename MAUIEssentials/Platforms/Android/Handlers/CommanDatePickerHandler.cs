using Android.Widget;
using Android.Graphics.Drawables;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using static Android.Views.ViewGroup;
using DatePicker = Microsoft.Maui.Controls.DatePicker;

namespace MAUIEssentials.Platforms.Android.Handlers
{
    public class CommanDatePickerHandler : DatePickerHandler
    {
        public static readonly IPropertyMapper<IDatePicker, CommanDatePickerHandler> Mapper =
            new PropertyMapper<IDatePicker, CommanDatePickerHandler>(DatePickerHandler.Mapper)
            {
                [nameof(CustomDatePicker.Placeholder)] = MapPlaceholder,
                [nameof(CustomDatePicker.PlaceholderColor)] = MapPlaceholderColor,
                [nameof(CustomDatePicker.IsBorder)] = MapIsBorder
            };

        public CommanDatePickerHandler() : base(Mapper) { }

        protected override void ConnectHandler(MauiDatePicker platformView)
        {
            try
            {
                base.ConnectHandler(platformView);

                if (VirtualView is not CustomDatePicker custom || platformView == null)
                    return;

                platformView.Text = custom.Placeholder;
                platformView.SetTextColor(custom.PlaceholderColor.ToAndroid());

                platformView.TextChanged += OnTextChanged;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected override void DisconnectHandler(MauiDatePicker platformView)
        {
            try
            {
                base.DisconnectHandler(platformView);

                platformView.TextChanged -= OnTextChanged;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void OnTextChanged(object? sender, global::Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (VirtualView is not CustomDatePicker custom || PlatformView == null)
                    return;

                var selectedText = e?.Text?.ToString();

                if (selectedText == custom.Placeholder)
                {
                    if (VirtualView is DatePicker basePicker)
                    {
                        var format = !string.IsNullOrEmpty(basePicker.Format) ? basePicker.Format : "dd/MM/yyyy";
                        PlatformView.Text = basePicker.Date.ToString(format);
                        PlatformView.SetTextColor(basePicker.TextColor.ToPlatform());
                    }
                }

                custom.UpdateSelectedDate();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void MapPlaceholder(CommanDatePickerHandler handler, IDatePicker view)
        {
            if (handler.PlatformView is EditText editText && view is CustomDatePicker custom)
            {
                editText.Text = custom.Placeholder;
            }
        }

        public static void MapPlaceholderColor(CommanDatePickerHandler handler, IDatePicker view)
        {
            if (handler.PlatformView is EditText editText && view is CustomDatePicker custom)
            {
                editText.SetTextColor(custom.PlaceholderColor.ToAndroid());
            }
        }

        public static void MapIsBorder(CommanDatePickerHandler handler, IDatePicker view)
        {
            try
            {
                if (handler.PlatformView is not EditText editText || view is not CustomDatePicker custom)
                    return;

                if (!custom.IsBorder)
                {
                    editText.Background = new ColorDrawable(global::Android.Graphics.Color.Transparent);

                    var layoutParams = new MarginLayoutParams(editText.LayoutParameters);
                    layoutParams.SetMargins(0, 0, 0, 0);
                    editText.LayoutParameters = layoutParams;
                    editText.SetPadding(0, 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}
