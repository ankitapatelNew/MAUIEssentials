using System;

namespace MAUIEssentials.AppCode.Controls
{
    public class CustomWebView : WebView
    {
        public CustomWebView()
        {
        }

        public static readonly BindableProperty UrlProperty =
            BindableProperty.Create(nameof(Url), typeof(string), typeof(Border), string.Empty);

        public string Url
        {
            get => (string)GetValue(UrlProperty);
            set => SetValue(UrlProperty, value);
        }
    }
}
