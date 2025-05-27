using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CoreGraphics;
using CoreLocation;
using Foundation;
using MapKit;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Maps.Platform;
using ObjCRuntime;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using UIKit;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MAUIEssentials
{
    public partial class CommanMapHandler : MapHandler
    {
        protected override MauiMKMapView CreatePlatformView()
        {
            MauiMKMapView mapView = MapPool.Get();
            if (mapView == null)
            {
                mapView = new MauiMKMapView(this);
            }
            return mapView;
        }

        static IMapHandler? _handler;
        static CustomMap? _map;

        static UIView? customPinView;
        static MKAnnotationView? annotationView;
        static UITapGestureRecognizer? _mapClickedGestureRecognizer;
        static CLLocationManager? _locationManager;

        static bool nextRegionChangeIsFromUserInteraction;
        static bool _shouldUpdateRegion;
        static object? _lastTouchedView;
        static bool isMarkerUpdating;

        static CancellationTokenSource? cts;

        const string MoveMessageName = "MapMoveToRegion";

        //protected override bool ManageNativeControlLifetime => !UIDevice.CurrentDevice.CheckSystemVersion(9, 0);

        public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            if (_handler != null)
            {
                return _handler.PlatformView.GetSizeRequest(widthConstraint, heightConstraint);
            }
            else
            {
                return base.GetDesiredSize(widthConstraint, heightConstraint);
            }
        }

        protected override void ConnectHandler(MauiMKMapView platformView)
        {
            try
            {
                base.ConnectHandler(platformView);

                platformView.Frame = new CGRect(
                    platformView.Frame.X,
                    platformView.Frame.Y,
                    platformView.Superview?.Frame.Width ?? UIScreen.MainScreen.Bounds.Width,
                    (platformView.Superview?.Frame.Width ?? UIScreen.MainScreen.Bounds.Width) / 1.5
                );
                platformView.LayoutSubviews();

                Console.WriteLine("MKMapView connected to handler.");

                _map = VirtualView as CustomMap;
                if (_map == null)
                {
                    Console.WriteLine("CommanMapHandler: _map is null in ConnectHandler");
                }
                _handler = this;
                //var MauiMap = e.NewElement as CustomMap;

                platformView.GetViewForAnnotation = GetViewForAnnotation;
                platformView.OverlayRenderer = GetViewForOverlay;

                platformView.CalloutAccessoryControlTapped += CalloutAccessoryControlTapped;
                platformView.DidSelectAnnotationView += OnDidSelectAnnotationView;
                platformView.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
                platformView.RegionChanged += MkMapViewOnRegionChanged;
                platformView.RegionWillChange += NativeMap_RegionWillChange;
                platformView.AddGestureRecognizer(_mapClickedGestureRecognizer = new UITapGestureRecognizer(OnMapClicked));
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

        protected override void DisconnectHandler(MauiMKMapView platformView)
        {
            try
            {
                if (_map != null)
                {
                    var mapModel = _map;
                    MessagingCenter.Unsubscribe<Map, MapSpan>(this, MoveMessageName);

                    ((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged -= OnPinCollectionChanged;
                    ((ObservableCollection<MapElement>)mapModel.MapElements).CollectionChanged -= OnMapElementCollectionChanged;

                    foreach (Pin pin in mapModel.Pins)
                    {
                        pin.PropertyChanged -= PinOnPropertyChanged;
                    }

                    mapModel.NativeGetMapCenterLocation = null;
                    mapModel.ShowInfoWindowAction -= MauiMap_ShowInfoWindowAction;
                    mapModel.HideInfoWindowAction -= MauiMap_HideInfoWindowAction;
                    mapModel.UpdateMarker -= UpdateMarker;
                }

                var mkMapView = (MauiMKMapView)platformView;
                mkMapView.CalloutAccessoryControlTapped -= CalloutAccessoryControlTapped;
                mkMapView.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                mkMapView.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
                mkMapView.RegionChanged -= MkMapViewOnRegionChanged;
                mkMapView.RegionWillChange -= NativeMap_RegionWillChange;

                mkMapView.GetViewForAnnotation = null;
                mkMapView.OverlayRenderer = null;

                if (mkMapView.Delegate != null)
                {
                    mkMapView.Delegate.Dispose();
                    mkMapView.Delegate = null;
                }

                mkMapView.RemoveFromSuperview();
                mkMapView.RemoveGestureRecognizer(_mapClickedGestureRecognizer);
                _mapClickedGestureRecognizer.Dispose();
                _mapClickedGestureRecognizer = null;

                if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
                {
                    // This renderer is done with the MKMapView; we can put it in the pool
                    // for other rendererers to use in the future
                    MapPool.Add(mkMapView);
                }

                // For iOS versions < 9, the MKMapView will be disposed in ViewRenderer's Dispose method
                if (_locationManager != null)
                {
                    _locationManager.Dispose();
                    _locationManager = null;
                }

                _lastTouchedView = null;
                customPinView?.Dispose();
                annotationView?.Dispose();

                customPinView = null;
                annotationView = null;

                cts?.Dispose();
                cts = null;

                base.DisconnectHandler(platformView);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected static IMKAnnotation CreateAnnotation(Pin pin)
        {
            return new MKPointAnnotation
            {
                Title = pin.Label,
                Subtitle = pin.Address ?? "",   
                Coordinate = new CLLocationCoordinate2D(pin.Location.Latitude, pin.Location.Longitude)
            };
        }

        void OnMapClicked(UITapGestureRecognizer recognizer)
        {
            try
            {
                if (_map == null)
                {
                    return;
                }

                var tapPoint = recognizer.LocationInView(_handler?.PlatformView);
                var tapGPS = _handler?.PlatformView.ConvertPoint(tapPoint, _handler.PlatformView);
                //((Map)_map).SendMapClicked(new Location(tapGPS.Latitude, tapGPS.Longitude));
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static private void UpdateMarker(object sender, CustomPinEventArgs e)
        {
            try
            {
                isMarkerUpdating = true;
                MauiMap_HideInfoWindowAction(null, null);

                if (_handler?.PlatformView.Annotations.Length > 0)
                {
                    var annotation = _handler.PlatformView.Annotations.FirstOrDefault(x => x.Coordinate.Latitude == e.Pin.Location.Latitude
                        && x.Coordinate.Longitude == e.Pin.Location.Longitude && x.GetTitle() == e.Pin.Label);

                    GetViewForAnnotation(_handler.PlatformView, annotation);
                    _handler.PlatformView.SelectAnnotation(annotation, false);
                }
                else
                {
                    isMarkerUpdating = false;
                }
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
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current?.Dispatcher?.StartTimer(TimeSpan.FromSeconds(0.3), () => {
                        annotationView?.SetSelected(true, true);
                        return false;
                    });
                });
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void MauiMap_HideInfoWindowAction(object sender, EventArgs args)
        {
            try
            {
                foreach (var item in _handler.PlatformView.SelectedAnnotations)
                {
                    _handler.PlatformView.DeselectAnnotation(item, false);
                }
                annotationView = null;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void NativeMap_RegionWillChange(object sender, MKMapViewChangeEventArgs e)
        {
            try
            {
                var tempView = (sender as MauiMKMapView).Subviews[0] as UIView;
                var listOfGestures = tempView.GestureRecognizers as UIGestureRecognizer[];

                foreach (var recognizer in listOfGestures)
                {
                    if (recognizer.State == UIGestureRecognizerState.Began ||
                        recognizer.State == UIGestureRecognizerState.Ended)
                    {
                        nextRegionChangeIsFromUserInteraction = true;
                        _map.OnDraggingStarted(GetMapCenterLocation());
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        void MkMapViewOnRegionChanged(object sender, MKMapViewChangeEventArgs e)
        {
            try
            {
                if (_map == null || _handler == null || _handler.PlatformView == null)
                {
                    Console.WriteLine("MkMapViewOnRegionChanged: _map or _handler is null");
                    return;
                }

                var mapModel = (Map)_map;
                var mkMapView = _handler.PlatformView;

                mapModel.MoveToRegion(new MapSpan(
                    new Location(mkMapView.Region.Center.Latitude, mkMapView.Region.Center.Longitude),
                    mkMapView.Region.Span.LatitudeDelta,
                    mkMapView.Region.Span.LongitudeDelta
                ));

                if (nextRegionChangeIsFromUserInteraction)
                {
                    nextRegionChangeIsFromUserInteraction = false;
                    _map.OnDraggingStopped(GetMapCenterLocation());
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static internal Location GetMapCenterLocation()
        {
            var centerPosition = _handler.PlatformView.CenterCoordinate;
            return new Location(centerPosition.Latitude, centerPosition.Longitude);
        }

        static protected void AttachGestureToPin(MKAnnotationView mapPin, IMKAnnotation annotation)
        {
            try
            {
                var recognizers = mapPin.GestureRecognizers;

                if (recognizers != null)
                {
                    foreach (var r in recognizers)
                    {
                        mapPin.RemoveGestureRecognizer(r);
                    }
                }

                var recognizer = new UITapGestureRecognizer(g => OnCalloutClicked(annotation))
                {
                    ShouldReceiveTouch = (gestureRecognizer, touch) => {
                        _lastTouchedView = touch.View;
                        return true;
                    }
                };
                mapPin.AddGestureRecognizer(recognizer);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static protected Pin GetPinForAnnotation(IMKAnnotation annotation)
        {
            if (_map == null)
            {
                Console.WriteLine("GetPinForAnnotation: _map is null");
                return null;
            }

            Pin? targetPin = null;
            var map = (Map)_map;

            for (int i = 0; i < map.Pins.Count; i++)
            {
                var pin = map.Pins[i];
                if (pin.MarkerId != null && (IMKAnnotation)pin.MarkerId == annotation)
                {
                    targetPin = pin;
                    break;
                }
            }

            return targetPin;
        }

        protected static MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            if (Runtime.GetNSObject(annotation.Handle) is MKUserLocation)
            {
                return null;
            }

            Console.WriteLine($"GetViewForAnnotation called for annotation: {annotation.GetTitle()}");

            var customPin = (CustomPin)GetPinForAnnotation(annotation as MKPointAnnotation);
            if (customPin == null)
            {
                return null;
            }

            annotationView = mapView.DequeueReusableAnnotation(customPin.MarkerId.ToString()) ??
                new MKAnnotationView(annotation, customPin.MarkerId.ToString());
            if (annotationView != null)
            {
                annotationView = new CustomMKAnnotationView(annotation, customPin.MarkerId.ToString());
                ((CustomMKAnnotationView)annotationView).Id = customPin.MarkerId.ToString();
                ((CustomMKAnnotationView)annotationView).Url = customPin.Url;
                ((CustomMKAnnotationView)annotationView).CanShowInfo = customPin.CanShowInfo;
                ((CustomMKAnnotationView)annotationView).Position = customPin.Location;
                ((CustomMKAnnotationView)annotationView).PinTitle = customPin.PinTitle;
                ((CustomMKAnnotationView)annotationView).PinDescription = customPin.PinDescription;
                ((CustomMKAnnotationView)annotationView).Image = UIImage.FromFile("ic_pin_gray");

                annotationView.CanShowCallout = customPin.CanShowInfo;
                annotationView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);

                Console.WriteLine($"Custom pin created for: {customPin.PinTitle}");
                _ = SetAnnotationPin(annotationView, customPin);
            }

            annotationView.Annotation = annotation;
            AttachGestureToPin(annotationView, annotation);

            return annotationView;
        }

        static void OnCalloutClicked(IMKAnnotation annotation)
        {
            try
            {
                // lookup pin
                var targetPin = GetPinForAnnotation(annotation);

                // pin not found. Must have been activated outside of MAUI
                if (targetPin == null)
                    return;

                // if the tap happened on the annotation view itself, skip because this is what happens when the callout is showing
                // when the callout is already visible the tap comes in on a different view
                if (_lastTouchedView is MKAnnotationView)
                    return;

#pragma warning disable CS0618
                //targetPin.SendTap();
#pragma warning restore CS0618

                bool deselect = targetPin.SendInfoWindowClick();
                if (deselect)
                {
                    _handler?.PlatformView.DeselectAnnotation(annotation, true);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void CalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            try
            {
                if (e.View is CustomMKAnnotationView customView)
                {
                    var customPin = (CustomPin)GetPinForAnnotation(customView?.Annotation as MKPointAnnotation);

                    if (customPin != null)
                    {
                        customPin.OnAnnotationTapped();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static async void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            try
            {
                var annotation = e.View?.Annotation;
                var customMap = _map;
                var customPin = (CustomPin)GetPinForAnnotation(annotation as MKPointAnnotation);

                if (customMap.IsSelectionAllowed)
                {
                    var normalPinTask = SetAnnotationPin(e.View, customPin);

                    annotationView = e.View as CustomMKAnnotationView;
                    var selectedPinTask = SetAnnotationPin(e.View, customPin, true);

                    await normalPinTask;
                    await selectedPinTask;

                    if (customPin != null && !isMarkerUpdating)
                    {
                        bool deselect = customPin.SendMarkerClick();

                        if (deselect)
                        {
                            _handler?.PlatformView.DeselectAnnotation(annotation, false);
                        }
                    }
                    else
                    {
                        isMarkerUpdating = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static async void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            try
            {
                var annotation = e.View?.Annotation;
                var customPin = (CustomPin)GetPinForAnnotation(annotation as MKPointAnnotation);

                await SetAnnotationPin(e.View, customPin);

                if (e.View != null && !e.View.Selected && customPinView != null)
                {
                    customPinView.RemoveFromSuperview();
                    customPinView.Dispose();
                    customPinView = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static private async Task SetAnnotationPin(MKAnnotationView annotationView, CustomPin customPin, bool isSelected = false)
        {
            try
            {
                var customMap = _map;

                if (annotationView == null || customPin == null)
                {
                    return;
                }

                if (cts == null)
                {
                    cts = new CancellationTokenSource();
                }

                if (isSelected)
                {
                    if (customPin != null && !string.IsNullOrEmpty(customPin.SelectedPinImage))
                    {
                        await SetPinIcon(annotationView, customPin.SelectedPinImage, new Size(40, 60));
                    }
                    else if (customMap != null && !string.IsNullOrEmpty(customMap.SelectedPinImage))
                    {
                        await SetPinIcon(annotationView, customMap.SelectedPinImage, new Size(40, 60));
                    }
                }
                else
                {
                    if (customPin != null && !string.IsNullOrEmpty(customPin.PinImage))
                    {
                        await SetPinIcon(annotationView, customPin.PinImage, new Size(45, 55));
                    }
                    else if (customMap != null && !string.IsNullOrEmpty(customMap.PinImage))
                    {
                        await SetPinIcon(annotationView, customMap.PinImage, new Size(45, 55));
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static private async Task SetPinIcon(MKAnnotationView annotationView, string pinImage, Size imageSize)
        {
            try
            {
                UIImage? image = null;
                annotationView.Image = null;

                if (pinImage.StartsWith("http"))
                {
                    image = FromUrl(pinImage);
                }
                else
                {
                    image = UIImage.FromFile(pinImage);
                }

                if (image == null)
                {
                    return;
                }

                if (imageSize != Size.Zero)
                {
                    annotationView.Image = image.ScaleImage(imageSize.ToSizeF());
                }
                else
                {
                    annotationView.Image = image;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static UIImage? FromUrl(string uri)
        {
            using (var url = new NSUrl(uri))
            using (var data = NSData.FromUrl(url))
            {
                return UIImage.LoadFromData(data);
            }
        }

        static void UpdateRegion()
        {
            try
            {
                if (_shouldUpdateRegion)
                {
                    MoveToRegion(((Map)_map).VisibleRegion, false);
                    _shouldUpdateRegion = false;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void MoveToRegion(MapSpan mapSpan, bool animated = true)
        {
            try
            {
                if (mapSpan == null)
                {
                    return;
                }

                MainThread.BeginInvokeOnMainThread(() => {
                    try
                    {
                        Location center = mapSpan.Center;
                        var coordinate2d = new CLLocationCoordinate2D(center.Latitude, center.Longitude);
                        var coordinateSpan = new MKCoordinateSpan(mapSpan.LatitudeDegrees, mapSpan.LongitudeDegrees);

                        _handler?.PlatformView.SetRegion(new MKCoordinateRegion(coordinate2d, coordinateSpan), animated);
                    }
                    catch (Exception ex)
                    {
                        ex.LogException();
                    }
                });
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void AddPins(IList pins)
        {
            try
            {
                foreach (Pin pin in pins)
                {
                    pin.PropertyChanged += PinOnPropertyChanged;

                    var annotation = CreateAnnotation(pin);
                    pin.MarkerId = annotation;

                    _handler?.PlatformView.AddAnnotation(annotation);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void PinOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                Pin pin = (Pin)sender;

                if (pin.MarkerId is MKPointAnnotation annotation)
                {
                    switch (e.PropertyName)
                    {
                        case nameof(Pin.Label):
                            annotation.Title = pin.Label;
                            break;

                        case nameof(Pin.Address):
                            annotation.Subtitle = pin.Address;
                            break;

                        case nameof(Pin.Location):
                            annotation.Coordinate = new CLLocationCoordinate2D(pin.Location.Latitude, pin.Location.Longitude);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static void OnPinCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                Console.WriteLine($"Pin Collection Changed: {e.Action}");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    PinCollectionChanged(e);
                    _handler?.PlatformView.SetNeedsDisplay();
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
                        var mapView = _handler?.PlatformView;
                        if (mapView?.Annotations?.Length > 0)
                            mapView.RemoveAnnotations(mapView.Annotations);
                        AddPins((IList)(_map as Map).Pins);
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
                foreach (Pin pin in pins)
                {
                    pin.PropertyChanged -= PinOnPropertyChanged;
                    _handler?.PlatformView.RemoveAnnotation((IMKAnnotation)pin.MarkerId);
                }
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
                handler.PlatformView.ScrollEnabled = ((Map)map).IsScrollEnabled;
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
                handler.PlatformView.ShowsTraffic = map.IsTrafficEnabled;
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
                handler.PlatformView.ZoomEnabled = map.IsZoomEnabled;
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
                if (map.IsShowingUser)
                {
                    _locationManager = new CLLocationManager();
                    _locationManager.RequestWhenInUseAuthorization();
                }
                handler.PlatformView.ShowsUserLocation = map.IsShowingUser;
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

                map.NativeGetMapCenterLocation = new Func<Location>(GetMapCenterLocation);
                map.ShowInfoWindowAction += MauiMap_ShowInfoWindowAction;
                map.HideInfoWindowAction += MauiMap_HideInfoWindowAction;
                map.UpdateMarker += UpdateMarker;

                MessagingCenter.Subscribe<Map, MapSpan>(nameof(CommanMapHandler), MoveMessageName, (s, a) => MoveToRegion(a), map);

                if (map.VisibleRegion != null)
                    MoveToRegion(map.VisibleRegion, false);

                ((ObservableCollection<Pin>)map.Pins).CollectionChanged += OnPinCollectionChanged;
                OnPinCollectionChanged(map.Pins, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                ((ObservableCollection<MapElement>)map.MapElements).CollectionChanged += OnMapElementCollectionChanged;
                OnMapElementCollectionChanged(map.MapElements, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                switch (map.MapType)
                {
                    case MapType.Street:
                        handler.PlatformView.MapType = MKMapType.Standard;
                        break;
                    case MapType.Satellite:
                        handler.PlatformView.MapType = MKMapType.Satellite;
                        break;
                    case MapType.Hybrid:
                        handler.PlatformView.MapType = MKMapType.Hybrid;
                        break;
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
                if (Device.IsInvokeRequired)
                {
                    MainThread.BeginInvokeOnMainThread(() => MapElementCollectionChanged(e));
                }
                else
                {
                    MapElementCollectionChanged(e);
                }
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
                        var mkMapView = _handler.PlatformView;

                        if (mkMapView.Overlays != null)
                        {
                            var overlays = mkMapView.Overlays;
                            foreach (var overlay in overlays)
                            {
                                mkMapView.RemoveOverlay(overlay);
                            }
                        }

                        AddMapElements(((Map)_map).MapElements);
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

                    IMKOverlay? overlay = null;
                    switch (element)
                    {
                        case Polyline polyline:
                            overlay = MKPolyline.FromCoordinates(polyline.Geopath
                                .Select(position => new CLLocationCoordinate2D(position.Latitude, position.Longitude))
                                .ToArray());
                            break;
                        case Polygon polygon:
                            overlay = MKPolygon.FromCoordinates(polygon.Geopath
                                .Select(position => new CLLocationCoordinate2D(position.Latitude, position.Longitude))
                                .ToArray());
                            break;
                        case Circle circle:
                            overlay = MKCircle.Circle(
                                new CLLocationCoordinate2D(circle.Center.Latitude, circle.Center.Longitude),
                                circle.Radius.Meters);
                            break;
                    }

                    element.MapElementId = overlay;

                    _handler?.PlatformView.AddOverlay(overlay);
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

                    var overlay = (IMKOverlay?)element.MapElementId;
                    _handler?.PlatformView.RemoveOverlay(overlay);
                }
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
                var element = (MapElement)sender;

                RemoveMapElements(new[] { element });
                AddMapElements(new[] { element });
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected virtual MKOverlayRenderer GetViewForOverlay(MKMapView mapview, IMKOverlay overlay)
        {
            switch (overlay)
            {
                case MKPolyline polyline:
                    return GetViewForPolyline(polyline);
                case MKPolygon polygon:
                    return GetViewForPolygon(polygon);
                case MKCircle circle:
                    return GetViewForCircle(circle);
                default:
                    return null;
            }
        }

        protected virtual MKPolylineRenderer GetViewForPolyline(MKPolyline mkPolyline)
        {
            var map = (Map?)_map;
            Polyline? targetPolyline = null;

            for (int i = 0; i < map.MapElements.Count; i++)
            {
                var element = map.MapElements[i];
                if (ReferenceEquals(element.MapElementId, mkPolyline))
                {
                    targetPolyline = (Polyline)element;
                    break;
                }
            }

            if (targetPolyline == null)
            {
                return null;
            }

            return new MKPolylineRenderer(mkPolyline)
            {
                StrokeColor = targetPolyline.StrokeColor.ToUIColor(Colors.Black),
                LineWidth = targetPolyline.StrokeWidth
            };
        }

        protected virtual MKPolygonRenderer GetViewForPolygon(MKPolygon mkPolygon)
        {
            var map = (Map?)_map;
            Polygon? targetPolygon = null;

            for (int i = 0; i < map.MapElements.Count; i++)
            {
                var element = map.MapElements[i];
                if (ReferenceEquals(element.MapElementId, mkPolygon))
                {
                    targetPolygon = (Polygon)element;
                    break;
                }
            }

            if (targetPolygon == null)
            {
                return null;
            }

            return new MKPolygonRenderer(mkPolygon)
            {
                StrokeColor = targetPolygon.StrokeColor.ToUIColor(Colors.Black),
                FillColor = targetPolygon.FillColor.ToUIColor(),
                LineWidth = targetPolygon.StrokeWidth
            };
        }

        protected virtual MKCircleRenderer GetViewForCircle(MKCircle mkCircle)
        {
            var map = (Map?)_map;
            Circle targetCircle = null;

            for (int i = 0; i < map.MapElements.Count; i++)
            {
                var element = map.MapElements[i];
                if (ReferenceEquals(element.MapElementId, mkCircle))
                {
                    targetCircle = (Circle)element;
                    break;
                }
            }

            if (targetCircle == null)
            {
                return null;
            }

            return new MKCircleRenderer(mkCircle)
            {
                StrokeColor = targetCircle.StrokeColor.ToUIColor(Colors.Black),
                FillColor = targetCircle.FillColor.ToUIColor(),
                LineWidth = targetCircle.StrokeWidth
            };
        }

    }

    public class CustomMKAnnotationView : MKAnnotationView
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public Location Position { get; set; }

        public string PinTitle { get; set; }

        public string PinDescription { get; set; }

        public bool CanShowInfo { get; set; }

        public CustomMKAnnotationView(IMKAnnotation annotation, string id)
            : base(annotation, id)
        {
        }
    }

    internal class MapPool
    {
        static MapPool? s_instance;
        public static MapPool Instance => s_instance ?? (s_instance = new MapPool());

        internal readonly ConcurrentQueue<MauiMKMapView> Maps = new ConcurrentQueue<MauiMKMapView>();

        public static void Add(MauiMKMapView mapView)
        {
            try
            {
                Instance.Maps.Enqueue(mapView);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static MauiMKMapView Get()
        {
            MauiMKMapView mapView;
            return Instance.Maps.TryDequeue(out mapView) ? mapView : null;
        }
    }
}