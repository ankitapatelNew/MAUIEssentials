namespace MAUIEssentials.Platforms.Android.MapHandlers
{
    public static class MapViewExtensions
	{
		[MethodImpl(MethodImplOptions.NoInlining)]

		internal static Task<GoogleMap> getGoogleMapAsync(this MapView self)
		{
			var comp = new TaskCompletionSource<GoogleMap>();
			self.GetMapAsync(new OnMapReadyCallback(map =>
			{
				comp.SetResult(map);
			}));

			return comp.Task;
		}
	}

	internal class OnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
	{
		private readonly Action<GoogleMap> _handler;

		public OnMapReadyCallback(Action<GoogleMap> handler)
		{
			try
			{
				_handler = handler;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

        public void OnMapReady(GoogleMap googleMap)
        {
			try
			{
				_handler?.Invoke(googleMap);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
        }
    }
}
