using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using DatePicker = Microsoft.Maui.Controls.DatePicker;
using MAUIEssentials.Platforms.Android.Renderers;
using Microsoft.Maui.Controls.Compatibility;
using Android.Content;
using Microsoft.Maui.Controls.Platform;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]
namespace MAUIEssentials.Platforms.Android.Renderers
{
    public class CustomDatePickerRenderer : DatePickerRenderer
    {
        public CustomDatePickerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            try
            {
                base.OnElementChanged(e);
                if (Control == null || e.NewElement == null)
                {
                    return;
                }

                var element = e.NewElement as CustomDatePicker;

                if (element != null && !element.IsBorder)
                {
                    HideBorder();
                }

                if (element != null)
                {
                    Control.Text = element.Placeholder;
                    Control.SetTextColor(element.PlaceholderColor.ToAndroid());
                }

                Control.TextChanged += (sender, arg) => {
                    var selectedDate = arg.Text.ToString();
                    if (selectedDate == element.Placeholder)
                    {
                        var format = !string.IsNullOrEmpty(element.Format) ? element.Format : "dd/MM/yyyy";
                        Control.Text = element.Date.ToString(format);
                        Control.SetTextColor(element.TextColor.ToAndroid());
                    }
                    element.UpdateSelectedDate();
                };
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                base.OnElementPropertyChanged(sender, e);
                var element = sender as CustomDatePicker;

                if (!element.IsBorder)
                {
                    HideBorder();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void HideBorder()
        {
            try
            {
                if (Control == null)
                {
                    return;
                }
                Control.Background = null;

                var layoutParams = new MarginLayoutParams(Control.LayoutParameters);
                layoutParams.SetMargins(0, 0, 0, 0);
                LayoutParameters = layoutParams;
                Control.LayoutParameters = layoutParams;
                Control.SetPadding(0, 0, 0, 0);
                SetPadding(0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}
