namespace MAUIEssentials.AppCode.Controls
{
    public class DashedLine : Microsoft.Maui.Controls.View
    {
        public static readonly BindableProperty DashWidthProperty =
            BindableProperty.Create(nameof(DashWidth), typeof(int), typeof(DashedLine), 4);

        public static readonly BindableProperty DashGapProperty =
            BindableProperty.Create(nameof(DashGap), typeof(int), typeof(DashedLine), 2);

        public static readonly BindableProperty DashColorProperty =
            BindableProperty.Create(nameof(DashColor), typeof(Color), typeof(DashedLine), Colors.Gray);

        public static readonly BindableProperty OrientationProperty =
            BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(DashedLine), StackOrientation.Horizontal);

        public int DashWidth
        {
            get { return (int)GetValue(DashWidthProperty); }
            set { SetValue(DashWidthProperty, value); }
        }

        public int DashGap
        {
            get { return (int)GetValue(DashGapProperty); }
            set { SetValue(DashGapProperty, value); }
        }

        public Color DashColor
        {
            get { return (Color)GetValue(DashColorProperty); }
            set { SetValue(DashColorProperty, value); }
        }

        public StackOrientation Orientation
        {
            get { return (StackOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
    }
}