using Foundation;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.Platforms.iOS.Renderers;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using WebKit;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace MAUIEssentials.Platforms.iOS.Renderers
{
    public class CustomWebViewRenderer : ViewRenderer<CustomWebView, WKWebView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                // Create a WKWebViewConfiguration and set AllowsInlineMediaPlayback to true
                var configuration = new WKWebViewConfiguration();
                configuration.AllowsInlineMediaPlayback = true;

                // Create a WKWebView with the custom configuration
                var webView = new WKWebView(Frame, configuration);

                // Set the WKWebView as the native control
                SetNativeControl(webView);
            }

            if (e.NewElement != null)
            {
                // Load the URL into the WKWebView
                if (Element?.Source is HtmlWebViewSource htmlSource)
                {
                    // Load HTML content with a base URL into the WKWebView
                    if (!string.IsNullOrWhiteSpace(htmlSource.Html))
                    {
                        Control?.LoadHtmlString(htmlSource.Html, null);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(Element?.Source?.ToString()))
                    {
                        Control?.LoadRequest(new NSUrlRequest(new NSUrl(Element.Source.ToString())));
                    }
                }
            }
        }
    }
}
