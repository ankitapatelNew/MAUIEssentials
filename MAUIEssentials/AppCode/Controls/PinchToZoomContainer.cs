using AsyncAwaitBestPractices;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.AppCode.Controls
{
    public class PinchToZoomContainer : ContentView
	{
        private double _startScale, _currentScale, _lastScaleChange;
        private double _startX, _startY;
        private double _xOffset, _yOffset;

        readonly PanGestureRecognizer pan;
        readonly WeakEventManager<ScaleChangeEventArgs> scaleChangeManager = new WeakEventManager<ScaleChangeEventArgs>();

        public event EventHandler<ScaleChangeEventArgs> ScaleChanged
        {
            add => scaleChangeManager.AddEventHandler(value);
            remove => scaleChangeManager.RemoveEventHandler(value);
        }

        public double MinScale { get; set; } = 1;
        public double MaxScale { get; set; } = 4;

        public PinchToZoomContainer()
        {
            var tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            tap.Tapped += OnTapped;
            GestureRecognizers.Add(tap);

            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            GestureRecognizers.Add(pinchGesture);

            pan = new PanGestureRecognizer();
            pan.PanUpdated += OnPanUpdated;
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
            MessagingCenter.Subscribe<string>(nameof(PinchToZoomContainer), "PinchToZoomOutCall", (obj) => {
                RestoreScaleValues();
            });
        }

        protected override void OnChildRemoved(Element child, int oldLogicalIndex)
        {
            base.OnChildRemoved(child, oldLogicalIndex);
            MessagingCenter.Unsubscribe<string>(nameof(PinchToZoomContainer), "PinchToZoomOutCall");
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            RestoreScaleValues();
            Content.AnchorX = 0.5;
            Content.AnchorY = 0.5;

            base.OnSizeAllocated(width, height);
        }

        private void RestoreScaleValues()
        {
            Content.ScaleTo(MinScale, 250, Easing.CubicInOut);
            Content.TranslateTo(0, 0, 250, Easing.CubicInOut);

            _currentScale = MinScale;
            _xOffset = Content.TranslationX = 0;
            _yOffset = Content.TranslationY = 0;
            scaleChangeManager?.RaiseEvent(this, new ScaleChangeEventArgs { Scale = _currentScale }, nameof(ScaleChanged));

            if (GestureRecognizers.Contains(pan))
            {
                GestureRecognizers.Remove(pan);
            }
        }

        private void OnTapped(object sender, EventArgs e)
        {
            if (Content.Scale > MinScale)
            {
                RestoreScaleValues();
            }
            else
            {
                StartScaling();
                ExecuteScaling(MaxScale, .5, .5, true);
                EndGesture();
            }
        }

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    StartScaling();
                    break;

                case GestureStatus.Running:
                    ExecuteScaling(e.Scale, e.ScaleOrigin.X, e.ScaleOrigin.Y);
                    break;

                case GestureStatus.Completed:
                    EndGesture();
                    break;
            }
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _startX = e.TotalX;
                    _startY = e.TotalY;

                    Content.AnchorX = 0;
                    Content.AnchorY = 0;
                    break;

                case GestureStatus.Running:
                    var maxTranslationX = Content.Scale * Content.Width - Content.Width;
                    Content.TranslationX = Math.Min(0, Math.Max(-maxTranslationX, _xOffset + e.TotalX - _startX));

                    var maxTranslationY = Content.Scale * Content.Height - Content.Height;
                    Content.TranslationY = Math.Min(0, Math.Max(-maxTranslationY, _yOffset + e.TotalY - _startY));
                    break;

                case GestureStatus.Completed:
                    EndGesture();
                    break;
            }
        }

        private void StartScaling()
        {
            _startScale = Content.Scale;

            Content.AnchorX = 0;
            Content.AnchorY = 0;
        }

        private void ExecuteScaling(double scale, double x, double y, bool isDoubleTap = false)
        {
            try
            {
                var scaleChange = (scale - 1) * _startScale;
                if ((_lastScaleChange < 0 && scaleChange > 0) || (_lastScaleChange > 0 && scaleChange < 0))
                    scaleChange = 0;

                _lastScaleChange = scaleChange;

                _currentScale += scaleChange;
                _currentScale = Math.Max(MinScale, _currentScale);
                _currentScale = Math.Min(MaxScale, _currentScale);

                var deltaX = (Content.X + _xOffset) / Width;
                var deltaWidth = Width / (Content.Width * _startScale);
                var originX = (x - deltaX) * deltaWidth;

                var deltaY = (Content.Y + _yOffset) / Height;
                var deltaHeight = Height / (Content.Height * _startScale);
                var originY = (y - deltaY) * deltaHeight;

                var targetX = _xOffset - (originX * Content.Width) * (_currentScale - _startScale);
                var targetY = _yOffset - (originY * Content.Height) * (_currentScale - _startScale);

                var translationX = Double.Clamp(targetX, -Content.Width * (_currentScale - 1), 0);
                var translationY = Double.Clamp(targetY,-Content.Height * (_currentScale - 1), 0);
               
                if (isDoubleTap)
                {
                    var animate = new Animation();
                    animate.Add(0, 1, new Animation(d => Content.Scale = d, MinScale, _currentScale));
                    animate.Add(0, 1, new Animation(d => Content.TranslationX = d, 0, translationX));
                    animate.Add(0, 1, new Animation(d => Content.TranslationY = d, 0, translationY));

                    animate.Commit(this, "zoomIn", easing: Easing.CubicInOut, finished: (x, y) => {
                        EndGesture();
                    });
                }
                else
                {
                    Content.TranslationX = translationX;
                    Content.TranslationY = translationY;

                    Content.Scale = _currentScale;
                }
                scaleChangeManager?.RaiseEvent(this, new ScaleChangeEventArgs { Scale = _currentScale }, nameof(ScaleChanged));

                if (_currentScale > 1)
                {
                    if (!GestureRecognizers.Contains(pan))
                    {
                        GestureRecognizers.Add(pan);
                    }
                }
                else
                {
                    if (GestureRecognizers.Contains(pan))
                    {
                        GestureRecognizers.Remove(pan);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            
        }

        private void EndGesture()
        {
            _xOffset = Content.TranslationX;
            _yOffset = Content.TranslationY;
        }
    }

	public class ScaleChangeEventArgs : EventArgs
	{
		public double Scale { get; set; }
	}
}
