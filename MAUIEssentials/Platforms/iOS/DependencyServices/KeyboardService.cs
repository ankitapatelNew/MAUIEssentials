
using MAUIEssentials.DependencyServices;
using MAUIEssentials.AppCode.Helpers;
using UIKit;

namespace MAUIEssentials.DepedencyServices
{
    public class KeyboardServiceImplementation : FirebaseEssentials.Shared.DisposableBase, IKeyboardService
    {
        public event EventHandler KeyboardIsShown;
        public event EventHandler KeyboardIsHidden;

        private double keyboardHeight = 0;
        bool isKeyboardVisible;

        public KeyboardServiceImplementation()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            UIKeyboard.Notifications.ObserveDidShow(OnKeyboardDidShow);
            UIKeyboard.Notifications.ObserveDidHide(OnKeyboardDidHide);

            UIKeyboard.Notifications.ObserveWillShow((sender, e) => {
                keyboardHeight = e.FrameEnd.Height;
            });
        }

        private void OnKeyboardDidShow(object sender, EventArgs e)
        {
            isKeyboardVisible = true;
            KeyboardIsShown?.Invoke(this, EventArgs.Empty);
        }

        private void OnKeyboardDidHide(object sender, EventArgs e)
        {
            isKeyboardVisible = false;
            KeyboardIsHidden?.Invoke(this, EventArgs.Empty);
        }

        public void HideKeyboard()
        {
            try
            {

                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                   {
                       var window = UIApplication.SharedApplication?.KeyWindow;
                       var vc = window?.RootViewController;
                       while (vc?.PresentedViewController != null)
                       {
                           vc = vc.PresentedViewController;
                       }

                       vc?.View?.EndEditing(true);
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
