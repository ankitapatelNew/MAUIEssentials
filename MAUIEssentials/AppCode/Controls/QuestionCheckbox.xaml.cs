namespace MAUIEssentials.AppCode.Controls
{
	public partial class QuestionCheckbox : Grid
	{
		public event EventHandler Selected;

		public static readonly BindableProperty TextProperty =
			BindableProperty.Create(nameof(Text), typeof(string), typeof(QuestionCheckbox), string.Empty,
				propertyChanged: (bindable, oldValue, newValue) => (bindable as QuestionCheckbox).SetMandatory());

		public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(QuestionCheckbox), Colors.Black);

		public static readonly BindableProperty FontSizeProperty =
			BindableProperty.Create(nameof(FontSize), typeof(double), typeof(QuestionCheckbox), 15d);

		public static readonly BindableProperty IsMandatoryProperty =
			BindableProperty.Create(nameof(IsMandatory), typeof(bool), typeof(QuestionCheckbox), false,
				propertyChanged: (bindable, oldValue, newValue) => (bindable as QuestionCheckbox).SetMandatory());

		public static readonly BindableProperty IsSelectedProperty =
			BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(QuestionCheckbox), false,
				propertyChanged: (bindable, oldValue, newValue) => (bindable as QuestionCheckbox).SetCheckbox());

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

		public bool IsSelected
		{
			get => (bool)GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value);
		}

		public QuestionCheckbox()
		{
			try
			{
				InitializeComponent();
				SetCheckbox();
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		private void SetMandatory()
		{
			try
			{
				if (IsMandatory && !Text.Contains(" *"))
				{
					var formattedString = new FormattedString();
					formattedString.Spans.Add(new Span
					{
						Text = Text,
						FontSize = FontSize,
						TextColor = TextColor
					});

					formattedString.Spans.Add(new Span
					{
						Text = " *",
						FontSize = FontSize - 3,
						TextColor = (Color)Application.Current.Resources["redColor"],
					});
					label.FormattedText = formattedString;
				}
				else
				{
					label.Text = Text;
					label.TextColor = TextColor;
					label.FontSize = FontSize;
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		private void SetCheckbox()
		{
			try
			{
				checkImage.Source = CommonUtils.SetCheckImage(IsSelected);
				checkImage.TintColor = CommonUtils.SetTintColor(IsSelected);
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		void CheckboxTapped(object sender, EventArgs e)
		{
			try
			{
				IsSelected = !IsSelected;
				Selected?.Invoke(this, e);
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}
	}
}