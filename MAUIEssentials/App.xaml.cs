namespace MAUIEssentials;

public partial class App : Application
{
	const int smallWightResolution = 768;
	const int smallHeightResolution = 1280;

	static App _instance;
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
	
	//Timer set like :- new MyTimer(TimeSpan.FromSeconds(10), (methodname));
	public class MyTimer
	{
		private readonly TimeSpan timespan;
		private readonly Action callback;

		private CancellationTokenSource cancellation;
		private IDispatcher? dispatcher;

		public bool IsRunning { get; set; }

		public MyTimer(TimeSpan timespan, Action callback)
		{
			try
			{
				this.timespan = timespan;
				this.callback = callback;
				cancellation = new CancellationTokenSource();
				dispatcher = Current?.Dispatcher;
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		public void Start()
		{
			if (IsRunning)
				return; // Prevent multiple timers

			IsRunning = true;
			CancellationTokenSource cts = cancellation; // safe copy

			dispatcher?.StartTimer(timespan, () =>
			{
				if (cts.IsCancellationRequested)
				{
					IsRunning = false;
					return false; // Stop the timer
				}

				callback.Invoke();
				return true; // Continue the timer
			});
		}

		public void Stop()
		{
			try
			{
				IsRunning = false;
				Interlocked.Exchange(ref cancellation, new CancellationTokenSource()).Cancel();
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}
	}
}