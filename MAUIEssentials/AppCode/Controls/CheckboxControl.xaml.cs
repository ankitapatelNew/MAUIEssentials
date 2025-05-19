namespace MAUIEssentials.AppCode.Controls
{
	public partial class CheckboxControl : Grid
	{
		public event EventHandler Selected;

		public static readonly BindableProperty TextProperty =
			BindableProperty.Create(nameof(Text), typeof(string), typeof(CheckboxControl), string.Empty,
				propertyChanged: (bindable, oldValue, newValue) => (bindable as CheckboxControl).SetMandatory());

		public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CheckboxControl), Colors.Black);

		public static readonly BindableProperty FontSizeProperty =
			BindableProperty.Create(nameof(FontSize), typeof(double), typeof(CheckboxControl), 15d);

		public static readonly BindableProperty IsMandatoryProperty =
			BindableProperty.Create(nameof(IsMandatory), typeof(bool), typeof(CheckboxControl), false,
				propertyChanged: (bindable, oldValue, newValue) => (bindable as CheckboxControl).SetMandatory());

		public static readonly BindableProperty HasInfoTextProperty =
			BindableProperty.Create(nameof(HasInfoText), typeof(bool), typeof(CheckboxControl), false);

		public static readonly BindableProperty InfoTextProperty =
			BindableProperty.Create(nameof(InfoText), typeof(string), typeof(CheckboxControl), string.Empty);

		public static readonly BindableProperty IsSelectedProperty =
			BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(CheckboxControl), false,
				propertyChanged: (bindable, oldValue, newValue) => (bindable as CheckboxControl).SetCheckbox());

		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public Color TextColor
		{
			get => (Color)GetValue(TextColorProperty);
			set => SetValue(TextColorProperty, value);
		}

		public double FontSize
		{
			get => (double)GetValue(FontSizeProperty);
			set => SetValue(FontSizeProperty, value);
		}

		public bool IsMandatory
		{
			get => (bool)GetValue(IsMandatoryProperty);
			set => SetValue(IsMandatoryProperty, value);
		}

		public bool HasInfoText
		{
			get => (bool)GetValue(HasInfoTextProperty);
			set => SetValue(HasInfoTextProperty, value);
		}

		public string InfoText
		{
			get => (string)GetValue(InfoTextProperty);
			set => SetValue(InfoTextProperty, value);
		}

		public bool IsSelected
		{
			get => (bool)GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value);
		}

		public CheckboxControl()
		{
			InitializeComponent();
			SetCheckbox();
		}

		private void SetMandatory()
		{
			if (IsMandatory && !Text.Contains(" *"))
			{
				Text = string.Format("{0} *", Text);
			}
		}

		private void SetCheckbox()
		{
			checkImage.Source = CommonUtils.SetCheckImage(IsSelected);
			checkImage.TintColor = CommonUtils.SetTintColor(IsSelected);
		}

		async void InfoTapped(object sender, EventArgs e)
		{
			if (CommonUtils.IsDoubleClick())
			{
				return;
			}

			var popup = new NotificationPopup(NotificationType.Info, string.Empty, InfoText, InfoText.IsHtml());
			await NavigationServices.OpenPopupPage(popup);
		}

		void CheckboxTapped(object sender, EventArgs e)
		{
			IsSelected = !IsSelected;
			Selected?.Invoke(this, e);
		}
	}
}