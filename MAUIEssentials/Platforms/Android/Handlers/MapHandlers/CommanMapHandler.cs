using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using AndroidX.Core.Content;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Bumptech.Glide.Signature;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Platform;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.Platforms.Android.MapHandlers;
using ACircle = Android.Gms.Maps.Model.Circle;
using APolygon = Android.Gms.Maps.Model.Polygon;
using APolyline = Android.Gms.Maps.Model.Polyline;
using Location = Microsoft.Maui.Devices.Sensors.Location;
using Path = System.IO.Path;
using XCircle = Microsoft.Maui.Controls.Maps.Circle;
using XPolygon = Microsoft.Maui.Controls.Maps.Polygon;
using XPolyline = Microsoft.Maui.Controls.Maps.Polyline;

namespace MAUIEssentials
{
    public partial class CommanMapHandler : MapHandler
    {
        private readonly OnCameraMoveListener onCameraMoveListener = new();

        private readonly OnInfoWindowAdapterListener onInfoWindowAdapterListener = new();

        const string moveMessageName = "MapMoveToRegion";

        static Bundle? s_bundle;

        static bool _disposed;

        static bool _init = true;

        static Marker? marker;

        static Context context = Platform.AppContext;

        static List<Marker>? _markers;

        static List<APolyline>? _polylines;

        static List<ACircle>? _circles;

        static List<APolygon>? _polygons;

        static IMapHandler? _handler;

        static CustomMap? _map;

        static GoogleMap? nativeMap;

        internal static Bundle bundle
        {
            set
            {
                s_bundle = value;
            }
        }

        protected override MapView CreatePlatformView()
        {
            Glide.Get(Platform.AppContext).SetMemoryCategory(MemoryCategory.Low);

            MapView mapView = new MapView(Context);
            mapView.OnCreate(s_bundle);
            mapView.OnResume();

            return mapView;
        }

        public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return base.GetDesiredSize(widthConstraint, heightConstraint);
        }

        protected override void DisconnectHandler(MapView platformView)
        {
            try
            {
                base.DisconnectHandler(platformView);
                DisposeView(_map);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void DisposeView(CustomMap map)
        {
            try
            {
                if (map != null)
                {
                    MessagingCenter.Unsubscribe<CustomMap, MapSpan>(this, moveMessageName);

                    ((ObservableCollection<Pin>)map.Pins).CollectionChanged -= OnPinCollectionChanged;
                    foreach (Pin pin in map.Pins)
                    {
                        pin.PropertyChanged -= PinOnPropertyChanged;
                    }

                    ((ObservableCollection<MapElement>)map.MapElements).CollectionChanged -= OnMapElementCollectionChanged;
                    foreach (MapElement child in map.MapElements)
                    {
                        child.PropertyChanged -= MapElementPropertyChanged;
                    }

                    map.ShowInfoWindowAction -= MauiMap_ShowInfoWindowAction;
                    map.HideInfoWindowAction -= MauiMap_HideInfoWindowAction;
                    map.UpdateMarker -= MauiMap_UpdateMarker;
                }

                if (nativeMap != null)
                {
                    nativeMap.MyLocationEnabled = false;
                    nativeMap.TrafficEnabled = false;

                    nativeMap.SetOnCameraMoveListener(null);
                    nativeMap.MarkerClick -= NativeMap_MarkerClick;
                    nativeMap.InfoWindowClick -= NativeMap_InfoWindowClick;
                    nativeMap.MapClick -= OnMapClick;
                    nativeMap.CameraIdle -= NativeMap_CameraIdle;
                    nativeMap.CameraMoveStarted -= NativeMap_CameraMove;

                    nativeMap.Clear();
                    nativeMap.Dispose();
                    nativeMap = null;

                    marker?.Dispose();
                    _markers?.Clear();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public void Connect()
        {
            try
            {
                ConnectHandler(PlatformView);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public void CleanUp()
        {
            try
            {
                if (PlatformView != null)
                {
                    DisconnectHandler(PlatformView);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected override async void ConnectHandler(MapView platformView)
        {
            try
            {
                base.ConnectHandler(platformView);

                var wrapper = new TouchableWrapper(Context);
                wrapper.SetBackgroundColor(Android.Graphics.Color.Transparent);

                platformView.AddView(wrapper, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

                var scrollView = platformView.Parent;

                wrapper.TouchUp += () =>
                {
                    if (scrollView != null)
                    {
                        scrollView.RequestDisallowInterceptTouchEvent(false);
                    }
                };

                wrapper.TouchDown += () =>
                {
                    if (scrollView != null)
                    {
                        scrollView.RequestDisallowInterceptTouchEvent(true);
                    }
                };

                _map = VirtualView as CustomMap;
                if (_map == null)
                {
                    Console.WriteLine("ConnectHandler: _map is NULL!");
                    return;
                }

                nativeMap = await platformView.getGoogleMapAsync();
                if (nativeMap == null)
                {
                    Console.WriteLine("ConnectHandler: Failed to initialize nativeMap!");
                    return;
                }
                OnMapReady();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void UpdateIsShowingUser(IMapHandler handler, CustomMap map)
        {
            try
            {
                _handler = handler;
                _map = map;
                SetUserVisible();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void UpdateMapType(IMapHandler handler, CustomMap map)
        {
            try
            {
                _handler = handler;
                _map = map;
                if (_map.VisibleRegion != null)
                {
                    var span = _map.VisibleRegion.ClampLatitude(85, -85);
                    MoveToRegion(span, true);
                }
                ((INotifyCollectionChanged)map.Pins).CollectionChanged += OnPinCollectionChanged;
                ((INotifyCollectionChanged)map.MapElements).CollectionChanged += OnMapElementCollectionChanged;

                map.NativeGetMapCenterLocation = new Func<Location>(GetMapCenterLocation);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void UpdateHasScrollEnabled(IMapHandler handler, CustomMap map)
        {
            try
            {
                _handler = handler;
                _map = map;
                if (nativeMap != null)
                {
                    nativeMap.UiSettings.ScrollGesturesEnabled = map.IsScrollEnabled;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void UpdateHasZoomEnabled(IMapHandler handler, CustomMap map)
        {
            try
            {
                _handler = handler;
                _map = map;
                if (nativeMap != null)
                {
                    nativeMap.UiSettings.ZoomControlsEnabled = map.IsZoomEnabled;
                    nativeMap.UiSettings.ZoomGesturesEnabled = map.IsZoomEnabled;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void UpdateTrafficEnabled(IMapHandler handler, CustomMap map)
        {
            try
            {
                _handler = handler;
                _map = map;
                if (nativeMap != null)
                {
                    nativeMap.TrafficEnabled = map.IsTrafficEnabled;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static Location GetMapCenterLocation()
        {
            //METHOD 1 FORM CameraPosition
            var centerPosition = nativeMap.CameraPosition.Target;

            //METHOD 2 CALCULATE PROJECTION
            var visibleRegion = nativeMap.Projection.VisibleRegion;
            var x = nativeMap.Projection.ToScreenLocation(visibleRegion.FarRight);
            var y = nativeMap.Projection.ToScreenLocation(visibleRegion.NearLeft);
            var centerPoint = new Android.Graphics.Point(x.X / 2, y.Y / 2);
            nativeMap.Projection.FromScreenLocation(centerPoint);

            //BOTH RETURNS THE SAME RESULT
            return new Location(centerPosition.Latitude, centerPosition.Longitude);
        }

        void OnMapReady()
        {
            try
            {
                if (_map == null)
                {
                    Console.WriteLine("OnMapReady: _map is NULL!");
                    return;
                }

                if (nativeMap == null)
                {
                    Console.WriteLine("OnMapReady: nativeMap is NULL!");
                    return;
                }

                onCameraMoveListener.mapHandler = this;
                onCameraMoveListener.nativeMap = nativeMap;
                onInfoWindowAdapterListener.mapHandler = this;
                nativeMap.SetOnCameraMoveListener(onCameraMoveListener);
                nativeMap.MarkerClick += NativeMap_MarkerClick;
                nativeMap.MapClick += OnMapClick;
                nativeMap.CameraIdle += NativeMap_CameraIdle;
                nativeMap.CameraMoveStarted += NativeMap_CameraMove;
                nativeMap.InfoWindowClick += NativeMap_InfoWindowClick;
                nativeMap.SetInfoWindowAdapter(onInfoWindowAdapterListener);

                // Hide zoom controls
                nativeMap.TrafficEnabled = _map.IsTrafficEnabled;
                nativeMap.UiSettings.ZoomControlsEnabled = _map.IsZoomEnabled;
                nativeMap.UiSettings.ZoomGesturesEnabled = _map.IsZoomEnabled;
                nativeMap.UiSettings.ScrollGesturesEnabled = _map.IsScrollEnabled;
                nativeMap.UiSettings.ZoomControlsEnabled = false;
                nativeMap.UiSettings.MapToolbarEnabled = false;
                //nativeMap.SetOnMarkerClickListener(this);

                SetUserVisible();
                SetMapType();

                _map.ShowInfoWindowAction += MauiMap_ShowInfoWindowAction;
                _map.HideInfoWindowAction += MauiMap_HideInfoWindowAction;
                _map.UpdateMarker += MauiMap_UpdateMarker;

                if (_map.VisibleRegion != null)
                {
                    var span = _map.VisibleRegion.ClampLatitude(85, -85);
                    MoveToRegion(span, false);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private static void MoveToRegion(MapSpan span, bool animate)
        {
            try
            {
                GoogleMap? map = nativeMap;
                if (map == null)
                {
                    return;
                }

                span = span.ClampLatitude(85, -85);

                var ne = new LatLng(span.Center.Latitude + span.LatitudeDegrees / 2,
                    span.Center.Longitude + span.LongitudeDegrees / 2);
                var sw = new LatLng(span.Center.Latitude - span.LatitudeDegrees / 2,
                    span.Center.Longitude - span.LongitudeDegrees / 2);

                CameraUpdate update = CameraUpdateFactory.NewLatLngBounds(new LatLngBounds(sw, ne), ScreenSize.ScreenWidth.ToInt(), ScreenSize.ScreenHeight.ToInt(), 0);

                if (_map.Height > 0 && _map.Width > 0)
                {
                    update = CameraUpdateFactory.NewLatLngBounds(new LatLngBounds(sw, ne), _map.Width.ToInt(), _map.Height.ToInt(), 0);
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MoveCamera(update, animate);
                });
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private static void MoveCamera(CameraUpdate update, bool animate)
        {
            try
            {
                if (_map.Height > 0 && _map.Width > 0 && !animate)
                {
                    nativeMap.MoveCamera(update);
                }
                else
                {
                    nativeMap.AnimateCamera(update);
                }
            }
            catch (Java.Lang.IllegalStateException exc)
            {
                System.Diagnostics.Debug.WriteLine("MoveToRegion exception: " + exc);
            }
        }

        static void MauiMap_UpdateMarker(object sender, CustomPinEventArgs e)
        {
            try
            {
                if (marker != null)
                {
                    var pin = GetPinForMarker(marker);
                    SetMarkerIcon(pin, marker);
                }

                marker = GetMarkerForPin(e.Pin);
                SetMarkerIcon(e.Pin, marker, true);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private static void NativeMap_InfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            try
            {
                var customPin = GetPinForMarker(e.Marker);

                if (customPin == null)
                {
                    return;
                }

#pragma warning disable CS0618
                //customPin.SendTap();
#pragma warning restore CS0618

                // SendInfoWindowClick() returns the value of PinClickedEventArgs.HideInfoWindow
                bool hideInfoWindow = customPin.SendInfoWindowClick();

                if (hideInfoWindow)
                {
                    marker.HideInfoWindow();
                }
                customPin.OnAnnotationTapped();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void MauiMap_ShowInfoWindowAction(object sender, CustomPinEventArgs args)
        {
            try
            {
                marker = GetMarkerForPin(args.Pin);
                marker?.ShowInfoWindow();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void MauiMap_HideInfoWindowAction(object sender, EventArgs e)
        {
            try
            {
                if (marker != null)
                {
                    marker.HideInfoWindow();
                    NativeMapClick();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void NativeMap_CameraIdle(object sender, EventArgs e)
        {
            try
            {
                _map.OnDraggingStopped(GetMapCenterLocation());
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void NativeMap_CameraMove(object sender, EventArgs e)
        {
            try
            {
                _map.OnDraggingStarted(GetMapCenterLocation());
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected static MarkerOptions CreateMarker(Pin pin)
        {
            var options = new MarkerOptions();

            options.SetPosition(new LatLng(pin.Location.Latitude, pin.Location.Longitude));

            if (pin is CustomPin customPin && customPin.CanShowInfo)
            {
                options.SetTitle(pin.Label);
                options.SetSnippet(pin.Address);
            }

            SetMarkerIconOptions(options, pin as CustomPin);
            return options;
        }

        static public void SetMarkerIconOptions(MarkerOptions options, CustomPin pin)
        {
            try
            {
                if (!string.IsNullOrEmpty(pin.PinImage))
                {
                    SetPinIcon(pin.PinImage, new Size(45, 55), pin, options);
                }
                else if (_map != null && !string.IsNullOrEmpty(_map.PinImage))
                {
                    SetPinIcon(_map.PinImage, new Size(45, 55), pin, options);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void NativeMap_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            try
            {
                if (marker != null)
                {
                    marker.HideInfoWindow();
                    NativeMapClick();
                }

                marker = e.Marker;
                var customPin = GetPinForMarker(marker);

                if (customPin == null)
                {
                    return;
                }

                // Setting e.Handled = true will prevent the info window from being presented
                // SendMarkerClick() returns the value of PinClickedEventArgs.HideInfoWindow
                bool handled = customPin.SendMarkerClick();
                e.Handled = handled;

                if (_map.IsSelectionAllowed)
                {
                    SetMarkerIcon(customPin, marker, true);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private static void OnMapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            try
            {
                //_map.SendMapClicked(new Location(e.Point.Latitude, e.Point.Longitude));
                NativeMapClick();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private static void NativeMapClick()
        {
            try
            {
                if (_map.IsSelectionAllowed && marker != null)
                {
                    var customPin = GetPinForMarker(marker);
                    SetMarkerIcon(customPin, marker);
                    marker = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private static void SetMarkerIcon(CustomPin pin, Marker marker, bool isSelected = false)
        {
            try
            {
                if (isSelected)
                {
                    if (pin != null && !string.IsNullOrEmpty(pin.SelectedPinImage))
                    {
                        SetPinIcon(pin.SelectedPinImage, new Size(40, 60), pin, marker: marker);
                    }
                    else if (_map != null && !string.IsNullOrEmpty(_map.SelectedPinImage))
                    {
                        SetPinIcon(_map.SelectedPinImage, new Size(40, 60), pin, marker: marker);
                    }
                }
                else
                {
                    if (pin != null && !string.IsNullOrEmpty(pin.PinImage))
                    {
                        SetPinIcon(pin.PinImage, new Size(45, 55), pin, marker: marker);
                    }
                    else if (_map != null && !string.IsNullOrEmpty(_map.PinImage))
                    {
                        SetPinIcon(_map.PinImage, new Size(45, 55), pin, marker: marker);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private static void SetPinIcon(string image, Size pinSize, CustomPin pin, MarkerOptions options = null, Marker marker = null)
        {
            try
            {
                if (image.StartsWith("http") || image.Contains(FileSystem.AppDataDirectory))
                {
                    var context = Platform.AppContext;

                    var resourceId = context.Resources.GetIdentifier("ic_pin_gray", "drawable", Android.App.Application.Context.PackageName);
                    if (resourceId == 0)
                    {
                        Console.WriteLine("Http Resource not found!");
                    }
                    else
                    {
                        Console.WriteLine($"Http Resource ID: {resourceId}");
                    }
                    var drawable = context.GetDrawable(resourceId);

                    using (var glide = Glide.With(context)
                        .SetDefaultRequestOptions(new RequestOptions()
                        .SetDiskCacheStrategy(DiskCacheStrategy.All)
                        .SkipMemoryCache(true)
                        .SetSignature(new ObjectKey($"{image}-{DateTime.Now:dd'/'MM}")))
                        .AsBitmap()
                        .Placeholder(drawable)
                        .Load(image))
                    {
                        if (pinSize != Size.Zero)
                        {
                            glide.Apply(new RequestOptions().Override(pinSize.Width.ToInt(), pinSize.Height.ToInt()));
                        }

                        glide.Into(new MapIconFutureTarget(pin));
                    }
                }
                else
                {
                    var drawableResId = GetDrawableResId(System.IO.Path.GetFileNameWithoutExtension(image), (int)pinSize.Width * 2, (int)pinSize.Height * 2);

                    if (options != null)
                    {
                        if (drawableResId != null)
                        {
                            options.SetIcon(BitmapDescriptorFactory.FromBitmap(drawableResId));
                        }
                        else
                        {
                            Console.WriteLine($"Error: Drawable options resource '{image}' not found.");
                        }
                    }
                    else
                    {
                        if (drawableResId != null)
                        {
                            marker?.HideInfoWindow();
                            marker?.SetIcon(BitmapDescriptorFactory.FromBitmap(drawableResId));
                        }
                        else
                        {
                            Console.WriteLine($"Error: Drawable marker resource '{image}' not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        /// <summary>
        /// Loads a drawable resource, resizes it, and returns it as a Bitmap.
        /// </summary>
        /// <param name="image">The name of the drawable resource (without extension).</param>
        /// <param name="width">The desired width of the resized image.</param>
        /// <param name="height">The desired height of the resized image.</param>
        /// <returns>The resized Bitmap, or null if the resource could not be loaded.</returns>
        public static Bitmap GetDrawableResId(string image, int width, int height)
        {
            try
            {
                var activity = Platform.CurrentActivity;
                if (activity == null)
                {
                    return null;
                }

                // Get the resource ID for the drawable
                int resID = activity.Resources.GetIdentifier(image, "drawable", activity.PackageName);
                if (resID == 0)
                {
                    return null;
                }

                // Load the drawable
                var drawable = ContextCompat.GetDrawable(activity, resID);
                if (drawable == null)
                {
                    return null;
                }

                // Convert the drawable to a Bitmap
                var bitmap = ((BitmapDrawable)drawable).Bitmap;
                if (bitmap == null)
                {
                    return null;
                }

                // Resize the Bitmap
                var resizedBitmap = Bitmap.CreateScaledBitmap(bitmap, width, height, false);
                if (resizedBitmap == null)
                {
                    return null;
                }

                return resizedBitmap;
            }
            catch (Exception ex)
            {
                ex.LogException();
                return null;
            }
        }

        static void AddPins(IList pins)
        {
            GoogleMap? map = nativeMap;
            if (map == null)
            {
                return;
            }

            if (_markers == null)
            {
                _markers = new List<Marker>();
            }

            _markers.AddRange(pins.Cast<Pin>().Select(p =>
            {
                Pin pin = p;
                var opts = CreateMarker(pin);
                var marker = map.AddMarker(opts);

                pin.PropertyChanged += PinOnPropertyChanged;

                // associate pin with marker for later lookup in event handlers
                pin.MarkerId = marker.Id;
                return marker;
            }));
        }

        static void PinOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                Pin pin = (Pin)sender;
                Marker marker = GetMarkerForPin(pin);

                if (marker == null)
                {
                    return;
                }

                if (e.PropertyName == Pin.LabelProperty.PropertyName)
                {
                    marker.Title = pin.Label;
                }
                else if (e.PropertyName == Pin.AddressProperty.PropertyName)
                {
                    marker.Snippet = pin.Address;
                }
                else if (e.PropertyName == Pin.LocationProperty.PropertyName)
                {
                    marker.Position = new LatLng(pin.Location.Latitude, pin.Location.Longitude);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static public Marker GetMarkerForPin(Pin pin)
        {
            Marker targetMarker = null;

            if (_markers != null)
            {
                for (int i = 0; i < _markers.Count; i++)
                {
                    var marker = _markers[i];

                    if (marker.Id == (string)pin.MarkerId)
                    {
                        targetMarker = marker;
                        break;
                    }
                }
            }

            return targetMarker;
        }

        protected static CustomPin GetPinForMarker(Marker marker)
        {
            CustomPin targetPin = null;
            var position = new Location(marker.Position.Latitude, marker.Position.Longitude);

            for (int i = 0; i < _map.Pins.Count; i++)
            {
                var pin = _map.Pins[i] as CustomPin;

                if (pin.Location == position && (string)pin.MarkerId == marker.Id)
                {
                    targetPin = pin;
                    break;
                }
            }

            return targetPin;
        }

        static void OnPinCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    PinCollectionChanged(e);
                });
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void PinCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        AddPins(e.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        RemovePins(e.OldItems);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        RemovePins(e.OldItems);
                        AddPins(e.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        if (_markers != null)
                        {
                            for (int i = 0; i < _markers.Count; i++)
                                _markers[i].Remove();

                            _markers = null;
                        }

                        AddPins((IList)_map?.Pins);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        //do nothing
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void RemovePins(IList pins)
        {
            try
            {
                GoogleMap? map = nativeMap;
                if (map == null)
                {
                    return;
                }

                if (_markers == null)
                {
                    return;
                }

                foreach (Pin p in pins)
                {
                    p.PropertyChanged -= PinOnPropertyChanged;
                    var marker = GetMarkerForPin(p);

                    if (marker == null)
                    {
                        continue;
                    }
                    marker.Remove();
                    _markers.Remove(marker);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void SetMapType()
        {
            try
            {
                GoogleMap? map = nativeMap;
                if (map == null)
                {
                    return;
                }

                var options = new GoogleMapOptions().InvokeLiteMode(true);

                switch (_map.MapType)
                {
                    case MapType.Street:
                        options.InvokeMapType(GoogleMap.MapTypeNormal);
                        break;
                    case MapType.Satellite:
                        options.InvokeMapType(GoogleMap.MapTypeSatellite);
                        break;
                    case MapType.Hybrid:
                        options.InvokeMapType(GoogleMap.MapTypeHybrid);
                        break;
                }
                map.MapType = options.MapType;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void MapElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                switch (sender)
                {
                    case XPolyline polyline:
                        {
                            PolylineOnPropertyChanged(polyline, e);
                            break;
                        }
                    case XPolygon polygon:
                        {
                            PolygonOnPropertyChanged(polygon, e);
                            break;
                        }
                    case XCircle circle:
                        {
                            CircleOnPropertyChanged(circle, e);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void OnMapElementCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MapElementCollectionChanged(e);
                });
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void MapElementCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        AddMapElements(e.NewItems.Cast<MapElement>());
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        RemoveMapElements(e.OldItems.Cast<MapElement>());
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        RemoveMapElements(e.OldItems.Cast<MapElement>());
                        AddMapElements(e.NewItems.Cast<MapElement>());
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        if (_polylines != null)
                        {
                            for (int i = 0; i < _polylines.Count; i++)
                                _polylines[i].Remove();

                            _polylines = null;
                        }

                        if (_polygons != null)
                        {
                            for (int i = 0; i < _polygons.Count; i++)
                                _polygons[i].Remove();

                            _polygons = null;
                        }

                        if (_circles != null)
                        {
                            for (int i = 0; i < _circles.Count; i++)
                                _circles[i].Remove();

                            _circles = null;
                        }

                        AddMapElements(_map.MapElements);
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void AddMapElements(IEnumerable<MapElement> mapElements)
        {
            try
            {
                foreach (var element in mapElements)
                {
                    element.PropertyChanged += MapElementPropertyChanged;

                    switch (element)
                    {
                        case XPolyline polyline:
                            AddPolyline(polyline);
                            break;
                        case XPolygon polygon:
                            AddPolygon(polygon);
                            break;
                        case XCircle circle:
                            AddCircle(circle);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void RemoveMapElements(IEnumerable<MapElement> mapElements)
        {
            try
            {
                foreach (var element in mapElements)
                {
                    element.PropertyChanged -= MapElementPropertyChanged;

                    switch (element)
                    {
                        case XPolyline polyline:
                            RemovePolyline(polyline);
                            break;
                        case XPolygon polygon:
                            RemovePolygon(polygon);
                            break;
                        case XCircle circle:
                            RemoveCircle(circle);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        #region Polylines

        protected static PolylineOptions CreatePolylineOptions(XPolyline polyline)
        {
            var opts = new PolylineOptions();

            opts.InvokeColor(polyline.StrokeColor.ToPlatform(Colors.Black));
            opts.InvokeWidth(polyline.StrokeWidth);

            foreach (var position in polyline.Geopath)
            {
                opts.Points.Add(new LatLng(position.Latitude, position.Longitude));
            }

            return opts;
        }

        static protected APolyline GetNativePolyline(XPolyline polyline)
        {
            APolyline? targetPolyline = null;

            if (_polylines != null)
            {
                for (int i = 0; i < _polylines.Count; i++)
                {
                    var native = _polylines[i];
                    if (native.Id == (string)polyline.MapElementId)
                    {
                        targetPolyline = native;
                        break;
                    }
                }
            }

            return targetPolyline;
        }

        static protected XPolyline GetMAUIPolyline(APolyline polyline)
        {
            XPolyline? targetPolyline = null;

            for (int i = 0; i < _map.MapElements.Count; i++)
            {
                var element = _map.MapElements[i];
                if ((string)element.MapElementId == polyline.Id)
                {
                    targetPolyline = element as XPolyline;
                    break;
                }
            }

            return targetPolyline;
        }

        static void PolylineOnPropertyChanged(XPolyline mauiPolyline, PropertyChangedEventArgs e)
        {
            try
            {
                var nativePolyline = GetNativePolyline(mauiPolyline);

                if (nativePolyline == null)
                {
                    return;
                }

                if (e.PropertyName == MapElement.StrokeColorProperty.PropertyName)
                {
                    nativePolyline.Color = mauiPolyline.StrokeColor.ToPlatform(Colors.Black);
                }
                else if (e.PropertyName == MapElement.StrokeWidthProperty.PropertyName)
                {
                    nativePolyline.Width = mauiPolyline.StrokeWidth;
                }
                else if (e.PropertyName == nameof(XPolyline.Geopath))
                {
                    nativePolyline.Points = mauiPolyline.Geopath.Select(position => new LatLng(position.Latitude, position.Longitude)).ToList();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void AddPolyline(XPolyline polyline)
        {
            try
            {
                var map = nativeMap;
                if (map == null)
                {
                    return;
                }

                if (_polylines == null)
                {
                    _polylines = new List<APolyline>();
                }

                var options = CreatePolylineOptions(polyline);
                var nativePolyline = map.AddPolyline(options);

                polyline.MapElementId = nativePolyline.Id;

                _polylines.Add(nativePolyline);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void RemovePolyline(XPolyline polyline)
        {
            try
            {
                var native = GetNativePolyline(polyline);

                if (native != null)
                {
                    native.Remove();
                    _polylines.Remove(native);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        #endregion

        #region Polygons

        protected static PolygonOptions CreatePolygonOptions(XPolygon polygon)
        {
            var opts = new PolygonOptions();

            opts.InvokeStrokeColor(polygon.StrokeColor.ToPlatform(Colors.Black));
            opts.InvokeStrokeWidth(polygon.StrokeWidth);

            if (!polygon.StrokeColor.IsDefault())
                opts.InvokeFillColor(polygon.FillColor.ToPlatform());

            // Will throw an exception when added to the map if Points is empty
            if (polygon.Geopath.Count == 0)
            {
                opts.Points.Add(new LatLng(0, 0));
            }
            else
            {
                foreach (var position in polygon.Geopath)
                {
                    opts.Points.Add(new LatLng(position.Latitude, position.Longitude));
                }
            }

            return opts;
        }

        static protected APolygon GetNativePolygon(XPolygon polygon)
        {
            APolygon targetPolygon = null;

            if (_polygons != null)
            {
                for (int i = 0; i < _polygons.Count; i++)
                {
                    var native = _polygons[i];
                    if (native.Id == (string)polygon.MapElementId)
                    {
                        targetPolygon = native;
                        break;
                    }
                }
            }

            return targetPolygon;
        }

        static protected XPolygon GetMAUIPolygon(APolygon polygon)
        {
            XPolygon targetPolygon = null;

            for (int i = 0; i < _map.MapElements.Count; i++)
            {
                var element = _map.MapElements[i];
                if ((string)element.MapElementId == polygon.Id)
                {
                    targetPolygon = (XPolygon)element;
                    break;
                }
            }

            return targetPolygon;
        }

        static void PolygonOnPropertyChanged(XPolygon polygon, PropertyChangedEventArgs e)
        {
            try
            {
                var nativePolygon = GetNativePolygon(polygon);

                if (nativePolygon == null)
                    return;

                if (e.PropertyName == MapElement.StrokeColorProperty.PropertyName)
                {
                    nativePolygon.StrokeColor = polygon.StrokeColor.ToPlatform(Colors.Black);
                }
                else if (e.PropertyName == MapElement.StrokeWidthProperty.PropertyName)
                {
                    nativePolygon.StrokeWidth = polygon.StrokeWidth;
                }
                else if (e.PropertyName == XPolygon.FillColorProperty.PropertyName)
                {
                    nativePolygon.FillColor = polygon.FillColor.ToPlatform();
                }
                else if (e.PropertyName == nameof(polygon.Geopath))
                {
                    nativePolygon.Points = polygon.Geopath.Select(p => new LatLng(p.Latitude, p.Longitude)).ToList();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void AddPolygon(XPolygon polygon)
        {
            try
            {
                var map = nativeMap;
                if (map == null)
                {
                    return;
                }

                if (_polygons == null)
                {
                    _polygons = new List<APolygon>();
                }

                var options = CreatePolygonOptions(polygon);
                var nativePolygon = map.AddPolygon(options);

                polygon.MapElementId = nativePolygon.Id;

                _polygons.Add(nativePolygon);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void RemovePolygon(XPolygon polygon)
        {
            try
            {
                var native = GetNativePolygon(polygon);

                if (native != null)
                {
                    native.Remove();
                    _polygons.Remove(native);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        #endregion

        #region Circles

        static protected CircleOptions CreateCircleOptions(XCircle circle)
        {
            var opts = new CircleOptions()
                .InvokeCenter(new LatLng(circle.Center.Latitude, circle.Center.Longitude))
                .InvokeRadius(circle.Radius.Meters)
                .InvokeStrokeWidth(circle.StrokeWidth);

            if (!circle.StrokeColor.IsDefault())
                opts.InvokeStrokeColor(circle.StrokeColor.ToPlatform());

            if (!circle.FillColor.IsDefault())
                opts.InvokeFillColor(circle.FillColor.ToPlatform());

            return opts;
        }

        static protected ACircle GetNativeCircle(XCircle circle)
        {
            ACircle? targetCircle = null;

            if (_circles != null)
            {
                for (int i = 0; i < _circles.Count; i++)
                {
                    var native = _circles[i];
                    if (native.Id == (string)circle.MapElementId)
                    {
                        targetCircle = native;
                        break;
                    }
                }
            }

            return targetCircle;
        }

        static protected XCircle GetMAUICircle(ACircle circle)
        {
            XCircle? targetCircle = null;

            for (int i = 0; i < _map.MapElements.Count; i++)
            {
                var mapElement = _map.MapElements[i];
                if ((string)mapElement.MapElementId == circle.Id)
                {
                    targetCircle = mapElement as XCircle;
                    break;
                }
            }

            return targetCircle;
        }

        static void CircleOnPropertyChanged(XCircle mauiCircle, PropertyChangedEventArgs e)
        {
            try
            {
                var nativeCircle = GetNativeCircle(mauiCircle);

                if (nativeCircle == null)
                {
                    return;
                }

                if (e.PropertyName == XCircle.FillColorProperty.PropertyName)
                {
                    nativeCircle.FillColor = mauiCircle.FillColor.ToPlatform();
                }
                else if (e.PropertyName == XCircle.CenterProperty.PropertyName)
                {
                    nativeCircle.Center = new LatLng(mauiCircle.Center.Latitude, mauiCircle.Center.Longitude);
                }
                else if (e.PropertyName == XCircle.RadiusProperty.PropertyName)
                {
                    nativeCircle.Radius = mauiCircle.Radius.Meters;
                }
                else if (e.PropertyName == MapElement.StrokeColorProperty.PropertyName)
                {
                    nativeCircle.StrokeColor = mauiCircle.StrokeColor.ToPlatform();
                }
                else if (e.PropertyName == MapElement.StrokeWidthProperty.PropertyName)
                {
                    nativeCircle.StrokeWidth = mauiCircle.StrokeWidth;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void AddCircle(XCircle circle)
        {
            try
            {
                var map = nativeMap;
                if (map == null)
                {
                    return;
                }

                if (_circles == null)
                {
                    _circles = new List<ACircle>();
                }

                var options = CreateCircleOptions(circle);
                var nativeCircle = map.AddCircle(options);

                circle.MapElementId = nativeCircle.Id;

                _circles.Add(nativeCircle);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void RemoveCircle(XCircle circle)
        {
            try
            {
                var native = GetNativeCircle(circle);

                if (native != null)
                {
                    native.Remove();
                    _circles.Remove(native);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        #endregion

        static void SetUserVisible()
        {
            try
            {
                GoogleMap? map = nativeMap;
                if (map == null)
                {
                    return;
                }

                if (_map.IsShowingUser)
                {
                    var coarseLocationPermission = ContextCompat.CheckSelfPermission(Platform.AppContext, Manifest.Permission.AccessCoarseLocation);
                    var fineLocationPermission = ContextCompat.CheckSelfPermission(Platform.AppContext, Manifest.Permission.AccessFineLocation);

                    if (coarseLocationPermission == Permission.Granted || fineLocationPermission == Permission.Granted)
                    {
                        map.MyLocationEnabled = true;
                        //map.UiSettings.MyLocationButtonEnabled = _map.ShowGPSButton;
                    }
                    else
                    {
                        Console.WriteLine("MAUI.MapRenderer", "Missing location permissions for IsShowingUser");
                        map.MyLocationEnabled = map.UiSettings.MyLocationButtonEnabled = false;
                    }
                }
                else
                {
                    map.MyLocationEnabled = map.UiSettings.MyLocationButtonEnabled = false;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}
