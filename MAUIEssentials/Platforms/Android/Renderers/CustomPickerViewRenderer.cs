using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using MAUIEssentials.AppCode.Controls;
using static Android.Widget.NumberPicker;
using Color = Android.Graphics.Color;
using Paint = Android.Graphics.Paint;

namespace MAUIEssentials.Platforms.Android.Renderers
{
    public class CustomPickerViewRenderer : ViewRenderer<PickerView, NumberPicker>
    {
        private static readonly Color _lightTextColor = Color.ParseColor("#FF000000");
        private static readonly Color _lightDividerColor = Color.ParseColor("#FFCCCCCC");
        private static readonly Color _lightBackgroundColor = Color.ParseColor("#FFFFFFFF");

        private int _selectedIndex;
        private Typeface _currentTypeface;
        private float _currentTextSize;

        public CustomPickerViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<PickerView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var picker = new NumberPicker(Context);
                SetNativeControl(picker);

                Control.SetBackgroundColor(_lightBackgroundColor);
                Control.WrapSelectorWheel = false;
            }
            else
            {
                Control.ValueChanged -= OnValueChanged;
            }

            if (e.NewElement != null)
            {
                Control.ValueChanged += OnValueChanged;
                UpdateAllProperties();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == PickerView.ItemsSourceProperty.PropertyName)
            {
                UpdateItemsSource();
            }
            else if (e.PropertyName == PickerView.SelectedIndexProperty.PropertyName)
            {
                UpdateSelectedIndex();
            }
            else if (e.PropertyName == PickerView.FontSizeProperty.PropertyName ||
                     e.PropertyName == PickerView.FontFamilyProperty.PropertyName)
            {
                UpdateFont();
            }
        }

        private void UpdateAllProperties()
        {
            UpdateItemsSource();
            UpdateSelectedIndex();
            UpdateFont();
            UpdateColors();
        }

        private void UpdateItemsSource()
        {
            if (Element.ItemsSource == null)
            {
                Control.SetDisplayedValues(null);
                Control.MinValue = 0;
                Control.MaxValue = 0;
                return;
            }

            var items = Element.ItemsSource.Cast<object>().Select(x => x.ToString()).ToArray();

            if (items.Length > 0)
            {
                Control.MinValue = 0;
                Control.MaxValue = items.Length - 1;
                Control.SetDisplayedValues(items);
                Control.WrapSelectorWheel = false;
            }
            else
            {
                Control.SetDisplayedValues(null);
                Control.MinValue = 0;
                Control.MaxValue = 0;
            }
        }

        private void UpdateSelectedIndex()
        {
            if (Element.SelectedIndex >= Control.MinValue && Element.SelectedIndex <= Control.MaxValue)
            {
                Control.Value = Element.SelectedIndex;
            }
        }

        private void UpdateFont()
        {
            var fontSize = Element.FontSize > 0 ? Element.FontSize : Device.GetNamedSize(NamedSize.Default, Element);
            _currentTextSize = TypedValue.ApplyDimension(ComplexUnitType.Sp, (float)fontSize, Context.Resources.DisplayMetrics);

            _currentTypeface = !string.IsNullOrEmpty(Element.FontFamily)
                ? Typeface.Create(Element.FontFamily, TypefaceStyle.Normal)
                : Typeface.Default;
        }

        private void UpdateColors()
        {
            // Update all child TextViews with light theme colors
            for (int i = 0; i < Control.ChildCount; i++)
            {
                if (Control.GetChildAt(i) is TextView textView)
                {
                    textView.SetTextColor(_lightTextColor);
                    textView.Typeface = _currentTypeface;
                    textView.SetTextSize(ComplexUnitType.Px, _currentTextSize);
                }
            }

            // Update internal NumberPicker elements using reflection
            try
            {
                // Set selector wheel paint
                var selectorWheelPaintField = Control.Class.GetDeclaredField("mSelectorWheelPaint");
                selectorWheelPaintField.Accessible = true;
                var wheelPaint = (Paint)selectorWheelPaintField.Get(Control);
                wheelPaint.Color = _lightTextColor;
                wheelPaint.TextSize = _currentTextSize;
                wheelPaint.SetTypeface(_currentTypeface);

                // Set selection divider
                var selectionDividerField = Control.Class.GetDeclaredField("mSelectionDivider");
                if (selectionDividerField != null)
                {
                    selectionDividerField.Accessible = true;
                    var divider = new ColorDrawable(_lightDividerColor);
                    selectionDividerField.Set(Control, divider);
                }

                Control.Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Reflection failed: {ex}");
            }
        }

        private void OnValueChanged(object sender, ValueChangeEventArgs e)
        {
            _selectedIndex = e.NewVal;
            Element.SelectedIndex = _selectedIndex;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Control != null)
            {
                Control.ValueChanged -= OnValueChanged;
            }
            base.Dispose(disposing);
        }
    }
}
