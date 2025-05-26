using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Google.Android.Material.Badge;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using System.ComponentModel;
using Microsoft.Maui.Controls.Platform;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.Platforms.Android.Renderers
{
    public class CustomShellRenderer : Microsoft.Maui.Controls.Handlers.Compatibility.ShellRenderer
    {
        public CustomShellRenderer(Context context) : base(context)
        {
        }

        protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
        {
            return new CustomShellSectionRenderer(this);
        }

        protected override IShellItemRenderer CreateShellItemRenderer(ShellItem shellItem)
        {
            return new CutommShellItemRenderer(this);
        }

        protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
        {
            return new BadgeShellBottomNavViewAppearanceTracker(this, shellItem);
        }

    }

    public class CutommShellItemRenderer : ShellItemRenderer
    {
        public CutommShellItemRenderer(IShellContext shellContext) : base(shellContext)
        {
        }

        protected override void OnTabReselected(ShellSection shellSection)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            await shellSection?.Navigation?.PopToRootAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override global::Android.Views.View OnCreateView(
           LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var outerlayout = base.OnCreateView(inflater, container, savedInstanceState);
            return outerlayout;
        }

        protected override void OnShellSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                base.OnShellSectionPropertyChanged(sender, e);
                MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            var item = (ShellSection)sender;
                            await item?.Navigation?.PopToRootAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    class BadgeShellBottomNavViewAppearanceTracker : ShellBottomNavViewAppearanceTracker
    {
        BottomNavigationView _bottomView;

        public BadgeShellBottomNavViewAppearanceTracker(IShellContext shellContext, ShellItem shellItem)
            : base(shellContext, shellItem)
        {
        }

        public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
        {
            try
            {
                base.SetAppearance(bottomView, appearance);

                _bottomView = bottomView;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    bottomView.Elevation = 10; // Adjust elevation as needed
                }
                var drawable = new GradientDrawable();
                drawable.SetColor(global::Android.Graphics.Color.White); // Transparent background
                drawable.SetStroke(2, global::Android.Graphics.Color.ParseColor("#e8e8e8")); // Border width and color
                bottomView.SetBackground(drawable);
                bottomView.ItemIconSize = 120;
                BadgeCounterService.CountChanged += BadgeCounterService_CountChanged;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void BadgeCounterService_CountChanged(object sender, TabCounter tabCounter)
        {
            try
            {
                UpdateBadge(tabCounter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateBadge(TabCounter tabCounter)
        {
            try
            {
                if (_bottomView != null && _bottomView.Handle != IntPtr.Zero)
                {
                    var badgeDrawable = _bottomView.GetOrCreateBadge((int)tabCounter.TabNumber);
                    badgeDrawable.HorizontalOffset = 10; // Adjust horizontal offset
                    badgeDrawable.VerticalOffset = 10;   // Adjust vertical offset

                    if (badgeDrawable is not null)
                    {
                        if (string.IsNullOrEmpty(tabCounter.BadgeText))
                        {
                            badgeDrawable.SetVisible(false);
                        }
                        else
                        {
                            //badgeDrawable.Number = tabCounter.BadgeCount;
                            badgeDrawable.BackgroundColor = Color.Parse("#FF5C39").ToPlatform();
                            badgeDrawable.BadgeTextColor = Colors.White.ToPlatform();
                            badgeDrawable.SetVisible(true);
                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine($"Object disposed: {ex.Message}");
            }
            catch (Java.Lang.IllegalArgumentException ex)
            {
                Console.WriteLine($"Object disposed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);
                BadgeCounterService.CountChanged -= BadgeCounterService_CountChanged; // Unsubscribe from the event
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class CustomShellSectionRenderer : ShellSectionRenderer
    {
        public CustomShellSectionRenderer(IShellContext shellContext) : base(shellContext)
        {
        }

        public override global::Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var result = base.OnCreateView(inflater, container, savedInstanceState);
            SetViewPager2UserInputEnabled(false);
            return result;
        }
        protected override void SetViewPager2UserInputEnabled(bool value)
        {
            base.SetViewPager2UserInputEnabled(false);
        }
    }
}
