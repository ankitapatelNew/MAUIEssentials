using System.ComponentModel;
using AsyncAwaitBestPractices;
using MAUIEssentials.AppCode.Helpers;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MAUIEssentials.AppCode.Controls
{
    public class CustomMap : Map
    {
        readonly WeakEventManager<CenterPositionEventArgs> draggingStartedEventManager
            = new WeakEventManager<CenterPositionEventArgs>();

        public event EventHandler<CenterPositionEventArgs> DraggingStarted
        {
            add => draggingStartedEventManager.AddEventHandler(value);
            remove => draggingStartedEventManager.RemoveEventHandler(value);
        }

        readonly WeakEventManager<CenterPositionEventArgs> draggingStoppedEventManager
            = new WeakEventManager<CenterPositionEventArgs>();

        public event EventHandler<CenterPositionEventArgs> DraggingStopped
        {
            add => draggingStoppedEventManager.AddEventHandler(value);
            remove => draggingStoppedEventManager.RemoveEventHandler(value);
        }

        readonly WeakEventManager<CustomPinEventArgs> updateMarkerEventManager
            = new WeakEventManager<CustomPinEventArgs>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<CustomPinEventArgs> UpdateMarker
        {
            add => updateMarkerEventManager.AddEventHandler(value);
            remove => updateMarkerEventManager.RemoveEventHandler(value);
        }

        readonly WeakEventManager<CustomPinEventArgs> showInfoEventManager
            = new WeakEventManager<CustomPinEventArgs>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<CustomPinEventArgs> ShowInfoWindowAction
        {
            add => showInfoEventManager.AddEventHandler(value);
            remove => showInfoEventManager.RemoveEventHandler(value);
        }

        readonly AsyncAwaitBestPractices.WeakEventManager hideInfoEventManager = new AsyncAwaitBestPractices.WeakEventManager();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler HideInfoWindowAction
        {
            add => hideInfoEventManager.AddEventHandler(value);
            remove => hideInfoEventManager.RemoveEventHandler(value);
        }

        public static readonly BindableProperty CustomPinsProperty =
            BindableProperty.Create(nameof(CustomPins), typeof(List<CustomPin>), typeof(CustomMap), new List<CustomPin>());

        public static readonly BindableProperty PinImageProperty =
            BindableProperty.Create(nameof(PinImage), typeof(string), typeof(CustomMap), string.Empty);

        public static readonly BindableProperty SelectedPinImageProperty =
            BindableProperty.Create(nameof(SelectedPinImage), typeof(string), typeof(CustomMap), string.Empty);

        public static readonly BindableProperty PinSizeProperty =
            BindableProperty.Create(nameof(PinSize), typeof(Size), typeof(CustomMap), Size.Zero);

        public static readonly BindableProperty SelectedPinSizeProperty =
            BindableProperty.Create(nameof(SelectedPinSize), typeof(Size), typeof(CustomMap), Size.Zero);

        public static readonly BindableProperty IsSelectionAllowedProperty =
            BindableProperty.Create(nameof(IsSelectionAllowed), typeof(bool), typeof(CustomMap), true);

        public static readonly BindableProperty ShowGPSButtonProperty =
            BindableProperty.Create(nameof(ShowGPSButton), typeof(bool), typeof(CustomMap), true);

        public List<CustomPin> CustomPins
        {
            get => (List<CustomPin>)GetValue(CustomPinsProperty);
            set => SetValue(CustomPinsProperty, value);
        }

        public string PinImage
        {
            get => (string)GetValue(PinImageProperty);
            set => SetValue(PinImageProperty, value);
        }

        public string SelectedPinImage
        {
            get => (string)GetValue(SelectedPinImageProperty);
            set => SetValue(SelectedPinImageProperty, value);
        }

        public Size PinSize
        {
            get => (Size)GetValue(PinSizeProperty);
            set => SetValue(PinSizeProperty, value);
        }

        public Size SelectedPinSize
        {
            get => (Size)GetValue(SelectedPinSizeProperty);
            set => SetValue(SelectedPinSizeProperty, value);
        }

        public bool IsSelectionAllowed
        {
            get => (bool)GetValue(IsSelectionAllowedProperty);
            set => SetValue(IsSelectionAllowedProperty, value);
        }

        public bool ShowGPSButton
        {
            get => (bool)GetValue(ShowGPSButtonProperty);
            set => SetValue(ShowGPSButtonProperty, value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnDraggingStarted(Location location)
        {
            draggingStartedEventManager?.RaiseEvent(this, new CenterPositionEventArgs { Location = location }, nameof(DraggingStarted));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnDraggingStopped(Location location)
        {
            draggingStoppedEventManager?.RaiseEvent(this, new CenterPositionEventArgs { Location = location }, nameof(DraggingStopped));
        }

        public CustomMap(bool setRegion = true)
        {
            hideInfoEventManager = new AsyncAwaitBestPractices.WeakEventManager();
            if (setRegion)
            {
                Task.Run(async () =>
                {
                    await SetRegion();
                });
            }
        }

        private async Task SetRegion()
        {
            await Task.Delay(200);

            var mapSpan = MapSpan.FromCenterAndRadius(new Location(CommonUtils.DefaultLat, CommonUtils.DefaultLon), Distance.FromKilometers(3));
            //TODO:MoveToRegion Should replace SetVisibleRegion => must test after running
            //SetVisibleRegion(mapSpan);
            MoveToRegion(mapSpan);
        }

        public Func<Location>? NativeGetMapCenterLocation { get; set; }
        public Location GetMapCenterLocation()
        {
            if (NativeGetMapCenterLocation != null)
            {
                return NativeGetMapCenterLocation();
            }
            else
            {
                return new Location(0, 0);
            }
        }

        public void ShowInfoWindow(CustomPin pin)
        {
            showInfoEventManager?.RaiseEvent(this, new CustomPinEventArgs
            {
                Pin = pin
            }, nameof(ShowInfoWindowAction));
        }

        public void HideInfoWindow()
        {
            try
            {
                if(hideInfoEventManager != null){
                    hideInfoEventManager.RaiseEvent(this, EventArgs.Empty, nameof(HideInfoWindowAction));
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public void OnUpdateMarker(CustomPin pin)
        {
            try
            {
                updateMarkerEventManager?.RaiseEvent(this, new CustomPinEventArgs
                {
                    Pin = pin
                }, nameof(UpdateMarker));
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public virtual void AddPin(CustomPin pin)
        {
            Pins.Add(pin);
        }

        public virtual void ClearPins()
        {
            Pins.Clear();
        }
    }

    public class MapTapEventArgs : EventArgs
    {
        public string? Id { get; set; }
        public Location? Location { get; set; }
        public string? PinTitle { get; set; }
        public string? PinDescription { get; set; }
    }

    public class CenterPositionEventArgs : EventArgs
    {
        public Location? Location { get; set; }
    }

    public class CustomPinEventArgs : EventArgs
    {
        public CustomPin? Pin { get; set; }
    }

    public class CustomPin : Pin
    {
        readonly AsyncAwaitBestPractices.WeakEventManager annotationEventManager = new AsyncAwaitBestPractices.WeakEventManager();

        public event EventHandler AnnotationTapped
        {
            add => annotationEventManager.AddEventHandler(value);
            remove => annotationEventManager.RemoveEventHandler(value);
        }   

        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(nameof(PinImageImageSource), typeof(ImageSource), typeof(CustomPin));

        public ImageSource? PinImageImageSource
        {
            get => (ImageSource?)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly BindableProperty SelectedImageSourceProperty = BindableProperty.Create(nameof(SelectedPinImageImageSource), typeof(ImageSource), typeof(CustomPin));

        public ImageSource? SelectedPinImageImageSource
        {
            get => (ImageSource?)GetValue(SelectedImageSourceProperty);
            set => SetValue(SelectedImageSourceProperty, value);
        }

        public int Index { get; set; }

        public string? Url { get; set; }
        public string? PinTitle { get; set; }
        public string? PinDescription { get; set; }

        private string? pinImage;
        public string? PinImage
        {
            get
            {
                return pinImage;
            }
            set
            {
                pinImage = value;
                PinImageImageSource = value;
            }
        }

        private string? selectedPinImage;
        public string? SelectedPinImage
        {
            get
            {
                return selectedPinImage;
            }
            set
            {
                selectedPinImage = value;
                SelectedPinImageImageSource = value;
            }
        }

        public bool CanShowInfo { get; set; }

        public Size PinSize { get; set; } = Size.Zero;
        public Size SelectedPinSize { get; set; } = Size.Zero;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnAnnotationTapped()
        {
            annotationEventManager?.RaiseEvent(this, EventArgs.Empty, nameof(AnnotationTapped));
        }
    }
}