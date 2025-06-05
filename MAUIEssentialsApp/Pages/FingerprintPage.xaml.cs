using System.Threading.Tasks;
using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using Plugin.Maui.Biometric;

namespace MAUIEssentialsApp.Pages;

public partial class FingerprintPage : ContentPage
{
	bool _isAppLink;
	CancellationTokenSource _cancel;

	public Action OpenOnAppLink { get; set; }

	public FingerprintPage(bool isAppLink = false)
	{
		try
		{
			InitializeComponent();
			_isAppLink = isAppLink;
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	protected override async void OnAppearing()
	{
		try
		{
			base.OnAppearing();
			await Authenticate();
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	private async Task Authenticate()
	{
		try
		{
			var authOption = await DependencyService.Get<ICommonUtils>().BioMetricAuthAvailability();

			switch (authOption)
			{
				case LocalBioMetricOption.Face:
					await AuthenticateAsync(LocalizationResources.scanYourFaceprint);
					break;
				case LocalBioMetricOption.Fingerprint:
					await AuthenticateAsync(LocalizationResources.scanYourFinger);
					break;
			}
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	private async Task AuthenticateAsync(string reason)
	{
		try
		{
			_cancel = new CancellationTokenSource();

			var dialogConfig = new AuthenticationRequest()
			{
				Title = LocalizationResources.touchIdMAUIEssentials,
				Description = reason,
				AllowPasswordAuth = true,
			};

			MainThread.BeginInvokeOnMainThread(async ()  =>
			{
				await Task.Delay(200);
				var result = await BiometricAuthenticationService.Default.AuthenticateAsync(dialogConfig, _cancel.Token);
				await SetResultAsync(result);
			});
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	private async Task SetResultAsync(AuthenticationResponse result)
	{
		try
		{
			if (result.Status == BiometricResponseStatus.Success)
			{
				Settings.FingerprintEnabled = true;
				await Task.Delay(100);
				ClosePage();
			}
			else
			{
				ClosePage(true);
			}
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	private async void ClosePage(bool isLogout = false)
	{
		try
		{
			if (isLogout)
			{
				CommonUtils.Logout();
			}
			else
			{
				await NavigationServices.OpenShellPage("MainPage", clearStack: true);

				if (_isAppLink)
				{
					OpenOnAppLink?.Invoke();
				}
			}
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}
	
	protected override bool OnBackButtonPressed()
    {
        return true;
    }

    async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        try
        {
            await Authenticate();
        }
        catch (Exception ex)
        {
            ex.LogException();
        }
    }
}