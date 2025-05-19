namespace MAUIEssentials.AppCode.Controls
{
    public class CaptchaImage : Image
    {
        public static readonly BindableProperty TintColorProperty =
            BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(Image), Colors.Transparent);

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(ImageSource), typeof(Image), null);

        public static readonly BindableProperty ErrorPlaceholderProperty =
            BindableProperty.Create(nameof(ErrorPlaceholder), typeof(ImageSource), typeof(Image), null);

        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }

        public ImageSource Placeholder
        {
            get => (ImageSource)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public ImageSource ErrorPlaceholder
        {
            get => (ImageSource)GetValue(ErrorPlaceholderProperty);
            set => SetValue(ErrorPlaceholderProperty, value);
        }
    }
}