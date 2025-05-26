using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Platform;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.Platforms.iOS.Renderers;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Font = Microsoft.Maui.Font;
using System.Collections;

[assembly: ExportRenderer(typeof(PickerView), typeof(CustomPickerViewRenderer))]
namespace MAUIEssentials.Platforms.iOS.Renderers
{
    public class CustomPickerViewRenderer : ViewRenderer<PickerView, UIPickerView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<PickerView> e)
        {
            try
            {
                base.OnElementChanged(e);

                if (Control == null)
                {
                    SetNativeControl(new UIPickerView(RectangleF.Empty));
                }

                if (e.NewElement != null)
                {
                    UpdateItemsSource();
                    UpdateSelectedIndex();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                base.OnElementPropertyChanged(sender, e);

                switch (e.PropertyName)
                {
                    case nameof(PickerView.ItemsSource):
                    case nameof(PickerView.FontSize):
                    case nameof(PickerView.FontFamily):
                        UpdateItemsSource();
                        break;
                    case nameof(PickerView.SelectedIndex):
                        UpdateSelectedIndex();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateItemsSource()
        {
            try
            {
                var font = string.IsNullOrEmpty(Element.FontFamily) ?
                Font.SystemFontOfSize(Element.FontSize) :
                Font.OfSize(Element.FontFamily, Element.FontSize);

                MyDataModel myDataModel = new MyDataModel(Element.ItemsSource, row => {
                    Element.SelectedIndex = row;
                });
                Control.Model = myDataModel;

                if (myDataModel.ItemCount == 0)
                {
                    Element.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateSelectedIndex()
        {
            try
            {
                if (Control?.Model == null)
                {
                    return;
                }

                if (Element.SelectedIndex < 0 || Element.SelectedIndex >= Control.Model.GetRowsInComponent(Control, 0))
                {
                    return;
                }
                Control.Select(Element.SelectedIndex, 0, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    class MyDataModel : UIPickerViewModel
    {
        public int ItemCount = 0;
        private readonly IList<string>? _list = new List<string>();
        private readonly Action<int>? _selectedHandler;

        public MyDataModel(IEnumerable items, Action<int> selectedHandler)
        {
            try
            {
                _selectedHandler = selectedHandler;

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        _list.Add(item.ToString() ?? string.Empty);
                    }
                    ItemCount = _list.Count;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override nfloat GetRowHeight(UIPickerView pickerView, nint component)
        {
            return 45;
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
            return new UILabel(pickerView.Bounds)
            {
                Text = _list[(int)row],
                TextAlignment = UITextAlignment.Center,
                Lines = 2,
                TextColor = UIColor.Black,
            };
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            try
            {
                _selectedHandler?.Invoke((int)row);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
