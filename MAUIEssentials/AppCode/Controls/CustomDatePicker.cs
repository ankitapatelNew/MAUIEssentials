namespace MAUIEssentials.AppCode.Controls
{
    public class CustomDatePicker : DatePicker
    {
        public event EventHandler<DateSelectedEventArgs> Completed;

        public static readonly BindableProperty IsBorderProperty =
            BindableProperty.Create(nameof(IsBorder), typeof(bool), typeof(CustomDatePicker), false);

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(CustomDatePicker), string.Empty);

        public static readonly BindableProperty PlaceholderColorProperty =
            BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(CustomDatePicker), Colors.Transparent);

        public bool IsBorder
        {
            get => (bool)GetValue(IsBorderProperty);
            set => SetValue(IsBorderProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public Color PlaceholderColor
        {
            get => (Color)GetValue(PlaceholderColorProperty);
            set => SetValue(PlaceholderColorProperty, value);
        }

        public void UpdateSelectedDate()
        {
            Completed?.Invoke(this, new DateSelectedEventArgs { Date = Date });
        }
    }

    public class DateSelectedEventArgs : EventArgs
    {
        public DateTime Date { get; set; }
    }
}