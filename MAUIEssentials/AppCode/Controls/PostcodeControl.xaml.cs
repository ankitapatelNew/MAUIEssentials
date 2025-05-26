using System.Text.Json.Serialization;
using MAUIEssentials.AppCode.AlertViews;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;

namespace MAUIEssentials.AppCode.Controls
{
	public partial class PostcodeControl : StackLayout
	{
		public event EventHandler PostcodeSearch;
		public event EventHandler AddressSelected;

		public static readonly BindableProperty PostcodeProperty =
			BindableProperty.Create(nameof(Postcode), typeof(string), typeof(PostcodeControl), string.Empty);

		public static readonly BindableProperty PostcodeErrorTextProperty =
			BindableProperty.Create(nameof(PostcodeErrorText), typeof(string), typeof(PostcodeControl), string.Empty);

		public static readonly BindableProperty HasAddressListProperty =
			BindableProperty.Create(nameof(HasAddressList), typeof(bool), typeof(PostcodeControl), false);

		public static readonly BindableProperty SelectedAddressProperty =
			BindableProperty.Create(nameof(SelectedAddress), typeof(PostcodeItem), typeof(PostcodeControl), null,
				propertyChanged: (bindable, oldValue, newValue) => ((PostcodeControl)bindable).SetPlaceholder());

		public static readonly BindableProperty AddressListProperty =
			BindableProperty.Create(nameof(AddressList), typeof(List<PostcodeItem>), typeof(PostcodeControl), null);

		public string Postcode
		{
			get => (string)GetValue(PostcodeProperty);
			set => SetValue(PostcodeProperty, value);
		}

		public string PostcodeErrorText
		{
			get => (string)GetValue(PostcodeErrorTextProperty);
			set => SetValue(PostcodeErrorTextProperty, value);
		}

		public bool HasAddressList
		{
			get => (bool)GetValue(HasAddressListProperty);
			set => SetValue(HasAddressListProperty, value);
		}

		public PostcodeItem SelectedAddress
		{
			get => (PostcodeItem)GetValue(SelectedAddressProperty);
			set => SetValue(SelectedAddressProperty, value);
		}

		public List<PostcodeItem> AddressList
		{
			get => (List<PostcodeItem>)GetValue(AddressListProperty);
			set => SetValue(AddressListProperty, value);
		}

		public PostcodeControl()
		{
			try
			{
				InitializeComponent();
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		void SetPlaceholder()
		{
			try
			{
				lblAddressPlaceholder.IsVisible = SelectedAddress == null;
				lblAddress.IsVisible = SelectedAddress != null;
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		void PostCode_TextChanged(object sender, EventArgs e)
		{
			try
			{

			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		void Button_Clicked(object sender, EventArgs e)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(Postcode))
				{
					PostcodeErrorText = string.Empty;
					PostcodeSearch?.Invoke(this, e);
				}
				else
				{
					PostcodeErrorText = LocalizationResources.enterPostcode;
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		async void ChooseAddressTapped(object sender, EventArgs e)
		{
			try
			{
				if (CommonUtils.IsDoubleClick())
				{
					return;
				}

				var pickerSettings = CommonUtils.PickerViewDialogConfig(LocalizationResources.chooseAddress);
				pickerSettings.ListData = AddressList.Select(x => x.Label).ToList();
				pickerSettings.SelectedObject = SelectedAddress?.Label;

				pickerSettings.Search += (addressPickText) =>
				{
					SelectedAddress = AddressList.FirstOrDefault(x => x.Label == addressPickText);
					SetPlaceholder();

					if (SelectedAddress != null)
					{
						AddressSelected?.Invoke(this, EventArgs.Empty);
					}
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

public class PostcodeItem
{
	public string? Label { get; set; }
	public List<FieldModel>? Fields { get; set; }
}

public class FieldModel
{
	public string Name { get; set; }
	public object Value { get; set; }

	[JsonIgnore]
	public string? ValueText => Value != null ? Value.ToString() : string.Empty;

	public FieldModel()
	{
		
	}

	public FieldModel(string name, object value)
	{
		Name = name;
		Value = value;
	}
}