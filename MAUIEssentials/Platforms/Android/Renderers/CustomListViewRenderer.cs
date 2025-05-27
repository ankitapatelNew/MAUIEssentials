using Android.Content;
using Android.Views;
using MAUIEssentials.AppCode.Controls;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using static Android.Views.View;

namespace MAUIEssentials.Platforms.Android.Renderers
{
    public class CustomListViewRenderer : ListViewRenderer, IOnScrollChangeListener
    {
        readonly CustomGestureListener _listener;
        readonly GestureDetector _detector;
        public CustomListViewRenderer(Context context) : base(context)
        {
            _listener = new CustomGestureListener();
            _detector = new GestureDetector(context, _listener);
        }

        protected override void DisconnectHandler(global::Android.Widget.ListView oldPlatformView)
        {
            if (_listener != null)
            {
                _listener.OnSwipeLeft += HandleOnSwipeLeft;
                _listener.OnSwipeRight += HandleOnSwipeRight;
            }
            base.DisconnectHandler(oldPlatformView);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null && e.NewElement == null)
            {
                _listener.OnSwipeLeft -= HandleOnSwipeLeft;
                _listener.OnSwipeRight -= HandleOnSwipeRight;
                return;
            }

            if (e.OldElement != null)
            {
                var element = e.OldElement as CustomListView;
                element.EventScrollToTop -= View_EventScrollToTop;
                _listener.OnSwipeLeft -= HandleOnSwipeLeft;
                _listener.OnSwipeRight -= HandleOnSwipeRight;
            }

            if (e.NewElement != null)
            {
                var element = e.NewElement as CustomListView;
                element.EventScrollToTop += View_EventScrollToTop;
                _listener.OnSwipeLeft += HandleOnSwipeLeft;
                _listener.OnSwipeRight += HandleOnSwipeRight;
                Control.SetOnScrollChangeListener(this);
            }
        }

        void View_EventScrollToTop(object sender, EventArgs e)
        {
            Control?.SmoothScrollToPositionFromTop(0, 0);
        }

        public override bool OnTouchEvent(MotionEvent? ev)
        {
            base.OnTouchEvent(ev);
            if (_detector != null)
            {
                return _detector.OnTouchEvent(ev);
            }

            return false;
        }

        public override bool DispatchTouchEvent(MotionEvent? e)
        {
            if (_detector != null)
            {
                _detector.OnTouchEvent(e);
                base.DispatchTouchEvent(e);
                return true;
            }

            return base.DispatchTouchEvent(e);
        }

        public override bool OnDragEvent(DragEvent? e)
        {
            return base.OnDragEvent(e);
        }


        void HandleOnSwipeLeft(object sender, EventArgs e) =>
           ((CustomListView)Element).OnSwiped(SwipeDirection.Left);

        void HandleOnSwipeRight(object sender, EventArgs e) =>
            ((CustomListView)Element).OnSwiped(SwipeDirection.Right);

        public void OnScrollChange(global::Android.Views.View? v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        {
            var firstVisibleItemIndex = Control.FirstVisiblePosition;
            ((CustomListView)Element).NotifyFirstVisibleItemIndexChanged(firstVisibleItemIndex);
        }
    }
    public class CustomGestureListener : GestureDetector.SimpleOnGestureListener
    {
        static readonly int SWIPE_THRESHOLD = 100;
        static readonly int SWIPE_VELOCITY_THRESHOLD = 100;

        MotionEvent? mLastOnDownEvent;

        public event EventHandler? OnSwipeLeft;
        public event EventHandler? OnSwipeRight;

        public override bool OnDown(MotionEvent e)
        {
            mLastOnDownEvent = e;
            return true;
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            if (e1 == null)
            {
                e1 = mLastOnDownEvent;
            }

            float diffY = e2.GetY() - e1.GetY();
            float diffX = e2.GetX() - e1.GetX();

            if (Math.Abs(diffX) > Math.Abs(diffY))
            {
                if (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD)
                {
                    if (diffX > 0)
                    {
                        OnSwipeRight?.Invoke(this, null);
                    }
                    else
                    {
                        OnSwipeLeft?.Invoke(this, null);
                    }
                }
            }

            return base.OnFling(e1, e2, velocityX, velocityY);
        }
    }
}
