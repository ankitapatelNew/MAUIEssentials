namespace MAUIEssentials.AppCode.Controls
{
    public class HtmlLabel : Microsoft.Maui.Controls.Label
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

        public Microsoft.Maui.Graphics.Color LinkColor
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

        public event EventHandler<WebNavigatingEventArgs> Navigating;

        public event EventHandler<WebNavigatingEventArgs> Navigated;

        public event EventHandler OverrideLinkClicked;

        internal void SendNavigating(WebNavigatingEventArgs args)
        {
            Navigating?.Invoke(this, args);
        }

        internal void SendNavigated(WebNavigatingEventArgs args)
        {
            Navigated?.Invoke(this, args);
        }

        internal void OnOverrideLinkClick()
        {
            OverrideLinkClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}