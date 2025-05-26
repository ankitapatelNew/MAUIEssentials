using AsyncAwaitBestPractices;

namespace MAUIEssentials.AppCode.Controls
{
    public class HtmlLabel : Label
    {
        public static readonly BindableProperty UnderlineTextProperty =
            BindableProperty.Create(nameof(UnderlineText), typeof(bool), typeof(HtmlLabel), true);

        public bool UnderlineText
        {
            get => (bool)GetValue(UnderlineTextProperty);
            set => SetValue(UnderlineTextProperty, value);
        }

        public static readonly BindableProperty LinkColorProperty =
            BindableProperty.Create(nameof(LinkColor), typeof(Color), typeof(HtmlLabel), default);

        public Color LinkColor
        {
            get => (Color)GetValue(LinkColorProperty);
            set => SetValue(LinkColorProperty, value);
        }

        public static readonly BindableProperty BrowserLaunchOptionsProperty =
            BindableProperty.Create(nameof(BrowserLaunchOptions), typeof(BrowserLaunchOptions), typeof(HtmlLabel), default);

        public BrowserLaunchOptions BrowserLaunchOptions
        {
            get => (BrowserLaunchOptions)GetValue(BrowserLaunchOptionsProperty);
            set => SetValue(BrowserLaunchOptionsProperty, value);
        }

        public static readonly BindableProperty AndroidLegacyModeProperty =
            BindableProperty.Create(nameof(AndroidLegacyMode), typeof(bool), typeof(HtmlLabel), default);

        public bool AndroidLegacyMode
        {
            get => (bool)GetValue(AndroidLegacyModeProperty);
            set => SetValue(AndroidLegacyModeProperty, value);
        }

        public static readonly BindableProperty AndroidListIndentProperty =
            BindableProperty.Create(nameof(AndroidListIndent), typeof(int), typeof(HtmlLabel), defaultValue: 20);

        public int AndroidListIndent
        {
            get { return (int)GetValue(AndroidListIndentProperty); }
            set { SetValue(AndroidListIndentProperty, value); }
        }

        public static readonly BindableProperty IsOverrideLinkProperty =
            BindableProperty.Create(nameof(IsOverrideLink), typeof(bool), typeof(HtmlLabel), false);

        public bool IsOverrideLink
        {
            get => (bool)GetValue(IsOverrideLinkProperty);
            set => SetValue(IsOverrideLinkProperty, value);
        }

        readonly WeakEventManager<WebNavigatingEventArgs> navigatingEventManager
            = new WeakEventManager<WebNavigatingEventArgs>();

        readonly WeakEventManager<WebNavigatingEventArgs> navigatedEventManager
            = new WeakEventManager<WebNavigatingEventArgs>();

        readonly AsyncAwaitBestPractices.WeakEventManager linkEventManager = new AsyncAwaitBestPractices.WeakEventManager();

        public event EventHandler<WebNavigatingEventArgs> Navigating
        {
            add => navigatingEventManager.AddEventHandler(value);
            remove => navigatingEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<WebNavigatingEventArgs> Navigated
        {
            add => navigatedEventManager.AddEventHandler(value);
            remove => navigatedEventManager.RemoveEventHandler(value);
        }

        public event EventHandler OverrideLinkClicked
        {
            add => linkEventManager.AddEventHandler(value);
            remove => linkEventManager.RemoveEventHandler(value);
        }

        internal void SendNavigating(WebNavigatingEventArgs args)
        {
            navigatingEventManager?.RaiseEvent(this, args, nameof(Navigating));
        }

        internal void SendNavigated(WebNavigatingEventArgs args)
        {
            navigatedEventManager?.RaiseEvent(this, args, nameof(Navigated));
        }

        internal void OnOverrideLinkClick()
        {
            linkEventManager?.RaiseEvent(this, EventArgs.Empty, nameof(OverrideLinkClicked));
        }
    }
}