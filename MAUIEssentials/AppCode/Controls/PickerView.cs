using System.Collections;

namespace MAUIEssentials.AppCode.Controls
{
    public class PickerView : View
    {
        private readonly AsyncAwaitBestPractices.WeakEventManager indexChangeEventmanager = new AsyncAwaitBestPractices.WeakEventManager();

        public event EventHandler SelectedIndexChanged
        {
            add => indexChangeEventmanager.AddEventHandler(value);
            remove => indexChangeEventmanager.RemoveEventHandler(value);
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(PickerView), null);

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(PickerView), -1, BindingMode.TwoWay,
                coerceValue: CoerceSelectedIndex, propertyChanged: (bindable, oldValue, newValue) => (bindable as PickerView).SendIndexChanged());

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        private static object CoerceSelectedIndex(BindableObject bindable, object value)
        {
            return value ?? 0;
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(PickerView), -1.0,
               defaultValueCreator: GetDefaultFontSize,
               coerceValue: CoerceFontSize);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        private static object GetDefaultFontSize(BindableObject bindable)
        {
            // Use the new MAUI way to get default font size
            return Application.Current?.Resources["FontMedium"] ?? 14.0;
        }

        private static object CoerceFontSize(BindableObject bindable, object value)
        {
            return value ?? 0;
        }

        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(PickerView), default(string));

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }
        
        public static readonly BindableProperty SelectedItemFontFamilyProperty =
            BindableProperty.Create(nameof(SelectedItemFontFamily), typeof(string), typeof(PickerView), default(string));

        public string SelectedItemFontFamily
        {
            get => (string)GetValue(SelectedItemFontFamilyProperty);
            set => SetValue(SelectedItemFontFamilyProperty, value);
        }

        public static readonly BindableProperty SelectedBackgroundColorProperty =
            BindableProperty.Create(nameof(SelectedBackgroundColor), typeof(Color), typeof(PickerView), Colors.LightGray);

        public Color SelectedBackgroundColor
        {
            get => (Color)GetValue(SelectedBackgroundColorProperty);
            set => SetValue(SelectedBackgroundColorProperty, value);
        }

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(PickerView), Colors.Black);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty SelectedTextColorProperty =
            BindableProperty.Create(nameof(SelectedTextColor), typeof(Color), typeof(PickerView), Colors.Black);

        public Color SelectedTextColor
        {
            get => (Color)GetValue(SelectedTextColorProperty);
            set => SetValue(SelectedTextColorProperty, value);
        }

        public static readonly BindableProperty ItemVisibleCountProperty =
            BindableProperty.Create(nameof(ItemVisibleCount), typeof(int), typeof(PickerView), 5);

        public int ItemVisibleCount
        {
            get => (int)GetValue(ItemVisibleCountProperty);
            set => SetValue(ItemVisibleCountProperty, value);
        }

        private void SendIndexChanged()
        {
            indexChangeEventmanager.RaiseEvent(this, EventArgs.Empty, nameof(SelectedIndexChanged));
        }

        public void UpdateIndexProperty()
        {
            //if (DeviceInfo.Platform == DevicePlatform.Android)
            //{
            //    OnPropertyChanged(SelectedIndexProperty.PropertyName);
            //}
        }
    }
}