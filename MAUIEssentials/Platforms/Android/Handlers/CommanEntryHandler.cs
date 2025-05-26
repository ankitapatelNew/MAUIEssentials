using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.Platforms.Android.Helpers;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;
using Color = Android.Graphics.Color;
using View = Android.Views.View;

namespace MAUIEssentials.Platforms.Android.Handlers
{
    public class CommanEntryHandler: EntryHandler
    {
        bool _disposed;

        public static IPropertyMapper<CommanEntry, CommanEntryHandler> PropertyMapper = new PropertyMapper<CommanEntry, CommanEntryHandler>(Mapper)
        {
            [nameof(CommanEntry.TextColor)] = SetCursorColor,
            [nameof(CommanEntry.BorderColor)] = SetBorder,
            [nameof(CommanEntry.BorderWidth)] = SetBorder,
            [nameof(CommanEntry.BorderRadius)] = SetBorder,
            [nameof(CommanEntry.IsEnabled)] = SetIsEnabled,
            [nameof(CommanEntry.ReturnType)] = SetReturnType,
        };

        public CommanEntryHandler() : base(PropertyMapper)
        {
        }

        protected override AppCompatEditText CreatePlatformView()
        {
            var editText = new AppCompatEditText(Context)
            {
                ImeOptions = ImeAction.Done
            };

            // Listen for focus changes to dynamically update ImeOptions
            editText.FocusChange += OnPlatformViewFocusChange;

            return editText;
        }

        protected override void ConnectHandler(AppCompatEditText platformView)
        {
            try
            {
                base.ConnectHandler(platformView);
                if (!_disposed)
                {
                    if (platformView != null)
                    {
                        PlatformView.BackgroundTintList = ColorStateList.ValueOf(Color.Transparent);
                        platformView.SetBackgroundColor(Color.Transparent);
                        platformView.SetPadding(0, 0, 0, 0);
                        platformView.SetOnKeyListener(new KeyListener(VirtualView as CommanEntry));
                        //platformView.KeyPress -= CommanEntryHandler_KeyPress;
                        //platformView.KeyPress += CommanEntryHandler_KeyPress;

                        // Set the ImeOptions based on the ReturnType
                        SetReturnType(this, VirtualView as CommanEntry);
                    }
                }
            }
            catch (Exception ex)
            {
               ex.LogException();
            }
        }

        private void OnPlatformViewFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
            {
                if (e.HasFocus)
                {
                    // When the field gains focus, update the ImeOptions
                    SetReturnType(this, VirtualView as CommanEntry);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void SetReturnType(CommanEntryHandler handler, CommanEntry entry)
        {
            try
            {
                if (entry == null || handler.PlatformView == null)
                    return;

                switch (entry.ReturnType)
                {
                    case ReturnType.Default:
                    case ReturnType.Done:
                        handler.PlatformView.ImeOptions = ImeAction.Done;
                        break;
                    case ReturnType.Next:
                        handler.PlatformView.ImeOptions = ImeAction.Next;
                        break;
                    case ReturnType.Send:
                        handler.PlatformView.ImeOptions = ImeAction.Send;
                        break;
                    case ReturnType.Search:
                        handler.PlatformView.ImeOptions = ImeAction.Search;
                        break;
                    case ReturnType.Go:
                        handler.PlatformView.ImeOptions = ImeAction.Go;
                        break;
                    default:
                        handler.PlatformView.ImeOptions = ImeAction.Done;
                        break;
                }

                // Force the keyboard to update its ImeOptions
                handler.PlatformView.InputType = handler.PlatformView.InputType; // This forces the keyboard to refresh
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void CommanEntryHandler_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Del)
                {
                    if (string.IsNullOrWhiteSpace(((EditText)sender).Text))
                    {
                        var entry = VirtualView as CommanEntry;
                        entry?.OnBackButtonPress();
                    }
                }
            }
            catch (Exception ex)
            {
               ex.LogException();
            }
        }

        private void PlatformView_AfterTextChanged(object sender, global::Android.Text.AfterTextChangedEventArgs e)
        {
            try
            {
                if (VirtualView is CommanEntry entry)
                {
                    entry.Text = ((EditText)sender).Text;
                }
            }
            catch (Exception ex)
            {
               ex.LogException();
            }
        }

        protected override void DisconnectHandler(AppCompatEditText platformView)
        {
            try
            {
                base.DisconnectHandler(platformView);
                _disposed = true;
                platformView.KeyPress -= CommanEntryHandler_KeyPress;
                platformView.FocusChange -= OnPlatformViewFocusChange; 
            }
            catch (Exception ex)
            {
               ex.LogException();
            }
        }

        public static void SetCursorColor(CommanEntryHandler handler, CommanEntry entry)
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                {
                    handler.PlatformView.SetTextCursorDrawable(0);
                }
                else
                {
                    IntPtr IntPtrtextViewClass = JNIEnv.FindClass(typeof(TextView));
                    IntPtr mCursorDrawableResProperty = JNIEnv.GetFieldID(IntPtrtextViewClass, "mCursorDrawableRes", "I");
                    JNIEnv.SetField(handler.PlatformView.Handle, mCursorDrawableResProperty, 0);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void SetBorder(CommanEntryHandler handler, CommanEntry entry)
        {
            try
            {
                if (entry.IsBorder)
                {
                    handler.PlatformView.Background = Utility.CustomDrawable(entry.BorderColor.ToAndroid(), entry.BorderRadius, entry.BorderWidth, entry.BackgroundColor.ToHex());
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void SetIsEnabled(CommanEntryHandler handler, CommanEntry entry)
        {
            try
            {
                handler.PlatformView.SetTextColor(entry.TextColor.ToAndroid());
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }

    public class KeyListener : Java.Lang.Object, View.IOnKeyListener
    {
        private readonly CommanEntry _customEntry;

        public KeyListener(CommanEntry customEntry)
        {
            try
            {
                _customEntry = customEntry;
            }
            catch (Exception ex)
            {
               ex.LogException();
            }
        }

        public bool OnKey(View v, Keycode keyCode, KeyEvent e)
        {
            try
            {
                if (e.Action == KeyEventActions.Down && keyCode == Keycode.Del)
                {
                    if (string.IsNullOrWhiteSpace(((EditText)v).Text))
                    {
                        _customEntry?.OnBackButtonPress();
                    }
                }
            }
            catch (Exception ex)
            {
               ex.LogException();
            }
            return false;
        }
    }
}
