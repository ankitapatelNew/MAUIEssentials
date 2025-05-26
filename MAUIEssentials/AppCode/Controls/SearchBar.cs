using System.Windows.Input;

namespace MAUIEssentials.AppCode.Controls
{
    public class SearchBar : Microsoft.Maui.Controls.SearchBar
    {
        public static readonly BindableProperty CloseIconImageProperty =
          BindableProperty.Create(nameof(CloseIcon), typeof(string), typeof(SearchBar), string.Empty);

        public static readonly BindableProperty SearchImageProperty =
            BindableProperty.Create(nameof(SearchIcon), typeof(string), typeof(SearchBar), string.Empty);

        public static BindableProperty TintColorProperty =
            BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(SearchBar), Colors.Transparent);

        public static readonly BindableProperty SearchCommandProperty =
            BindableProperty.Create(nameof(SearchCommand), typeof(ICommand), typeof(SearchBar));

        public string CloseIcon
        {
            get => (string)GetValue(CloseIconImageProperty);
            set => SetValue(CloseIconImageProperty, value);
        }

        public string SearchIcon
        {
            get => (string)GetValue(SearchImageProperty);
            set => SetValue(SearchImageProperty, value);
        }

        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }

        public ICommand SearchCommand
        {
            get => (ICommand)GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }
    }
}