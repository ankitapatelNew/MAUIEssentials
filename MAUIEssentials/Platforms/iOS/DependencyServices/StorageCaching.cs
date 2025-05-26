using Foundation;
using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.Platforms.iOS.DependencyServices
{
    public class StorageCaching : IStorageCaching
    {
        public void loadStorageCaching()
        {
            try
            {
                var configuration = NSUrlSessionConfiguration.DefaultSessionConfiguration;
                configuration.URLCache = new NSUrlCache(0, 0, "");

                NSUrlSession seeion = NSUrlSession.FromConfiguration(configuration);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}