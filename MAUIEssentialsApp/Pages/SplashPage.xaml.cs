using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentialsApp.Pages;

public partial class SplashPage : ContentPage
{
	public SplashPage()
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

    protected override async void OnAppearing()
    {
		try
		{
			base.OnAppearing();
			await ShowFingerPrintView();
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
    }

    private async Task ShowFingerPrintView()
    {
        try
		{
			var authOption = await DependencyService.Get<ICommonUtils>().BioMetricAuthAvailability();

			if (Settings.FingerprintEnabled && authOption == LocalBioMetricOption.None)
			{
				CommonUtils.Logout();
			}
			else if (Settings.FingerprintEnabled && authOption != LocalBioMetricOption.None)
			{
				await NavigationServices.PushAsyncPage(new FingerprintPage());
			}
			else
			{
				await NavigationServices.OpenShellPage("LoginPage", clearStack: true);
            }
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
    }
}