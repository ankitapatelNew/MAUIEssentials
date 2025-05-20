namespace MAUIEssentials.AppCode.Controls
{
	public partial class DatePickerControl : Grid
	{
		public DateTime SelectedDate { get; private set; }
		public event EventHandler DateSelected;

		public static readonly BindableProperty TextProperty =
			BindableProperty.Create(nameof(Text), typeof(string), typeof(DatePickerControl), string.Empty);

		public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(DatePickerControl), Colors.Black);

		public static readonly BindableProperty PlaceholderProperty =
			BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(DatePickerControl), string.Empty);

		public static readonly BindableProperty PlaceholderColorProperty =
			BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(DatePickerControl), Colors.Gray);

		public static readonly BindableProperty FontSizeProperty =
			BindableProperty.Create(nameof(FontSize), typeof(double), typeof(DatePickerControl), 16d);

		public static readonly BindableProperty DateFormatProperty =
			BindableProperty.Create(nameof(DateFormat), typeof(string), typeof(DatePickerControl), "dd/MM/yyyy");

		public static readonly BindableProperty MinimumDateProperty =
			BindableProperty.Create(nameof(MinimumDate), typeof(DateTime), typeof(DatePickerControl), DateTime.MinValue);

		public static readonly BindableProperty MaximumDateProperty =
			BindableProperty.Create(nameof(MaximumDate), typeof(DateTime), typeof(DatePickerControl), DateTime.MaxValue);

		public static readonly BindableProperty HasInfoTextProperty =
			BindableProperty.Create(nameof(HasInfoText), typeof(bool), typeof(DatePickerControl), false);

		public static readonly BindableProperty InfoTextProperty =
			BindableProperty.Create(nameof(InfoText), typeof(string), typeof(DatePickerControl), string.Empty);

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

		public string Placeholder
		{
			get => (string)GetValue(PlaceholderProperty);
			set => SetValue(PlaceholderProperty, value);
		}

		public Color PlaceholderColor
		{
			get => (Color)GetValue(PlaceholderColorProperty);
			set => SetValue(PlaceholderColorProperty, value);
		}

		public double FontSize
		{
			get => (double)GetValue(FontSizeProperty);
			set => SetValue(FontSizeProperty, value);
		}

		public string DateFormat
		{
			get => (string)GetValue(DateFormatProperty);
			set => SetValue(DateFormatProperty, value);
		}

		public DateTime MinimumDate
		{
			get => (DateTime)GetValue(MinimumDateProperty);
			set => SetValue(MinimumDateProperty, value);
		}

		public DateTime MaximumDate
		{
			get => (DateTime)GetValue(MaximumDateProperty);
			set => SetValue(MaximumDateProperty, value);
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

		public DatePickerControl()
		{
			InitializeComponent();
			SetVisibility();

			if (!string.IsNullOrEmpty(Text))
			{
				try
				{
					SelectedDate = DateTime.ParseExact(Text, DateFormat, CultureInfo.InvariantCulture);
				}
				catch (Exception ex)
				{
					ex.LogException();
				}
			}
		}

		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (propertyName == TextProperty.PropertyName)
			{
				SetVisibility();

				if (!string.IsNullOrEmpty(Text))
				{
					try
					{
						SelectedDate = DateTime.ParseExact(Text, DateFormat, CultureInfo.InvariantCulture);
					}
					catch (Exception ex)
					{
						ex.LogException();
					}
				}
			}
		}

		public void DatePickerControlTapped(object sender, EventArgs args)
		{
			try
			{
				datePicker.Focus();
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		public void DatePickerSelected(object sender, DateChangedEventArgs args)
		{
			try
			{
				SelectedDate = args.NewDate;
				Text = SelectedDate.ToString(DateFormat);

				SetVisibility();
				DateSelected?.Invoke(this, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		public void DatePickerCompleted(object sender, DateSelectedEventArgs args)
		{
			try
			{
				SelectedDate = args.Date;
				Text = SelectedDate.ToString(DateFormat);

				SetVisibility();
				DateSelected?.Invoke(this, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		private void SetVisibility()
		{
			try
			{
				lblPlaceholder.IsVisible = string.IsNullOrEmpty(Text);
				lblDate.IsVisible = !lblPlaceholder.IsVisible;
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		async void InfoTapped(object sender, EventArgs e)
		{
			try
			{
				if (CommonUtils.IsDoubleClick())
				{
					return;
				}

				var popup = new NotificationPopup(NotificationType.Info, string.Empty, InfoText, InfoText.IsHtml());
				await NavigationServices.OpenPopupPage(popup);
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		void DatePickerUnfocused(object sender, FocusEventArgs e)
		{
			try
			{
				SelectedDate = datePicker.Date;
				Text = SelectedDate.ToString(DateFormat);

				SetVisibility();
				DateSelected?.Invoke(this, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}
	}
}