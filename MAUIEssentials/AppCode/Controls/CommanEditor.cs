namespace MAUIEssentials.AppCode.Controls
{
    public class CommanEditor : Editor
    {
        public static readonly BindableProperty IsBorderProperty =
			BindableProperty.Create(nameof(IsBorder), typeof(bool), typeof(CommanEditor), false);

		public static readonly BindableProperty AttributedTextProperty =
			 BindableProperty.Create(nameof(AttributedText), typeof(bool), typeof(CommanEditor), false);

		public static readonly BindableProperty BorderWidthProperty =
			BindableProperty.Create(nameof(BorderWidth), typeof(int), typeof(CommanEditor), 1);

		public static readonly BindableProperty BorderRadiusProperty =
			BindableProperty.Create(nameof(BorderRadius), typeof(int), typeof(CommanEditor), 0);

		public static readonly BindableProperty BorderColorProperty =
			BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(CommanEditor), Colors.Gray);

		public bool AttributedText {
			get { return (bool)GetValue(AttributedTextProperty); }
			set { SetValue(AttributedTextProperty, value); }
		}

		public bool IsBorder {
			get { return (bool)GetValue(IsBorderProperty); }
			set { SetValue(IsBorderProperty, value); }
		}

		public int BorderWidth {
			get { return (int)GetValue(BorderWidthProperty); }
			set { SetValue(BorderWidthProperty, value); }
		}

		public int BorderRadius {
			get { return (int)GetValue(BorderRadiusProperty); }
			set { SetValue(BorderRadiusProperty, value); }
		}

		public Color BorderColor {
			get { return (Color)GetValue(BorderColorProperty); }
			set { SetValue(BorderColorProperty, value); }
		}

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			var sizeRequest = base.OnMeasure(widthConstraint, heightConstraint);
			return new SizeRequest(new Size(sizeRequest.Request.Width, Math.Max(40, sizeRequest.Request.Height)));
		}
    }
}