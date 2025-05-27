using CoreGraphics;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;
using Font = Microsoft.Maui.Font;

namespace MAUIEssentials.Handlers
{
    public partial class CommanPickerViewHandler : ViewHandler<PickerView, UIPickerView>
    {
        protected override UIPickerView CreatePlatformView() => new UIPickerView(CGRect.Empty);

        protected override void ConnectHandler(UIPickerView platformView)
        {
            base.ConnectHandler(platformView);

            // Perform any control setup here
        }

        protected override void DisconnectHandler(UIPickerView platformView)
        {
            myDataModel?.Dispose();
            platformView.Dispose();
            base.DisconnectHandler(platformView);
        }
        public static void UpdateFont(CommanPickerViewHandler handler, PickerView picker)
        {
            UpdateItemsSource(handler, picker);
        }

        static MyDataModel? myDataModel;
        public static void UpdateItemsSource(CommanPickerViewHandler handler, PickerView picker)
        {
            myDataModel = new MyDataModel(picker, new Action<PickerView,int>(RowAction));
            if (handler != null && handler.PlatformView != null)
            {
                handler.PlatformView.Model = myDataModel;
            }
            if (myDataModel.ItemCount == 0)
            {
                picker.SelectedIndex = -1;
            }
        }

        public static void RowAction(PickerView picker,int row)
        {
            picker.SelectedIndex = row;
        }

        public static async void UpdateSelectedIndex(CommanPickerViewHandler handler, PickerView picker)
        {
            try
            {
                if (handler == null || handler.PlatformView == null || handler.PlatformView.Model == null)
                {
                    return; // Early exit if handler or PlatformView is not ready
                }

                if (picker.SelectedIndex < 0 || picker.SelectedIndex >= handler.PlatformView.Model.GetRowsInComponent(handler.PlatformView, 0))
                {
                    return;
                }

                handler.PlatformView?.Select(picker.SelectedIndex, 0, false);
                await Task.Delay(100);
                if (handler.PlatformView != null)
                {
                    handler.PlatformView.ReloadAllComponents();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }

     class MyDataModel : UIPickerViewModel
    {
        public int ItemCount = 0;
        private readonly IList<string> _list = new List<string>();
        private readonly Action<PickerView,int> _selectedHandler;
        private readonly PickerView _pickerView;

        public MyDataModel(PickerView pickerView, Action<PickerView,int> selectedHandler)
        {
            _pickerView = pickerView;
            _selectedHandler = selectedHandler;

            if (_pickerView.ItemsSource != null)
            {
                foreach (var item in _pickerView.ItemsSource)
                {
                    _list.Add(item.ToString());
                }
                ItemCount = _list.Count;
            }
        }

        public override nfloat GetRowHeight(UIPickerView pickerView, nint component)
        {
            return _pickerView.ItemVisibleCount <= 3 ? 30 : 45;
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return _list.Count;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return _list[(int)row];
        }

        public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
        {
            foreach (var item in pickerView.Subviews)
            {
                item.BackgroundColor = UIColor.Clear;
            }
            pickerView.Subviews[0].Subviews[0].Subviews[2].BackgroundColor = _pickerView.SelectedBackgroundColor.ToPlatform();

            var label = new UILabel(pickerView.Bounds)
            {
                Text = _list[(int)row],
                TextAlignment = UITextAlignment.Center,
                Lines = 2
            };

            if (pickerView.SelectedRowInComponent(component) == row)
            {
                var font = string.IsNullOrEmpty(_pickerView.SelectedItemFontFamily)
                    ? Font.SystemFontOfSize(_pickerView.FontSize, FontWeight.Bold)
                    : Font.OfSize(_pickerView.SelectedItemFontFamily, _pickerView.FontSize);

                //label.Font = font.ToUIFont();
                label.TextColor = _pickerView.SelectedTextColor.ToPlatform();
            }
            else
            {
                var font = string.IsNullOrEmpty(_pickerView.FontFamily)
                    ? Font.SystemFontOfSize(_pickerView.FontSize)
                    : Font.OfSize(_pickerView.FontFamily, _pickerView.FontSize);

                //label.Font = font.ToUIFont();
                label.TextColor = _pickerView.TextColor.ToPlatform();
            }

            return label;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            pickerView.ReloadAllComponents();
            _selectedHandler?.Invoke(_pickerView,(int)row);
        }
    }
}
