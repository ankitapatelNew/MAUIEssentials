namespace MAUIEssentials.Platforms.Android.MapHandlers
{
    public class MapIconFutureTarget : CustomTarget
	{
        readonly CustomPin customPin;

		public MapIconFutureTarget(CustomPin pin)
        {
            try
            {
                customPin = pin;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public override void OnLoadCleared(Drawable p0)
        {
        }

        public override void OnResourceReady(Java.Lang.Object resource, ITransition transition)
        {
            try
            {
                var bitmapDescriptor = BitmapDescriptorFactory.FromBitmap(resource as global::Android.Graphics.Bitmap);
                var optionMarker = CommanMapHandler.GetMarkerForPin(customPin);

                if (optionMarker != null)
                {
                    optionMarker.HideInfoWindow();
                    optionMarker.SetIcon(bitmapDescriptor);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Marker getting null");
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }    
}
