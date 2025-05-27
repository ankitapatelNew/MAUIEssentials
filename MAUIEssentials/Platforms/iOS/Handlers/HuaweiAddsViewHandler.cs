using MAUIEssentials.AppCode.Controls;
using Microsoft.Maui.Handlers;
using UIKit;

namespace MAUIEssentials.Platforms.iOS.Handlers
{
    public class HuaweiAddsViewHandler : ViewHandler<HuaweiAddsView, UIView>
    {
        public HuaweiAddsViewHandler() : base(ViewMapper)
        {
        }

        public static IPropertyMapper<HuaweiAddsView, HuaweiAddsViewHandler> ViewMapper = new PropertyMapper<HuaweiAddsView, HuaweiAddsViewHandler>(ViewHandler.ViewMapper)
        {
        };

        protected override UIView CreatePlatformView()
        {
            var nativeView = new UIView();
            nativeView.BackgroundColor = UIColor.Gray; // Example of setting a background color for the view
            return nativeView;
        }
    }
}