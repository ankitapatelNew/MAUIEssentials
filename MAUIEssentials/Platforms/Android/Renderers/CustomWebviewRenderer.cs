using Android.Views;
using Android.Content;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Compatibility;
using MAUIEssentials.Platforms.Android.Renderers;

[assembly: ExportRenderer(typeof(WebView), typeof(CustomWebviewRenderer))]
namespace MAUIEssentials.Platforms.Android.Renderers
{
    public class CustomWebviewRenderer : WebViewRenderer
    {
        public CustomWebviewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                SetDefaultSettings();
            }
        }

        private void SetDefaultSettings()
        {
            var settings = Control.Settings;

            if (settings != null)
            {
                settings.JavaScriptEnabled = true;
                settings.LoadsImagesAutomatically = true;
                settings.SaveFormData = true;
                settings.BuiltInZoomControls = true;
                settings.DisplayZoomControls = false;
                settings.DomStorageEnabled = true;

                SetLayerType(LayerType.Hardware, null);
            }
        }
    }
}
