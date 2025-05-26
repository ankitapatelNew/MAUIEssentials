using MAUIEssentials.AppCode.Helpers;
using Mopups.Pages;
using Mopups.Services;

namespace MAUIEssentials.AppCode.Controls
{
	public partial class LoaderPopup : PopupPage
	{
		bool _isLoaderVisible;
		readonly CancellationTokenSource cts;

		static LoaderPopup? _instance;
		public static LoaderPopup Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LoaderPopup();
				}
				return _instance;
			}
		}
		
		public LoaderPopup(string title = "")
		{
			InitializeComponent();
			indicator.IsRunning = true;
			cts = new CancellationTokenSource();

			if (!string.IsNullOrEmpty(title))
			{
				lblTitle.Text = title;
				lblTitle.IsVisible = true;
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			indicator.IsRunning = false;
			cts?.Cancel();
		}

		protected override bool OnBackButtonPressed()
		{
			return true;
		}

		public async Task ShowLoader(string title = "")
		{
			try
			{
				if (_isLoaderVisible)
				{
					return;
				}

				_isLoaderVisible = true;
				await NavigationServices.OpenPopupPage(new LoaderPopup(title));
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		public async Task HideLoader()
		{
			try
			{
				if (MopupService.Instance.PopupStack.Any() && MopupService.Instance.PopupStack.LastOrDefault() is LoaderPopup)
				{
					await NavigationServices.ClosePopupPage();
					_isLoaderVisible = false;
				}
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}
	}
}