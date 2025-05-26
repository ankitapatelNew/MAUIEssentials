using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;
using View = Android.Views.View;

namespace MAUIEssentials.Platforms.Android.DepedencyServices
{
    public class KeyboardService : Java.Lang.Object, IKeyboardService, ViewTreeObserver.IOnGlobalLayoutListener
    {
        public event EventHandler KeyboardIsShown;
        public event EventHandler KeyboardIsHidden;

        private readonly View _rootView;

        private bool wasShown = false;
        private int keypadHeight = 0;

        public KeyboardService()
        {
            try
            {
                _rootView = Platform.CurrentActivity.Window.DecorView.FindViewById(global::Android.Resource.Id.Content);
                SubscribeEvents();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void SubscribeEvents()
        {
            try
            {
                Platform.CurrentActivity.Window.DecorView.ViewTreeObserver.AddOnGlobalLayoutListener(this);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public void OnGlobalLayout()
        {
            try
            {
                global::Android.Graphics.Rect r = new global::Android.Graphics.Rect();
                Platform.CurrentActivity.Window.DecorView.GetWindowVisibleDisplayFrame(r);

                var screenHeight = Platform.CurrentActivity.Window.DecorView.RootView.Height;
                keypadHeight = (int)(screenHeight - r.Bottom);

                if (keypadHeight > screenHeight * 0.15)
                {
                    if (!wasShown)
                    {
                        KeyboardIsShown?.Invoke(this, EventArgs.Empty);
                        wasShown = true;
                    }
                    else
                    {
                        if (wasShown)
                        {
                            KeyboardIsHidden?.Invoke(this, EventArgs.Empty);
                            wasShown = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public void HideKeyboard()
        {
            try
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
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public double KeyboardHeight()
        {
            var density = Platform.CurrentActivity.Resources.DisplayMetrics.Density;
            return (keypadHeight / density);
        }
    }
}
