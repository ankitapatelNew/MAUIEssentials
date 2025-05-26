using Plugin.MauiMTAdmob.Extra;

namespace MAUIEssentials.AppCode.Controls
{
    public class HuaweiAddsView : View
    {
        public event EventHandler AdsClicked;
        public event EventHandler AdsClosed;
        public event EventHandler AdsImpression;
        public event EventHandler AdsOpened;
        public event EventHandler<MTEventArgs> AdsFailedToLoad;
        public event EventHandler AdsLeftApplication;
        public event EventHandler AdsLoaded;

        public HuaweiAddsView()
        {
        }

        public static readonly BindableProperty AdIdProperty =
            BindableProperty.Create(nameof(AdId), typeof(string), typeof(HuaweiAddsView), string.Empty);

        public string AdId
        {
            get => (string)GetValue(AdIdProperty);
            set => SetValue(AdIdProperty, value);
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

        public void AdFailedToLoad(object sender, MTEventArgs e)
        {
            AdsFailedToLoad?.Invoke(sender, e);
        }

        public void AdLeftApplication(object sender, EventArgs e)
        {
            AdsLeftApplication?.Invoke(sender, e);
        }

        public void AdLoaded(object sender, EventArgs e)
        {
            AdsLoaded?.Invoke(sender, e);
        }
    }
}
