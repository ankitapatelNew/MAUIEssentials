using Plugin.MauiMTAdmob.Extra;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.Services;

namespace MAUIEssentials.AppCode.Controls
{
	public partial class AddsView : Grid
    {
        public event EventHandler AdsClicked;
        public event EventHandler AdsClosed;
        public event EventHandler AdsImpression;
        public event EventHandler AdsOpened;
        public event EventHandler<System.EventArgs> AdsFailedToLoad;
        public event EventHandler AdsLeftApplication;
        public event EventHandler AdsLoaded;

        public static readonly BindableProperty IsAdsVisibleProperty =
            BindableProperty.Create(nameof(IsAdsVisible), typeof(bool), typeof(AddsView), false);

        public static readonly BindableProperty AdsIdProperty =
            BindableProperty.Create(nameof(AdsId), typeof(string), typeof(AddsView), string.Empty);

        public bool IsAdsVisible
        {
            get => (bool)GetValue(IsAdsVisibleProperty);
            set => SetValue(IsAdsVisibleProperty, value);
        }

        public string AdsId
        {
            get => (string)GetValue(AdsIdProperty);
            set => SetValue(AdsIdProperty, value);
        }

        public AddsView()
        {
            InitializeComponent();

            var isHuawei = Settings.IsHuaweiApp;
            if (Settings.AppType == AppTypeEnum.Owner)
            {
                adView.AdsId = DeviceInfo.Platform == DevicePlatform.Android
                    ? isHuawei ? ServiceConstants.OwnerBannerAdHuawei : ServiceConstants.OwnerBannerAdAndroid
                    : ServiceConstants.OwnerBannerAdiOS;
            }
            else
            {
                adView.AdsId = DeviceInfo.Platform == DevicePlatform.Android
                    ? isHuawei ? ServiceConstants.BannerAdHuawei : ServiceConstants.BannerAdAndroid
                    : ServiceConstants.BannerAdiOS;
            }
            huaweiAddsView.IsVisible = false;
            if (isHuawei)
            {
                huaweiAddsView.IsVisible = true;
            }
            else
            {
                adView.IsVisible = true;
            }
        }

        public void AdClicked(object sender, EventArgs e)
        {
            AdsClicked?.Invoke(sender, e);
        }

        public void AdClosed(object sender, EventArgs e)
        {
            AdsClosed?.Invoke(sender, e);
        }

        public void AdImpression(object sender, EventArgs e)
        {
            AdsImpression?.Invoke(sender, e);
        }

        public void AdOpened(object sender, EventArgs e)
        {
            AdsOpened?.Invoke(sender, e);
        }

        public void AdLeftApplication(object sender, EventArgs e)
        {
            AdsLeftApplication?.Invoke(sender, e);
        }

        public void AdLoaded(object sender, EventArgs e)
        {
            AdsLoaded?.Invoke(sender, e);
        }

        public void AdFailedToLoad(System.Object sender, MTEventArgs e)
        {
            AdsFailedToLoad?.Invoke(sender, e);
        }
    }
}