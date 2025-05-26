using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using MAUIEssentials.AppCode.DependencyServices;
using View = Android.Views.View;

namespace MAUIEssentials.DepedencyServices
{
    public class KeyboardServiceImplementation : Java.Lang.Object, IKeyboardService, ViewTreeObserver.IOnGlobalLayoutListener
    {
        public event EventHandler KeyboardIsShown;
        public event EventHandler KeyboardIsHidden;
        private readonly View _rootView;

        private bool wasShown = false;
        private int keypadHeight = 0;

        public KeyboardServiceImplementation()
        {
            var context = Platform.CurrentActivity ??
                        throw new InvalidOperationException("CurrentActivity is null");

            _rootView = context.FindViewById(Android.Resource.Id.Content) ??
                        throw new InvalidOperationException("DecorView or Content view not found");

            SubscribeEvents();
        }

        public void OnGlobalLayout()
        {
            var context = Platform.CurrentActivity;
            if (context?.Window?.DecorView == null)
                return;

            var r = new global::Android.Graphics.Rect();
            _rootView.GetWindowVisibleDisplayFrame(r);

            var screenHeight = _rootView.RootView.Height;
            keypadHeight = screenHeight - r.Bottom;

            bool isKeyboardNowVisible = keypadHeight > screenHeight * 0.15;

            if (isKeyboardNowVisible && !wasShown)
            {
                KeyboardIsShown?.Invoke(this, EventArgs.Empty);
                wasShown = true;
            }
            else if (!isKeyboardNowVisible && wasShown)
            {
                KeyboardIsHidden?.Invoke(this, EventArgs.Empty);
                wasShown = false;
            }
        }

        private void SubscribeEvents()
        {
            _rootView.ViewTreeObserver?.AddOnGlobalLayoutListener(this);
        }

        public void HideKeyboard()
        {
            var context = Platform.CurrentActivity;
            if (context != null)
            {
                var inputMethodManager = (InputMethodManager?)context.GetSystemService(Context.InputMethodService);
                var currentFocus = context.CurrentFocus;
                if (currentFocus != null && inputMethodManager != null)
                {
                    inputMethodManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
                }
            }
        }

        public double KeyboardHeight()
        {
            var context = Platform.CurrentActivity;
            if (context?.Resources?.DisplayMetrics == null)
                return 0;
            
           var density = context.Resources?.DisplayMetrics?.Density ?? 1.0f;
            return keypadHeight / density;
        }
    }
}
