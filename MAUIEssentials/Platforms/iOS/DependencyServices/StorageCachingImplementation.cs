using FirebaseEssentials.Shared;
using Foundation;

namespace MAUIEssentials.DependencyServices
{
    public sealed class StorageCachingImplementation : DisposableBase, IStorageCaching
    {
        public void LoadStorageCaching()
        {
            var configuration = NSUrlSessionConfiguration.DefaultSessionConfiguration;
            configuration.URLCache = new NSUrlCache(0, 0, "");
            NSUrlSession seeion = NSUrlSession.FromConfiguration(configuration);
        }
    }
}