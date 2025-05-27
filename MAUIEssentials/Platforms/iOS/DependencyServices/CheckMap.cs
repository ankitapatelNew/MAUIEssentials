using Foundation;
using MAUIEssentials.AppCode.DependencyServices;
using UIKit;

namespace MAUIEssentials.Platforms.iOS.DependencyServices
{
    public class CheckMap : ICheckMap
    {
        public bool IsAppleMap()
        {
            return UIApplication.SharedApplication.CanOpenUrl(NSUrl.FromString("maps://"));
        }

        public bool IsGoogleMap()
        {
            return UIApplication.SharedApplication.CanOpenUrl(NSUrl.FromString("comgooglemaps://"));
        }

        public void OpenGoogleMap(double latitude, double longitude)
        {
            var url = NSUrl.FromString($"comgooglemaps://?q={latitude},{longitude}");
            UIApplication.SharedApplication.OpenUrl(url, new UIApplicationOpenUrlOptions(), null);
        }

        public void OpenGoogleMap(Location source, Location destination)
        {
            var url = NSUrl.FromString($"comgooglemaps://?saddr={source.Latitude},{source.Longitude}&daddr={destination.Latitude},{destination.Longitude})&directionsmode=driving");
            UIApplication.SharedApplication.OpenUrl(url, new UIApplicationOpenUrlOptions(), null);
        }
    }
}
