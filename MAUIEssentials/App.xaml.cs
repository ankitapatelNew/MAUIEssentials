using System.Globalization;
using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using MAUIEssentials.Models;
using MAUIEssentials.Styles;
using MonkeyCache.FileStore;
using Plugin.MauiMTAdmob;
using Plugin.MauiMTAdmob.Extra;

namespace MAUIEssentials;

public partial class App : Application
{
	public static IMauiContext? mauiContext;
	const int smallWightResolution = 768;
	const int smallHeightResolution = 1280;

	public static bool IsRunning { get; set; }

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
			DependencyService.Get<ICommonUtils>().StatusbarColor(AppColorResources.white.ToColor(), AppColorResources.white.ToColor(), GradientOrientation.ReverseDiagonal);

			Barrel.ApplicationId = AppInfo.PackageName;
			Barrel.Current.AutoExpire = true;
			if (!Settings.IsHuaweiApp)
			{
				CrossMauiMTAdmob.Current.UserPersonalizedAds = true;
				CrossMauiMTAdmob.Current.ComplyWithFamilyPolicies = true;
				CrossMauiMTAdmob.Current.UseRestrictedDataProcessing = true;
				CrossMauiMTAdmob.Current.TagForChildDirectedTreatment = MTTagForChildDirectedTreatment.TagForChildDirectedTreatmentUnspecified;
				CrossMauiMTAdmob.Current.MaxAdContentRating = MTMaxAdContentRating.MaxAdContentRatingG;
				CrossMauiMTAdmob.Current.TagForUnderAgeOfConsent = MTTagForUnderAgeOfConsent.TagForUnderAgeOfConsentUnspecified;
			}

			SetAppLanguage();
			ChangeFontStyles(true);
			SetCurrentResources();

			CommonUtils.CheckInternetAccess(new ConnectivityChangedEventArgs(Connectivity.NetworkAccess, Connectivity.ConnectionProfiles));
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	protected override void OnStart()
	{
		try
		{
			base.OnStart();
			Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

			FirebaseEssentials.Shared.CrossFirebaseEssentials.Notifications.OnNotificationOpened -= OnNotificationOpened;
			FirebaseEssentials.Shared.CrossFirebaseEssentials.Notifications.OnNotificationReceived -= OnNotificationReceived;
			FirebaseEssentials.Shared.CrossFirebaseEssentials.Notifications.OnNotificationStatus -= OnNotificationStatus;

			FirebaseEssentials.Shared.CrossFirebaseEssentials.Notifications.OnNotificationOpened += OnNotificationOpened;
			FirebaseEssentials.Shared.CrossFirebaseEssentials.Notifications.OnNotificationReceived += OnNotificationReceived;
			FirebaseEssentials.Shared.CrossFirebaseEssentials.Notifications.OnNotificationStatus += OnNotificationStatus;

			IsRunning = true;
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	private void OnNotificationOpened(object source, FirebaseEssentials.Shared.FirebasePushNotificationResponseEventArgs e)
	{
		throw new NotImplementedException();
	}

	private void OnNotificationReceived(object source, FirebaseEssentials.Shared.FirebasePushNotificationDataEventArgs e)
	{
		throw new NotImplementedException();
	}

	private void OnNotificationStatus(object source, FirebaseEssentials.Shared.FirebasePushNotificationErrorEventArgs e)
	{
		throw new NotImplementedException();
	}

	protected override void OnSleep()
	{
		try
		{
			base.OnSleep();
			Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
			IsRunning = false;
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
			IsRunning = true;
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

			if (IsRunning)
			{
				NavigateDeepLink();
			}
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	void NavigateDeepLink()
	{
		try
		{

		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	public static void SetAppLanguage()
	{
		if (Settings.AppLanguage == null)
		{
			Settings.AppLanguage = Languages.FirstOrDefault();
		}
		LocalizationResources.Culture = new CultureInfo(Settings.AppLanguage?.Code ?? string.Empty);

		Current.Resources["ArrowImageRotation"] = Settings.AppLanguage?.Language == AppLanguage.Arabic ? 90 : 270;
		Current.Resources["BackImageRotation"] = Settings.AppLanguage?.Language == AppLanguage.Arabic ? 180 : 0;
		Current.Resources["PreviousImageRotation"] = Settings.AppLanguage?.Language == AppLanguage.Arabic ? 270 : 90;
		Current.Resources["NextImageRotation"] = Settings.AppLanguage?.Language == AppLanguage.Arabic ? 90 : 270;
		Current.Resources["AppFlowDirection"] = Settings.AppLanguage?.FlowDirection;
		Current.Resources["ViewScaleX"] = DeviceInfo.Platform == DevicePlatform.iOS && Settings.AppLanguage?.Language == AppLanguage.Arabic ? -1 : 1;
		Current.Resources["TutorialCardScaleX"] = Settings.AppLanguage?.Language == AppLanguage.Arabic ? -1 : 1;
		Current.Resources["SwitchScaleX"] = DeviceInfo.Platform == DevicePlatform.Android && Settings.AppLanguage?.Language == AppLanguage.Arabic ? -1 : 1;
		Current.Resources["TooltipPosition"] = Settings.AppLanguage?.Language == AppLanguage.Arabic ? TooltipPosition.Right : TooltipPosition.Left;
		Current.Resources["LeftArrow"] = Settings.AppLanguage?.Language == AppLanguage.Arabic ? "ic_arrow_right" : "ic_arrow_left";
		Current.Resources["RigthArrow"] = Settings.AppLanguage?.Language == AppLanguage.Arabic ? "ic_arrow_left" : "ic_arrow_right";
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

	public static void ChangeFontStyles(bool isMain = false)
	{
		try
		{
			if (!isMain)
			{
				if (Current.Resources.MergedDictionaries.Any(x => x.GetType() == typeof(EnglishFontStyles))
					&& Settings.AppLanguage?.Language == AppLanguage.Arabic)
				{
					var englishStyles = Current.Resources.MergedDictionaries.FirstOrDefault(x => x.GetType() == typeof(EnglishFontStyles));

					Current.Resources.MergedDictionaries.Remove(englishStyles);
					Current.Resources.MergedDictionaries.Add(ArabicFontStyles.Instance);

				}
				else if (Current.Resources.MergedDictionaries.Any(x => x.GetType() == typeof(ArabicFontStyles))
				&& Settings.AppLanguage?.Language == AppLanguage.English)
				{
					var arabicStyles = Current.Resources.MergedDictionaries.FirstOrDefault(x => x.GetType() == typeof(ArabicFontStyles));

					Current.Resources.MergedDictionaries.Remove(arabicStyles);
					Current.Resources.MergedDictionaries.Add(EnglishFontStyles.Instance);
				}
			}
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
			dictionary.MergedDictionaries.Remove(ArabicFontStyles.Instance);
			dictionary.MergedDictionaries.Remove(EnglishFontStyles.Instance);
			dictionary.MergedDictionaries.Remove(IPhoneXDevicesStyle.Instance);
			dictionary.MergedDictionaries.Remove(SmallDevicesStyle.Instance);
			dictionary.MergedDictionaries.Remove(DefaultDevicesStyle.Instance);

			if (Settings.AppLanguage?.Language == AppLanguage.Arabic)
			{
				if (ArabicFontStyles.Instance != null)
				{
					dictionary.MergedDictionaries.Add(ArabicFontStyles.Instance);
				}
				else
				{
					throw new InvalidOperationException("ArabicFontStyles.Instance is not initialized.");
				}
			}
			else
			{
				if (EnglishFontStyles.Instance != null)
				{
					dictionary.MergedDictionaries.Add(EnglishFontStyles.Instance);
				}
				else
				{
					throw new InvalidOperationException("EnglishFontStyles.Instance is not initialized.");
				}
			}

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

		return width <= smallWightResolution && height <= smallHeightResolution;
	}
	
	public void UpdateLocation()
    {
        CommonUtils.GetUserLocation().ConfigureAwait(false);
    }
}