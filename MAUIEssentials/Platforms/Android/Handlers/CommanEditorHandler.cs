using Android.Text.Method;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Text;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.Platforms.Android.Helpers;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;

namespace MAUIEssentials.Platforms.Android.Handlers
{
    public class CommanEditorHandler: EditorHandler
    {
        protected override AppCompatEditText CreatePlatformView()
        {
            var nativeEditText = base.CreatePlatformView();
            try
            {
                ConfigureEditor(nativeEditText);
                RemoveUnderline(nativeEditText);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return nativeEditText;
        }

        protected override void ConnectHandler(AppCompatEditText platformView)
        {
            try
            {
                base.ConnectHandler(platformView);

                if (VirtualView is CommanEditor editor)
                {
                    UpdateBorder(editor);
                    UpdateAttributedText(editor);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void ConfigureEditor(AppCompatEditText nativeEditText)
        {
            try
            {
                nativeEditText.Background = null;
                nativeEditText.SetBackgroundColor(global::Android.Graphics.Color.Transparent);
                nativeEditText.SetPadding(0, 0, 0, 0);

                // Scroll configuration
                nativeEditText.OverScrollMode = OverScrollMode.Always;
                nativeEditText.ScrollBarStyle = ScrollbarStyles.InsideInset;
                nativeEditText.SetOnTouchListener(new DroidTouchListener());

                // Text scrolling
                nativeEditText.VerticalScrollBarEnabled = true;
                nativeEditText.MovementMethod = ScrollingMovementMethod.Instance;
                nativeEditText.Gravity = GravityFlags.Top;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void RemoveUnderline(AppCompatEditText editText)
        {
            try
            {
                editText.BackgroundTintList = global::Android.Content.Res.ColorStateList.ValueOf(global::Android.Graphics.Color.Transparent);
                editText.SetBackgroundColor(global::Android.Graphics.Color.Transparent);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void MapAttributedText(CommanEditorHandler handler, CommanEditor editor)
        {
            try
            {
                handler?.UpdateAttributedText(editor);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void MapBorderProperties(CommanEditorHandler handler, CommanEditor editor)
        {
            try
            {
                handler?.UpdateBorder(editor);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void UpdateAttributedText(CommanEditor editor)
        {
            try
            {
                if (editor?.AttributedText == true && !string.IsNullOrEmpty(editor.Text))
                {
                    var htmlSpanned = HtmlCompat.FromHtml(editor.Text, HtmlCompat.FromHtmlModeLegacy);
                    PlatformView.SetText(htmlSpanned, TextView.BufferType.Spannable);
                    PlatformView.MovementMethod = LinkMovementMethod.Instance;
                    PlatformView.SetMaxLines(10000);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void UpdateBorder(CommanEditor editor)
        {
            try
            {
                if (editor?.IsBorder == true && PlatformView != null)
                {
                    var backgroundDrawable = Utility.CustomDrawable(
                        editor.BorderColor.ToAndroid(),
                        editor.BorderRadius,
                        editor.BorderWidth,
                        editor.BackgroundColor?.ToHex()
                    );
                    PlatformView.Background = backgroundDrawable;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }

    public class DroidTouchListener : Java.Lang.Object, global::Android.Views.View.IOnTouchListener
    {
        public bool OnTouch(global::Android.Views.View? v, MotionEvent? e)
        {
            v?.Parent?.RequestDisallowInterceptTouchEvent(true);
            if ((e?.Action & MotionEventActions.Up) != 0 && (e?.ActionMasked & MotionEventActions.Up) != 0)
            {
                v?.Parent?.RequestDisallowInterceptTouchEvent(false);
            }
            return false;
        }
    }
}
