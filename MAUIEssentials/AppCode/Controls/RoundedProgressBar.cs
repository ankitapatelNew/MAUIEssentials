namespace MAUIEssentials.AppCode.Controls
{
    public class RoundedProgressBar : ProgressBar
	{
		public static readonly BindableProperty CornerRadiusProperty =
			BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(RoundedProgressBar), 0);

		public static readonly BindableProperty ProgressBackgroundColorProperty =
			BindableProperty.Create(nameof(ProgressBackgroundColor), typeof(Color), typeof(RoundedProgressBar), Colors.LightGray);

		public int CornerRadius {
			get => (int)GetValue(CornerRadiusProperty);
			set { SetValue(CornerRadiusProperty, value); }
		}

		public Color ProgressBackgroundColor {
			get => (Color)GetValue(ProgressBackgroundColorProperty);
			set { SetValue(ProgressBackgroundColorProperty, value); }
		}
	}
}
