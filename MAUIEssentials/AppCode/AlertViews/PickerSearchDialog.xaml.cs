namespace MAUIEssentials.AppCode.AlertViews
{
	public partial class PickerSearchDialog : PopupPage
	{
		PickerSearchDialogViewModel viewModel;

		public PickerSearchDialog(PickerDialogSettings settings)
		{
			try
			{
				InitializeComponent();

				BindingContext = viewModel = new PickerSearchDialogViewModel(settings);

				if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
				{
					border.WidthRequest = 350;
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		async void Handle_Clicked_Cancel(object sender, EventArgs e)
		{
			try
			{
				viewModel.PickerDialog.Search = null;
				await NavigationServices.ClosePopupPage();
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		async void Handle_Clicked_Ok(object sender, EventArgs e)
		{
			try
			{
				var selectedItem = viewModel.DataList.FirstOrDefault(x => x.IsSelected);

				if (selectedItem == null)
				{
					Handle_Clicked_Cancel(null, null);
				}
				else
				{
					await NavigationServices.ClosePopupPage();

					viewModel.PickerDialog.Search?.Invoke(selectedItem.Name);
					viewModel.PickerDialog.Search = null;
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		void PickerItemTapped(object sender, ItemTappedEventArgs e)
		{
			try
			{
				var selectedItem = e.Item as PickerModel;
				pickerView.SelectedItem = null;

				foreach (var item in viewModel.DataList)
				{
					item.IsSelected = item.Name == selectedItem.Name;
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		void OverrideLinkClicked(object sender, EventArgs e)
		{
			try
			{
				foreach (var item in viewModel.DataList)
				{
					item.IsSelected = item.Name == (sender as HtmlLabel).Text;
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}
	}
}