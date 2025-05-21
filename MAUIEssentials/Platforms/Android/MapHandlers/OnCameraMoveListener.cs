namespace MAUIEssentials.Platforms.Android.MapHandlers
{
    public class OnCameraMoveListener : Java.Lang.Object, GoogleMap.IOnCameraMoveListener
    {
        public MapHandler? mapHandler { get; set; }

        public GoogleMap? nativeMap { get; set; }

        public void OnCameraMove()
        {
            try
            {
                UpdateVisibleRegion();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateVisibleRegion()
        {
            try
            {
                GoogleMap? map = nativeMap;
                if (map == null)
                {
                    return;
                }

                Projection projection = map.Projection;

                int width = mapHandler.PlatformView.Width;
                int height = mapHandler.PlatformView.Height;

                LatLng ul = projection.FromScreenLocation(new global::Android.Graphics.Point(0, 0));
                LatLng ur = projection.FromScreenLocation(new global::Android.Graphics.Point(width, 0));
                LatLng ll = projection.FromScreenLocation(new global::Android.Graphics.Point(0, height));
                LatLng lr = projection.FromScreenLocation(new global::Android.Graphics.Point(width, height));

                double dlat = Math.Max(Math.Abs(ul.Latitude - lr.Latitude), Math.Abs(ur.Latitude - ll.Latitude));
                double dlong = Math.Max(Math.Abs(ul.Longitude - lr.Longitude), Math.Abs(ur.Longitude - ll.Longitude));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
