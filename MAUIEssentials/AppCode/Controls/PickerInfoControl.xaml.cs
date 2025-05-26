using MAUIEssentials.AppCode.AlertViews;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.AppCode.Controls
{
	public partial class PickerInfoControl : Grid
	{
		public List<string> ItemList { get; set; }

		public event EventHandler ItemSelected;
		public event EventHandler FileSelected;

		public static readonly BindableProperty TextProperty =
			BindableProperty.Create(nameof(Text), typeof(string), typeof(PickerInfoControl), string.Empty, BindingMode.TwoWay);

		public static readonly BindableProperty HeaderTextProperty =
			BindableProperty.Create(nameof(HeaderText), typeof(string), typeof(PickerInfoControl), string.Empty,
				propertyChanged: (bindable, oldValue, newValue) => (bindable as PickerInfoControl).SetMandatory());

		public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(PickerInfoControl), Colors.Black);

		public static readonly BindableProperty HeaderTextColorProperty =
			BindableProperty.Create(nameof(HeaderTextColor), typeof(Color), typeof(PickerInfoControl), Colors.Gray);

		public static readonly BindableProperty FontSizeProperty =
			BindableProperty.Create(nameof(FontSize), typeof(double), typeof(PickerInfoControl), 16d);

		public static readonly BindableProperty HeaderFontSizeProperty =
			BindableProperty.Create(nameof(HeaderFontSize), typeof(double), typeof(PickerInfoControl), 14d);

		public static readonly BindableProperty IsMandatoryProperty =
			BindableProperty.Create(nameof(IsMandatory), typeof(bool), typeof(PickerInfoControl), false,
				propertyChanged: (bindable, oldValue, newValue) => (bindable as PickerInfoControl).SetMandatory());

		public static readonly BindableProperty HasInfoTextProperty =
			BindableProperty.Create(nameof(HasInfoText), typeof(bool), typeof(PickerInfoControl), false);

		public static readonly BindableProperty InfoTextProperty =
			BindableProperty.Create(nameof(InfoText), typeof(string), typeof(PickerInfoControl), string.Empty);

		public static readonly BindableProperty HasUploadButtonProperty =
			BindableProperty.Create(nameof(HasUploadButton), typeof(bool), typeof(PickerInfoControl), false);

		public static readonly BindableProperty SelectedFileProperty =
			BindableProperty.Create(nameof(SelectedFile), typeof(FileResult), typeof(PickerInfoControl), null);

		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public string HeaderText
		{
			get => (string)GetValue(HeaderTextProperty);
			set => SetValue(HeaderTextProperty, value);
		}

		public Color TextColor
		{
			get => (Color)GetValue(TextColorProperty);
			set => SetValue(TextColorProperty, value);
		}

		public Color HeaderTextColor
		{
			get => (Color)GetValue(HeaderTextColorProperty);
			set => SetValue(HeaderTextColorProperty, value);
		}

		public double FontSize
		{
			get => (double)GetValue(FontSizeProperty);
			set => SetValue(FontSizeProperty, value);
		}

		public double HeaderFontSize
		{
			get => (double)GetValue(HeaderFontSizeProperty);
			set => SetValue(HeaderFontSizeProperty, value);
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

		public bool HasUploadButton
		{
			get => (bool)GetValue(HasUploadButtonProperty);
			set => SetValue(HasUploadButtonProperty, value);
		}

		public FileResult SelectedFile
		{
			get => (FileResult)GetValue(SelectedFileProperty);
			set => SetValue(SelectedFileProperty, value);
		}

		public PickerInfoControl()
		{
			InitializeComponent();
		}

		private void SetMandatory()
		{
			if (IsMandatory && !HeaderText.Contains(" *"))
			{
				HeaderText = string.Format("{0} *", HeaderText);
			}
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

		async void UploadFileClicked(object sender, TappedEventArgs e)
		{
			SelectedFile = await CommonUtils.OpenAttachmentDialog();
			FileSelected?.Invoke(this, EventArgs.Empty);
        }

		async void PickerTapped(object sender, EventArgs e)
		{
			try
			{
				if (CommonUtils.IsDoubleClick() || ItemList == null || !ItemList.Any())
				{
					return;
				}

				var title = HeaderText.Contains(" *") ? HeaderText.Replace(" *", "") : HeaderText;

				var pickerSettings = CommonUtils.PickerViewDialogConfig(title);
				pickerSettings.IsSearchVisible = false;
				pickerSettings.ListData = ItemList;

				if (!string.IsNullOrEmpty(Text))
				{
					pickerSettings.SelectedObject = ItemList.FirstOrDefault(x => x == Text);
				}

				pickerSettings.Search += (selectedValue) =>
				{
					Text = selectedValue;
					ItemSelected?.Invoke(this, EventArgs.Empty);
				};

				await NavigationServices.OpenPopupPage(new PickerSearchDialog(pickerSettings));
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}
	}
}