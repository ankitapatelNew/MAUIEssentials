namespace MAUIEssentials.AppCode.Controls
{
	public partial class ChipView : Border
	{
		public static readonly BindableProperty TextProperty =
			BindableProperty.Create(nameof(Text), typeof(string), typeof(ChipView), string.Empty);

		public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ChipView), Colors.Black);

		public static readonly BindableProperty FontFamilyProperty =
			BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(ChipView), string.Empty);

		public static readonly BindableProperty FontSizeProperty =
			BindableProperty.Create(nameof(FontSize), typeof(double), typeof(ChipView), 10d);

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(ChipView), Colors.Gray);

        public static readonly BindableProperty BackgroundColorChipProperty =
            BindableProperty.Create(nameof(BackgroundColorChip), typeof(Color), typeof(ChipView), Colors.Gray);

        public Color BackgroundColorChip
        {
            get => (Color)GetValue(BackgroundColorChipProperty);
            set => SetValue(BackgroundColorChipProperty, value);
        }

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public string Text {
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public Color TextColor {
			get => (Color)GetValue(TextColorProperty);
			set => SetValue(TextColorProperty, value);
		}

		public string FontFamily {
			get => (string)GetValue(FontFamilyProperty);
			set => SetValue(FontFamilyProperty, value);
		}

		public double FontSize {
			get => (double)GetValue(FontSizeProperty);
			set => SetValue(FontSizeProperty, value);
		}

		public ChipView()
		{
			InitializeComponent();
		}
	}
}