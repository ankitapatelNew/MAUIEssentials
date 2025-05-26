namespace MAUIEssentials.AppCode.Controls
{
    public class Switch : Microsoft.Maui.Controls.Switch
	{
		public static readonly BindableProperty OffThumbColorProperty =
			BindableProperty.Create(nameof(OffThumbColor), typeof(Color), typeof(Switch), new Color());

		public static readonly BindableProperty OffColorProperty =
			BindableProperty.Create(nameof(OffColor), typeof(Color), typeof(Switch), new Color());

		public Color OffThumbColor {
			get => (Color)GetValue(OffThumbColorProperty);
			set => SetValue(OffThumbColorProperty, value);
		}

		public Color OffColor {
			get => (Color)GetValue(OffColorProperty);
			set => SetValue(OffColorProperty, value);
		}
	}
}
