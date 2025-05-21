namespace MAUIEssentials.Platforms.iOS.Handlers
{
    public partial class CommanEntryHandler : EntryHandler
    {
        protected override MauiTextField CreatePlatformView()
        {
            var nativeView = base.CreatePlatformView();
            ConfigureControl(nativeView);
            return nativeView;
        }

        private void ConfigureControl(MauiTextField nativeView)
        {
            try
            {
                var entry = VirtualView as CommanEntry;

                if (entry != null)
                {
                    SetBorder(nativeView, entry);
                    SetCursorColor(nativeView, entry);

                    if (entry.Keyboard == Keyboard.Numeric || entry.Keyboard == Keyboard.Telephone)
                    {
                        SetToolbar(nativeView);
                    }

                    if (!entry.IsEnabled)
                    {
                        nativeView.TextColor = entry.TextColor.ToPlatform();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void SetBorder(MauiTextField nativeView, CommanEntry entry)
        {
            try
            {
                if (entry.IsBorder)
                {
                    nativeView.BorderStyle = UITextBorderStyle.Line;
                    nativeView.Layer.CornerRadius = entry.BorderRadius;
                    nativeView.Layer.BorderWidth = entry.BorderWidth;
                    nativeView.Layer.BorderColor = entry.BorderColor.ToCGColor();
                }
                else
                {
                    nativeView.BorderStyle = UITextBorderStyle.None;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void SetCursorColor(MauiTextField nativeView, CommanEntry entry)
        {
            try
            {
                nativeView.TintColor = entry.IsCursorVisible ? entry.TextColor.ToPlatform() : UIColor.Clear;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void SetToolbar(MauiTextField nativeView)
        {
            try
            {
                var entry = VirtualView as CommanEntry;

                if (entry != null)
                {
                    var space = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
                    var done = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (sender, e) =>
                        {
                            nativeView.EndEditing(true);
                        }
                    );

                    var toolbar = new UIToolbar
                    {
                        Items = new UIBarButtonItem[] { space, done }
                    };
                    toolbar.SizeToFit();
                    nativeView.InputAccessoryView = toolbar;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void MapIsBorder(CommanEntryHandler handler, CommanEntry entry)
        {
            try
            {
                handler.UpdateBorder(entry);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void MapBorderWidth(CommanEntryHandler handler, CommanEntry entry)
        {
            try
            {
                handler.UpdateBorder(entry);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void MapBorderRadius(CommanEntryHandler handler, CommanEntry entry)
        {
            try
            {
                handler.UpdateBorder(entry);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void MapBorderColor(CommanEntryHandler handler, CommanEntry entry)
        {
            try
            {
                handler.UpdateBorder(entry);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void MapIsCursorVisible(CommanEntryHandler handler, CommanEntry entry)
        {
            try
            {
                handler.UpdateCursorColor(entry);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void UpdateBorder(CommanEntry entry)
        {
            try
            {
                if (PlatformView != null)
                {
                    SetBorder(PlatformView, entry);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void UpdateCursorColor(CommanEntry entry)
        {
            try
            {
                if (PlatformView != null)
                {
                    SetCursorColor(PlatformView, entry);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}
