using AsyncAwaitBestPractices;

namespace MAUIEssentials.AppCode.Controls
{
    public class CustomDatePicker : DatePicker
    {
        readonly WeakEventManager<DateSelectedEventArgs> completedEventManager = new WeakEventManager<DateSelectedEventArgs>();

        public event EventHandler<DateSelectedEventArgs> Completed
        {
            add => completedEventManager.AddEventHandler(value);
            remove => completedEventManager.RemoveEventHandler(value);
        }

        public static readonly BindableProperty IsBorderProperty =
            BindableProperty.Create(nameof(IsBorder), typeof(bool), typeof(CustomDatePicker), false);

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(CustomDatePicker), string.Empty);

        public static readonly BindableProperty PlaceholderColorProperty =
            BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(CustomDatePicker), new Color());

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
            completedEventManager?.RaiseEvent(this, new DateSelectedEventArgs { Date = Date }, nameof(Completed));
        }
    }

    public class DateSelectedEventArgs : EventArgs
    {
        public DateTime Date { get; set; }
    }
}