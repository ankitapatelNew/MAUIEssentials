using Android.Content;
using Android.Runtime;
using Android.Util;
using AndroidX.RecyclerView.Widget;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.Platforms.Android.Renderers;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using PointF = Android.Graphics.PointF;

[assembly: ExportRenderer(typeof(CustomCollectionView), typeof(CustomCollectionViewRenderer))]
namespace MAUIEssentials.Platforms.Android.Renderers
{
    public class CustomCollectionViewRenderer : CollectionViewRenderer
    {
        public CustomCollectionViewRenderer(Context context) : base(context)
        {
        }


        protected override void OnElementChanged(ElementChangedEventArgs<ItemsView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var element = e.NewElement as CustomCollectionView;
                SetLayoutManager(new CustomLayoutManager(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, LinearLayoutManager.Horizontal, false, element.ScrollDuration));

            }
        }
    }

    public class CustomLayoutManager : LinearLayoutManager
    {
        private float ScrollDuration = 30000f;
        public CustomLayoutManager(Context context) : base(context)
        {
        }
        public CustomLayoutManager(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }
        public CustomLayoutManager(Context context, int orientation, bool reverseLayout, float scrollDuration) : base(context, orientation, reverseLayout)
        {
            ScrollDuration = scrollDuration;
        }
        public CustomLayoutManager(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }
        public override void SmoothScrollToPosition(RecyclerView recyclerView, RecyclerView.State state, int position)
        {
            CustomScroller scroller = new CustomScroller(recyclerView.Context, ScrollDuration, recyclerView);

            scroller.TargetPosition = position;
            StartSmoothScroll(scroller);
        }
        void setDuration(float duration)
        {
            ScrollDuration = duration;
        }
    }

    public class CustomScroller : LinearSmoothScroller
    {
        private float ScrollDuration;
        private RecyclerView recyclerView;
        public CustomScroller(Context context, float scrollDuration, RecyclerView recyclerView) : base(context)
        {
            ScrollDuration = scrollDuration;
        }

        public override PointF ComputeScrollVectorForPosition(int targetPosition)
        {
            return base.ComputeScrollVectorForPosition(targetPosition);
        }

        protected override float CalculateSpeedPerPixel(DisplayMetrics displayMetrics)
        {
            return ScrollDuration / recyclerView.ComputeVerticalScrollRange();
        }
    }
}
