using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using MAUIEssentialsApp.Pages;

namespace MAUIEssentialsApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		try
		{
			InitializeComponent();

#if IOS
			if (DeviceInfo.Platform == DevicePlatform.iOS && OperatingSystem.IsIOSVersionAtLeast(15))
			{
#pragma warning disable CA1416 // Validate platform compatibility
				this.Behaviors.Add(new StatusBarBehavior
				{
					StatusBarColor = AppColorResources.redColor.ToColor(),
					StatusBarStyle = StatusBarStyle.LightContent,
				});
#pragma warning restore CA1416 // Validate platform compatibility
			}
#endif
			SetNavBarIsVisible(this, false);
			NavigationPage.SetHasBackButton(this, false);
			NavigationPage.SetHasNavigationBar(this, false);

			RegisterPages();
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	private void RegisterPages()
	{
		try
		{
			Routing.RegisterRoute(nameof(FingerprintPage), typeof(FingerprintPage));
			Routing.RegisterRoute(nameof(FingerprintVerificationPage), typeof(FingerprintVerificationPage));

			Routing.RegisterRoute(nameof(SampleFormPage), typeof(SampleFormPage));
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

    protected override bool OnBackButtonPressed()
    {
        return base.OnBackButtonPressed();
    }
}
