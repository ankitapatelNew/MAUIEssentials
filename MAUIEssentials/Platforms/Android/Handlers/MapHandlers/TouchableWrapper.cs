using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MAUIEssentials.Platforms.Android.MapHandlers
{
    public class TouchableWrapper : FrameLayout
    {
        public Action TouchDown;
        public Action TouchUp;

        #region ctors
        protected TouchableWrapper(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        public TouchableWrapper(Context context) : this(context, null) { }
        public TouchableWrapper(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }
        public TouchableWrapper(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }
        #endregion

        public override bool DispatchTouchEvent(MotionEvent? e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    TouchDown?.Invoke();
                    break;
                case MotionEventActions.Cancel:
                case MotionEventActions.Up:
                    TouchUp?.Invoke();
                    break;
            }

            return base.DispatchTouchEvent(e);
        }
    }
}