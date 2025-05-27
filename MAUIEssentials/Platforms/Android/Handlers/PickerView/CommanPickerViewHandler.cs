using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.Platforms.Android.Handlers;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using static Android.Views.ViewGroup;
using Font = Microsoft.Maui.Font;

namespace MAUIEssentials.Handlers
{
    public partial class CommanPickerViewHandler : ViewHandler<PickerView, WheelPicker>, IOnValueChangeListener, IOnScrollListener
    {
        protected override WheelPicker CreatePlatformView()
        {
          var weel =  new WheelPicker(Context);
            weel.SetOnValueChangedListener(this);
            weel.SetOnScrollListener(this);
            return weel;
        }
        protected override void ConnectHandler(WheelPicker platformView)
        {
            base.ConnectHandler(platformView);
            {
                //platformView.SetOnValueChangedListener(null);
                //platformView.SetOnScrollListener(null);
                if (!_disposed)
                {
                    var layoutParameters = platformView.LayoutParameters;
                    if (layoutParameters != null)
                    {
                        layoutParameters.Width = LayoutParams.MatchParent;
                        platformView.LayoutParameters = layoutParameters;
                    }
                    else
                    {
                        platformView.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, 200);
                    }

                    platformView.SetTextAlign(WheelItemAlign.Center);
                }
            }
        }
        protected override void DisconnectHandler(WheelPicker platformView)
        {
            _disposed = true;
            platformView.Dispose();
            base.DisconnectHandler(platformView);
        }

        bool _disposed;
        int _selectedIndex;
        public static void UpdateItemsSource(CommanPickerViewHandler handler, PickerView picker)
        {
            int itemCount;

            if (picker.ItemVisibleCount < 3)
            {
                itemCount = 3;
            }
            else
            {
                itemCount = picker.ItemVisibleCount % 2 == 0 ? picker.ItemVisibleCount + 1 : picker.ItemVisibleCount;
            }

            handler?.PlatformView.SetWheelItemCount(itemCount);
            handler?.PlatformView.SetSelectedItemBackgroundColor(picker.SelectedBackgroundColor.ToPlatform());
            handler?.PlatformView.SetTextColor(picker.TextColor.ToPlatform());
            handler?.PlatformView.SetSelectedTextColor(picker.SelectedTextColor.ToPlatform());

            var data = GetData(picker);
            if (data.Any())
            {
                handler?.PlatformView.SetMinValue(0);
                handler?.PlatformView.SetMaxValue(data.Count - 1);
                handler?.PlatformView.SetAdapter(new PickerAdapter(data));
            }
        }

        public static void UpdateSelectedIndex(CommanPickerViewHandler handler, PickerView picker)
        {
            if (picker.SelectedIndex > GetData(picker).Count || picker.SelectedIndex < 0)
            {
                return;
            }

            handler?.PlatformView.ScrollTo(picker.SelectedIndex);
            UpdateFont(handler, picker);
        }

        public static void UpdateFont(CommanPickerViewHandler? handler, PickerView picker)
        {
            var font = string.IsNullOrEmpty(picker.FontFamily) ?
                Font.SystemFontOfSize(picker.FontSize) :
                Font.OfSize(picker.FontFamily, picker.FontSize);

            var fontManager = handler?.MauiContext?.Services.GetService<IFontManager>();
            if (fontManager != null)
            {
                var typeface = fontManager.GetTypeface(font);
                handler?.PlatformView?.SetTypeface(typeface);
            }

            var fontSize = picker.FontSize * Platform.CurrentActivity?.Resources?.DisplayMetrics?.Density;
            handler?.PlatformView.SetTextSize(fontSize > 33 ? 33 : (int)fontSize);

            if (picker.SelectedIndex == handler?.PlatformView.GetCurrentIndex())
            {
                if (string.IsNullOrEmpty(picker.SelectedItemFontFamily))
                {
                    font = Font.SystemFontOfSize(picker.FontSize, FontWeight.Bold);
                }
                else
                {
                    font = Font.OfSize(picker.SelectedItemFontFamily, picker.FontSize);
                }
                if (fontManager != null)
                {
                    var typeface = fontManager.GetTypeface(font);
                    handler?.PlatformView.SetSelectedTypeface(typeface);
                }
            }
        }

        private static List<string> GetData(PickerView picker)
        {
            var data = new List<string>();

            if (picker.ItemsSource != null)
            {
                foreach (var item in picker.ItemsSource)
                {
                    data.Add(item.ToString());
                }
            }
            return data;
        }

        void SetSelectedIndex()
        {
            if (VirtualView != null)
            {
                VirtualView.SelectedIndex = _selectedIndex;
            }
        }

        public void OnValueChange(WheelPicker picker, string oldValue, string newValue)
        {
            _selectedIndex = picker.GetCurrentIndex();
            SetSelectedIndex();
            UpdateFont(this, VirtualView);
        }

        public void OnScrollStateChange(WheelPicker picker, WheelScrollState state)
        {
            if (state == WheelScrollState.Idle)
            {
                SetSelectedIndex();
            }
        }
    }

    public class PickerAdapter : WheelAdapter
    {
        readonly List<string> _data;

        public PickerAdapter(List<string> data)
        {
            _data = data;
        }

        public override int GetPosition(string value)
        {
            return 0;
        }

        public override string GetTextWithMaximumLength()
        {
            var maxLength = _data.Max(x => x.Length);
            return _data.FirstOrDefault(x => x.Length == maxLength);
        }

        public override string GetValue(int position)
        {
            return position < 0 || position > _data.Count - 1 ? string.Empty : _data[position];
        }
    }
}
