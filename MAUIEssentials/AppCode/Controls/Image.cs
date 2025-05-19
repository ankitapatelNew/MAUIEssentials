namespace MAUIEssentials.AppCode.Controls
{
    public class Image : Microsoft.Maui.Controls.Image
    {
        public static readonly BindableProperty TintColorProperty =
           BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(Image), Colors.Transparent, propertyChanged: OnTintColorPropertyChanged);

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(ImageSource), typeof(Image), null);

        public static readonly BindableProperty ErrorPlaceholderProperty =
            BindableProperty.Create(nameof(ErrorPlaceholder), typeof(ImageSource), typeof(Image), null);

        private static void OnTintColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                var control = (Image)bindable;
                control.Behaviors.Add(new IconTintColorBehavior { TintColor = (Color)newValue });
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                if (bindable is Image image)
                {
                    image.Handler?.UpdateValue(nameof(TintColor));
                }
            }
        }

        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource Placeholder
        {
            get => (ImageSource)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource ErrorPlaceholder
        {
            get => (ImageSource)GetValue(ErrorPlaceholderProperty);
            set => SetValue(ErrorPlaceholderProperty, value);
        }
    }
}