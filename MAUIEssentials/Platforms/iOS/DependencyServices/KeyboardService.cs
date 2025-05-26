using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;
using UIKit;

namespace MAUIEssentials.Platforms.iOS.DependencyServices
{
    public class KeyboardService : IKeyboardService
    {
        public event EventHandler KeyboardIsShown;
        public event EventHandler KeyboardIsHidden;

        private double keyboardHeight = 0;
        bool isKeyboardVisible;

        public KeyboardService()
        {
            try
            {
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
                UIKeyboard.Notifications.ObserveDidShow(OnKeyboardDidShow);
                UIKeyboard.Notifications.ObserveDidHide(OnKeyboardDidHide);

                UIKeyboard.Notifications.ObserveWillShow((sender, e) =>
                {
                    keyboardHeight = e.FrameEnd.Height;
                });
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void OnKeyboardDidShow(object sender, EventArgs e)
        {
            try
            {
                isKeyboardVisible = true;
                KeyboardIsShown?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void OnKeyboardDidHide(object sender, EventArgs e)
        {
            try
            {
                isKeyboardVisible = false;
                KeyboardIsHidden?.Invoke(this, EventArgs.Empty);
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
                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    var window = UIApplication.SharedApplication.KeyWindow;
                    var vc = window.RootViewController;
                    while (vc.PresentedViewController != null)
                    {
                        vc = vc.PresentedViewController;
                    }

                    vc.View.EndEditing(true);
                });
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public double KeyboardHeight()
        {
            return keyboardHeight;
        }
    }
}
