using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.Models;
using MAUIEssentials.Styles;

namespace MAUIEssentials;

public partial class App : Application
{
	public static IMauiContext? mauiContext;
	const int smallWightResolution = 768;
	const int smallHeightResolution = 1280;

	static App? _instance;
	public static App Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new App();
			}
			return _instance;
		}
	}

	public App()
	{
		try
		{
			InitializeComponent();
			SetCurrentResources();

			CommonUtils.CheckInternetAccess(new ConnectivityChangedEventArgs(Connectivity.NetworkAccess, Connectivity.ConnectionProfiles));
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	private void SetCurrentResources()
	{
		try
		{
			if (dictionary == null)
			{
				return;
			}

			// Clear existing device-specific resources
			dictionary.MergedDictionaries.Remove(IPhoneXDevicesStyle.Instance);
			dictionary.MergedDictionaries.Remove(SmallDevicesStyle.Instance);
			dictionary.MergedDictionaries.Remove(DefaultDevicesStyle.Instance);

			var commonUtils = DependencyService.Get<ICommonUtils>();
			if (commonUtils != null && commonUtils.IsIphoneX() && DeviceInfo.Platform == DevicePlatform.iOS)
			{
				if (IPhoneXDevicesStyle.Instance != null)
				{
					dictionary.MergedDictionaries.Add(IPhoneXDevicesStyle.Instance);
				}
				else
				{
					throw new InvalidOperationException("IPhoneXDevicesStyle.Instance is not initialized.");
				}
			}
			else if (IsASmallDevice())
			{
				if (SmallDevicesStyle.Instance != null)
				{
					dictionary.MergedDictionaries.Add(SmallDevicesStyle.Instance);
				}
				else
				{
					throw new InvalidOperationException("SmallDevicesStyle.Instance is not initialized.");
				}
			}
			else
			{
				if (DefaultDevicesStyle.Instance != null)
				{
					dictionary.MergedDictionaries.Add(DefaultDevicesStyle.Instance);
				}
				else
				{
					throw new InvalidOperationException("DefaultDevicesStyle.Instance is not initialized.");
				}
			}
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	public static bool IsASmallDevice()
	{
		// Get Metrics
		var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

		var width = mainDisplayInfo.Width;
		var height = mainDisplayInfo.Height;

		return (width <= smallWightResolution && height <= smallHeightResolution);
	}

	protected override void OnStart()
	{
		try
		{
			base.OnStart();
			Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	protected override void OnSleep()
	{
		try
		{
			base.OnSleep();
			Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	protected override void OnResume()
	{
		try
		{
			base.OnResume();
			Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	protected override void OnHandlerChanged()
	{
		try
		{
			base.OnHandlerChanged();
			mauiContext = Handler.MauiContext;
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
	{
		try
		{
			CommonUtils.CheckInternetAccess(e);
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	//Deep Linking / Universal Links
	protected override void OnAppLinkRequestReceived(Uri uri)
	{
		try
		{
			base.OnAppLinkRequestReceived(uri);
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}
	
	public static List<LanguageModel> Languages => new List<LanguageModel>()
	{
		new LanguageModel() {
			Name = "English",
			Code = "en",
			Language = AppLanguage.English,
			FlowDirection = FlowDirection.LeftToRight
		},
		new LanguageModel() {
			Name = "العربية",
			Code = "ar",
			Language = AppLanguage.Arabic,
			FlowDirection = FlowDirection.RightToLeft
		},
	};
}