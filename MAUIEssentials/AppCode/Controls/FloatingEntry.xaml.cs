using System.Runtime.CompilerServices;
using AsyncAwaitBestPractices;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.AppCode.Controls
{
	public partial class FloatingEntry : Grid
	{
		double _placeholderFontSize, _titleFontSize;

		readonly AsyncAwaitBestPractices.WeakEventManager completedEventManager = new AsyncAwaitBestPractices.WeakEventManager();
        readonly WeakEventManager<FocusEventArgs> focusEventManager = new WeakEventManager<FocusEventArgs>();
        readonly WeakEventManager<FocusEventArgs> unfocusEventManager = new WeakEventManager<FocusEventArgs>();

        public event EventHandler Completed
        {
            add => completedEventManager.AddEventHandler(value);
            remove => completedEventManager.AddEventHandler(value);
        }

        public event EventHandler<FocusEventArgs> EntryFocused
        {
            add => focusEventManager.AddEventHandler(value);
            remove => focusEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<FocusEventArgs> EntryUnfocused
        {
            add => unfocusEventManager.AddEventHandler(value);
            remove => unfocusEventManager.RemoveEventHandler(value);
        }
		
		public event EventHandler TextChanged;

		public static readonly BindableProperty TextProperty =
			BindableProperty.Create(nameof(Text), typeof(string), typeof(FloatingEntry), string.Empty, BindingMode.TwoWay, null, HandleBindingPropertyChangedDelegate);

		public static readonly BindableProperty PlaceholderProperty =
			BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(FloatingEntry), string.Empty);

		public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(FloatingEntry), Colors.Black);

		public static readonly BindableProperty PlaceholderColorProperty =
			BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(FloatingEntry), Colors.Black);

		public static readonly BindableProperty FontSizeProperty =
			BindableProperty.Create(nameof(FontSize), typeof(double), typeof(FloatingEntry), 17d);

		public static readonly BindableProperty ReturnTypeProperty =
			BindableProperty.Create(nameof(ReturnType), typeof(ReturnType), typeof(FloatingEntry), ReturnType.Default);

		public static readonly BindableProperty KeyboardProperty =
			BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(FloatingEntry), Keyboard.Default, coerceValue: (o, v) => (Keyboard)v ?? Keyboard.Default);

		public static readonly BindableProperty IsPasswordProperty =
			BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(FloatingEntry), default(bool));

		public static readonly BindableProperty TopMarginProperty =
			BindableProperty.Create(nameof(TopMargin), typeof(int), typeof(FloatingEntry), 24);

		public static readonly BindableProperty IsMandatoryProperty =
			BindableProperty.Create(nameof(IsMandatory), typeof(bool), typeof(FloatingEntry), false);

		public static readonly BindableProperty HasInfoTextProperty =
			BindableProperty.Create(nameof(HasInfoText), typeof(bool), typeof(FloatingEntry), false);

		public static readonly BindableProperty InfoTextProperty =
			BindableProperty.Create(nameof(InfoText), typeof(string), typeof(FloatingEntry), string.Empty);

		public static readonly BindableProperty FontAttributeEntryProperty =
			BindableProperty.Create(nameof(FontAttributeEntry), typeof(FontAttributes), typeof(FloatingEntry), FontAttributes.None);

		public static readonly BindableProperty FontAttributePlaceholderProperty =
			BindableProperty.Create(nameof(FontAttributePlaceholder), typeof(FontAttributes), typeof(FloatingEntry), FontAttributes.None);

		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public string Placeholder
		{
			get => (string)GetValue(PlaceholderProperty);
			set => SetValue(PlaceholderProperty, value);
		}

		public Color TextColor
		{
			get => (Color)GetValue(TextColorProperty);
			set => SetValue(TextColorProperty, value);
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

		public bool IsPassword
		{
			get => (bool)GetValue(IsPasswordProperty);
			set => SetValue(IsPasswordProperty, value);
		}

		public ReturnType ReturnType
		{
			get => (ReturnType)GetValue(ReturnTypeProperty);
			set => SetValue(ReturnTypeProperty, value);
		}

		public Keyboard Keyboard
		{
			get => (Keyboard)GetValue(KeyboardProperty);
			set => SetValue(KeyboardProperty, value);
		}

		public int TopMargin
		{
			get => (int)GetValue(TopMarginProperty);
			set => SetValue(TopMarginProperty, value);
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

		public FontAttributes FontAttributeEntry
		{
			get => (FontAttributes)GetValue(FontAttributeEntryProperty);
			set => SetValue(FontAttributeEntryProperty, value);
		}

		public FontAttributes FontAttributePlaceholder
		{
			get => (FontAttributes)GetValue(FontAttributePlaceholderProperty);
			set => SetValue(FontAttributePlaceholderProperty, value);
		}

		static async void HandleBindingPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
		{
			try
			{
				var control = bindable as FloatingEntry;
				if (!control.entry.IsFocused)
				{
					if (!string.IsNullOrEmpty((string)newValue))
					{
						await control.TransitionToTitle(false);
					}
					else
					{
						await control.TransitionToPlaceholder(false);
					}
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		public FloatingEntry()
		{
			try
			{
				InitializeComponent();
				lblPlaceholder.TranslationX = 5;
				lblPlaceholder.HorizontalTextAlignment = TextAlignment.Start;

				_titleFontSize = FontSize - 2;
				_placeholderFontSize = FontSize;

				if (IsMandatory)
				{
					SetMandatoryLabel();
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		public new void Focus()
		{
			try
			{
				if (IsEnabled)
				{
					entry.Focus();
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		async void Handle_Focused(object sender, FocusEventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(Text))
				{
					await TransitionToTitle(true);
				}

				focusEventManager?.RaiseEvent(this, e, nameof(EntryFocused));
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		async void Handle_Unfocused(object sender, FocusEventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(Text))
				{
					await TransitionToPlaceholder(true);
				}

				unfocusEventManager?.RaiseEvent(this, e, nameof(EntryUnfocused));
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		async Task TransitionToTitle(bool animated)
		{
			try
			{
				if (animated)
				{
					var t1 = lblPlaceholder.TranslateTo(-1, -TopMargin, 100);
					var t2 = SizeTo(_titleFontSize);
					await Task.WhenAll(t1, t2);
				}
				else
				{
					lblPlaceholder.TranslationX = -1;
					lblPlaceholder.TranslationY = -TopMargin;
					lblPlaceholder.FontSize = _titleFontSize;
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		async Task TransitionToPlaceholder(bool animated)
		{
			try
			{
				if (animated)
				{
					var t1 = lblPlaceholder.TranslateTo(5, 0, 100);
					var t2 = SizeTo(_placeholderFontSize);
					await Task.WhenAll(t1, t2);
				}
				else
				{
					lblPlaceholder.TranslationX = 5;
					lblPlaceholder.TranslationY = 0;
					lblPlaceholder.FontSize = _placeholderFontSize;
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		void Handle_Tapped(object sender, EventArgs e)
		{
			try
			{
				if (IsEnabled)
				{
					entry.Focus();
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		Task SizeTo(double fontSize)
		{
			var taskCompletionSource = new TaskCompletionSource<bool>();

			try
			{
				// setup information for animation
				Action<double> callback = input => { lblPlaceholder.FontSize = input; };
				double startingHeight = lblPlaceholder.FontSize;
				double endingHeight = fontSize;
				uint rate = 5;
				uint length = 100;
				Easing easing = Easing.Linear;

				// now start animation with all the setup information
				lblPlaceholder.Animate("placeholder", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));
			}
			catch (Exception ex)
			{
				ex.LogException();
			}

			return taskCompletionSource.Task;
		}

		void Handle_Completed(object sender, EventArgs e)
		{
			try
			{
				completedEventManager?.RaiseEvent(this, e, nameof(Completed));
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			try
			{
				base.OnPropertyChanged(propertyName);

				if (propertyName == IsEnabledProperty.PropertyName)
				{
					entry.IsEnabled = IsEnabled;
				}
				else if (propertyName == FontSizeProperty.PropertyName)
				{
					_titleFontSize = FontSize - 2;
					_placeholderFontSize = FontSize;
					lblPlaceholder.FontSize = _placeholderFontSize;
				}
				else if (propertyName == IsMandatoryProperty.PropertyName)
				{
					if (IsMandatory)
					{
						SetMandatoryLabel();
					}
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		private void SetMandatoryLabel()
		{
			try
			{
				var formattedString = new FormattedString();
				formattedString.Spans.Add(new Span
				{
					Text = Placeholder
				});

				formattedString.Spans.Add(new Span
				{
					Text = " "
				});

				formattedString.Spans.Add(new Span
				{
					Text = "*"
				});

				lblPlaceholder.FormattedText = formattedString;
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

		void Handle_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				completedEventManager?.RaiseEvent(this, e, nameof(Completed));
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}
	}
}