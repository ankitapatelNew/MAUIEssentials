using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Views;
using Microsoft.Maui.Maps.Handlers;

namespace MAUIEssentials.Platforms.Android.MapHandlers
{
    public class OnInfoWindowAdapterListener : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
    {
        public MapHandler? mapHandler { get; set; }

        public string? layoutInflaterServices;

        public global::Android.Views.View? GetInfoContents(Marker marker)
        {
            if (Platform.AppContext.GetSystemService(layoutInflaterServices) is LayoutInflater)
            {
                global::Android.Views.View? view = null;

                return view;
            }
            return null;
        }

        public global::Android.Views.View? GetInfoWindow(Marker marker)
        {
            return null;
        }
    }
}