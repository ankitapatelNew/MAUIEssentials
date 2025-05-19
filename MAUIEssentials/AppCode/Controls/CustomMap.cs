namespace MAUIEssentials.AppCode.Controls
{
    public class CustomMap : Map
    {
        public event EventHandler<CenterPositionEventArgs> DraggingStarted;
        public event EventHandler<CenterPositionEventArgs> DraggingStopped;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<CustomPinEventArgs> UpdateMarker;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<CustomPinEventArgs> ShowInfoWindowAction;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler HideInfoWindowAction;

        public static readonly BindableProperty CustomPinsProperty =
            BindableProperty.Create(nameof(CustomPins), typeof(List<CustomPin>), typeof(CustomMap), new List<CustomPin>());

        public static readonly BindableProperty PinImageProperty =
            BindableProperty.Create(nameof(PinImage), typeof(string), typeof(CustomMap), string.Empty);

        public static readonly BindableProperty SelectedPinImageProperty =
            BindableProperty.Create(nameof(SelectedPinImage), typeof(string), typeof(CustomMap), string.Empty);

        public static readonly BindableProperty IsSelectionAllowedProperty =
            BindableProperty.Create(nameof(IsSelectionAllowed), typeof(bool), typeof(CustomMap), true);

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

        public bool IsSelectionAllowed
        {
            get => (bool)GetValue(IsSelectionAllowedProperty);
            set => SetValue(IsSelectionAllowedProperty, value);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnDraggingStarted(Location location)
        {
            DraggingStarted?.Invoke(this, new CenterPositionEventArgs { Position = location });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnDraggingStopped(Location location)
        {
            DraggingStopped?.Invoke(this, new CenterPositionEventArgs { Position = location });
        }

        public Func<Location> NativeGetMapCenterLocation { get; set; }
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
            ShowInfoWindowAction?.Invoke(this, new CustomPinEventArgs
            {
                Pin = pin
            });
        }

        public void HideInfoWindow()
        {
            HideInfoWindowAction?.Invoke(this, EventArgs.Empty);
        }

        public void OnUpdateMarker(CustomPin pin)
        {
            UpdateMarker?.Invoke(this, new CustomPinEventArgs
            {
                Pin = pin
            });
        }
    }

    public class MapTapEventArgs : EventArgs
    {
        public string Id { get; set; }
        public Location Position { get; set; }
        public string PinTitle { get; set; }
        public string PinDescription { get; set; }
    }

    public class CenterPositionEventArgs : EventArgs
    {
        public Location Position { get; set; }
    }

    public class CustomPinEventArgs : EventArgs
    {
        public CustomPin Pin { get; set; }
    }

    public class CustomPin : Pin
    {
        public event EventHandler AnnotationTapped;

        public int Index { get; set; }

        public string Url { get; set; }
        public string PinTitle { get; set; }
        public string PinDescription { get; set; }

        public string PinImage { get; set; }
        public string SelectedPinImage { get; set; }
        public bool CanShowInfo { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnAnnotationTapped()
        {
            AnnotationTapped?.Invoke(this, EventArgs.Empty);
        }
    }
}