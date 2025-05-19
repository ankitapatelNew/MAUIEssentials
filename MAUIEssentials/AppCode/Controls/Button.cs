namespace MAUIEssentials.AppCode.Controls
{
    public class Button : Microsoft.Maui.Controls.Button
    {
        public static readonly BindableProperty FillTextProperty =
            BindableProperty.Create(nameof(FillText), typeof(bool), typeof(Button), false);

        public static readonly BindableProperty OrientationProperty =
            BindableProperty.Create(nameof(Orientation), typeof(GradientOrientation), typeof(Button), default(GradientOrientation));

        public static readonly BindableProperty StartColorProperty =
            BindableProperty.Create(nameof(StartColor), typeof(Color), typeof(Button), new Color());

        public static readonly BindableProperty EndColorProperty =
            BindableProperty.Create(nameof(EndColor), typeof(Color), typeof(Button), new Color());

        public static readonly BindableProperty RippleColorProperty =
            BindableProperty.Create(nameof(RippleColor), typeof(Color), typeof(Button), new Color());
            
        public bool FillText
        {
            get => (bool)GetValue(FillTextProperty);
            set => SetValue(FillTextProperty, value);
        }

        public GradientOrientation Orientation
        {
            get => (GradientOrientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public Color StartColor
        {
            get => (Color)GetValue(StartColorProperty);
            set => SetValue(StartColorProperty, value);
        }

        public Color EndColor
        {
            get => (Color)GetValue(EndColorProperty);
            set => SetValue(EndColorProperty, value);
        }

        public Color RippleColor
        {
            get => (Color)GetValue(RippleColorProperty);
            set => SetValue(RippleColorProperty, value);
        }

        public new InnerPadding Padding { get; } = new InnerPadding();

        public class InnerPadding
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }
    }
}