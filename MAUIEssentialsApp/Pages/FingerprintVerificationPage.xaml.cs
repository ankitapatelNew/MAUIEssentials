using System.Threading.Tasks;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using Plugin.Maui.Biometric;

namespace MAUIEssentialsApp.Pages;

public partial class FingerprintVerificationPage : PopupBase
{
	CancellationTokenSource _cancel;
	bool _isFromAppstart;

	public Action Result { get; set; }

	public FingerprintVerificationPage(bool isFromAppstart = false)
	{
		try
		{
			InitializeComponent();
			_isFromAppstart = isFromAppstart;

			mainGrid.BackgroundColor = _isFromAppstart ? AppColorResources.redColor.ToColor() : Colors.Transparent;

			imgSplash.IsVisible = isFromAppstart;
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	protected override async void OnAppearingAnimationEnd()
	{
		try
		{
			base.OnAppearingAnimationEnd();
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
			var authOptions = await DependencyService.Get<ICommonUtils>().BioMetricAuthAvailability();

			switch (authOptions)
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

			var result = await BiometricAuthenticationService.Default.AuthenticateAsync(dialogConfig, _cancel.Token);
			await SetResultAsync(result);
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

	private async void ClosePage(bool isLogOut = false)
	{
		try
		{
			if (isLogOut)
			{
				CommonUtils.Logout();
			}
			else
			{
				Result?.Invoke();
				await NavigationServices.ClosePopupPage();
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