using MAUIEssentials.AppCode.Controls;
using Microsoft.Maui.Handlers;

namespace MAUIEssentials.Platforms.Android.Handlers
{
    public class HuaweiAddsViewHandler : ViewHandler<HuaweiAddsView, global::Android.Views.View>
    { 
        public HuaweiAddsViewHandler() : base(ViewMapper)
        {
        }

        // ViewMapper defines how properties from HuaweiAddsView are mapped to the Android View
        public static IPropertyMapper<HuaweiAddsView, HuaweiAddsViewHandler> ViewMapper = new PropertyMapper<HuaweiAddsView, HuaweiAddsViewHandler>(ViewHandler.ViewMapper)
        {
        };

        // This method is called to create the native Android view
        protected override global::Android.Views.View CreatePlatformView()
        {
            return new global::Android.Views.View(Platform.AppContext);
        }

    }
}