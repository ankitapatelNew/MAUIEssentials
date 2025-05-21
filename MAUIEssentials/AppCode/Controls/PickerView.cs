namespace MAUIEssentials.AppCode.Controls
{
    public class PickerView : Microsoft.Maui.Controls.View
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(PickerView), null);

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(PickerView), -1, BindingMode.TwoWay,
                coerceValue: CoerceSelectedIndex);

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        private static object CoerceSelectedIndex(BindableObject bindable, object value)
        {
            if (value == null)
            {
                return 0;
            }
            return value;
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(PickerView), -1.0,
               defaultValueCreator: bindable => Device.GetNamedSize(NamedSize.Default, (PickerView)bindable),
               coerceValue: CoerceFontSize);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        private static object CoerceFontSize(BindableObject bindable, object value)
        {
            if (value == null)
            {
                return Device.GetNamedSize(NamedSize.Default, (PickerView)bindable);
            }
            return value;
        }

        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(PickerView), default(string));

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }
    }
}