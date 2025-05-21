namespace MAUIEssentials.Platforms.iOS.Renderers
{
    public class CustomShellRenderer : ShellRenderer
    {
        protected override IShellTabBarAppearanceTracker CreateTabBarAppearanceTracker()
        {
            return new BadgeShellTabbarAppearanceTracker();
        }

        protected override IShellItemRenderer CreateShellItemRenderer(ShellItem item)
        {
            var renderer = base.CreateShellItemRenderer(item);

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad && UIDevice.CurrentDevice.CheckSystemVersion(18, 0) && renderer is ShellItemRenderer shellItemRenderer)
                shellItemRenderer.TraitOverrides.HorizontalSizeClass = UIUserInterfaceSizeClass.Compact;

            return renderer;
        }

        protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
        {
            return new CustomSectionRenderer(this);
        }
    }

    public class CustomSectionRenderer : ShellSectionRenderer
    {
        public CustomSectionRenderer(IShellContext context) : base(context)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            InteractivePopGestureRecognizer.Enabled = false;
        }
    }

    class BadgeShellTabbarAppearanceTracker : ShellTabBarAppearanceTracker
    {
        private UITabBarController? _controller;
        public override void UpdateLayout(UITabBarController controller)
        {
            base.UpdateLayout(controller);
            _controller = controller;
            BadgeCounterService.CountChanged += OnCountChanged;
        }

        private void OnCountChanged(object sender, TabCounter tabCounter)
        {
            UpdateBadge(tabCounter);
        }

        UIColor? _defaultTint;
        UITabBarAppearance? _tabBarAppearance;
        public override void SetAppearance(UITabBarController controller, ShellAppearance appearance)
        {
            IShellAppearanceElement appearanceElement = appearance;
            var tabBar = controller.TabBar;

            if (_defaultTint == null)
            {
                _defaultTint = tabBar.TintColor;
            }

            if (OperatingSystem.IsIOSVersionAtLeast(15) || OperatingSystem.IsTvOSVersionAtLeast(15))
                UpdateiOS15TabBarAppearance(controller, appearance);
            else
                UpdateTabBarAppearance(controller, appearance);

            // Ensure that the tab bar has been initialized before modifying icons
            if (controller?.ViewControllers != null && controller.TabBar?.Items != null)
            {
                SetTabBarIconSize(controller, 50); // Adjust icon size
            }
        }

        void SetTabBarIconSize(UITabBarController controller, float iconSize)
        {
            if (controller?.TabBar?.Items == null)
                return; // Ensure TabBar and Items are not null

            foreach (var item in controller.TabBar.Items)
            {
                if (item?.SelectedImage == null)
                    continue; // Skip if no selected image

                var image = item.SelectedImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                if (image != null)
                {
                    UIGraphics.BeginImageContextWithOptions(new CGSize(iconSize, iconSize), false, 0);
                    image.Draw(new CoreGraphics.CGRect(0, 0, iconSize, iconSize));
                    var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
                    UIGraphics.EndImageContext();

                    if (resizedImage != null)
                    {
                        item.SelectedImage = resizedImage;
                        item.Image = resizedImage;
                    }
                }
            }
        }

        void UpdateiOS15TabBarAppearance(UITabBarController controller, ShellAppearance appearance)
        {
            IShellAppearanceElement appearanceElement = appearance;

            var backgroundColor = appearanceElement.EffectiveTabBarBackgroundColor;
            var foregroundColor = appearanceElement.EffectiveTabBarForegroundColor;
            var unselectedColor = appearanceElement.EffectiveTabBarUnselectedColor;
            var titleColor = appearanceElement.EffectiveTabBarTitleColor;

            controller.TabBar.UpdateiOS15TabBarAppearance(
                    ref _tabBarAppearance,
                    null,
                    null,
                    foregroundColor ?? titleColor,
                    unselectedColor,
                    backgroundColor,
                    titleColor ?? foregroundColor,
                    unselectedColor);
        }

        void UpdateTabBarAppearance(UITabBarController controller, ShellAppearance appearance)
        {
            try
            {
                IShellAppearanceElement appearanceElement = appearance;
                var backgroundColor = appearanceElement.EffectiveTabBarBackgroundColor;
                var foregroundColor = appearanceElement.EffectiveTabBarForegroundColor;
                var unselectedColor = appearanceElement.EffectiveTabBarUnselectedColor;
                var titleColor = appearanceElement.EffectiveTabBarTitleColor;

                var tabBar = controller.TabBar;

                if (backgroundColor is not null && backgroundColor.IsNotDefault())
                    tabBar.BarTintColor = backgroundColor.ToPlatform();

                if (unselectedColor is not null && unselectedColor.IsNotDefault())
                {
                    tabBar.UnselectedItemTintColor = unselectedColor.ToPlatform();
                    UITabBarItem.Appearance.SetTitleTextAttributes(new UIStringAttributes { ForegroundColor = unselectedColor.ToPlatform() }, UIControlState.Normal);
                }

                if (titleColor is not null && titleColor.IsNotDefault() ||
                    foregroundColor is not null && foregroundColor.IsNotDefault())
                {
                    tabBar.TintColor = (foregroundColor ?? titleColor).ToPlatform();
                    UITabBarItem.Appearance.SetTitleTextAttributes(new UIStringAttributes { ForegroundColor = (titleColor ?? foregroundColor).ToPlatform() }, UIControlState.Selected);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void UpdateBadge(TabCounter tabCounter)
        {
            try
            {
                if (_controller != null && _controller.TabBar != null && _controller.TabBar.Items?.Length != 0)
                {
                    if (_controller.TabBar.Items?.ElementAtOrDefault(tabCounter.TabNumber) is UITabBarItem currentTabBarItem)
                    {
                        if (string.IsNullOrEmpty(tabCounter.BadgeText))
                        {
                            currentTabBarItem.BadgeValue = default;
                            //textColor = Colors.Transparent;
                        }
                        else
                        {
                            //textColor = Colors.Pink; //Color.Parse("#FF5C39");
                            currentTabBarItem.BadgeValue = "●";
                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                // Handle the object disposed exception gracefully
                Console.WriteLine($"Object disposed: {ex.Message}");
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _controller = null;
                BadgeCounterService.CountChanged -= OnCountChanged;
            }
            base.Dispose(disposing);
        }
    }

    internal static class TabbedViewExtensions
    {
        [System.Runtime.Versioning.SupportedOSPlatform("ios15.0")]
        [System.Runtime.Versioning.SupportedOSPlatform("tvos15.0")]
        internal static void UpdateiOS15TabBarAppearance(
            this UITabBar tabBar,
            ref UIKit.UITabBarAppearance? _tabBarAppearance,
            UIColor? defaultBarColor,
            UIColor? defaultBarTextColor,
            Color? selectedTabColor,
            Color? unselectedTabColor,
            Color? barBackgroundColor,
            Color? selectedBarTextColor,
            Color? unSelectedBarTextColor)
        {
            try
            {
                if (_tabBarAppearance == null)
                {
                    _tabBarAppearance = new UIKit.UITabBarAppearance();
                    _tabBarAppearance.ConfigureWithDefaultBackground();
                }

                var effectiveBarColor = (barBackgroundColor == null) ? defaultBarColor : barBackgroundColor.ToPlatform();
                // Set BarBackgroundColor
                if (effectiveBarColor != null)
                {
                    _tabBarAppearance.BackgroundColor = effectiveBarColor;
                }

                // Set BarTextColor

                var effectiveSelectedBarTextColor = (selectedBarTextColor == null) ? defaultBarTextColor : selectedBarTextColor.ToPlatform();
                var effectiveUnselectedBarTextColor = (unSelectedBarTextColor == null) ? defaultBarTextColor : unSelectedBarTextColor.ToPlatform();

                // Update colors for all variations of the appearance to also make it work for iPads, etc. which use different layouts for the tabbar
                // Also, set ParagraphStyle explicitly. This seems to be an iOS bug. If we don't do this, tab titles will be truncat...

                // Set SelectedTabColor
                if (selectedTabColor is not null)
                {
                    var foregroundColor = selectedTabColor.ToPlatform();
                    var titleColor = effectiveSelectedBarTextColor ?? foregroundColor;

                    _tabBarAppearance.StackedLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(13f), ForegroundColor = titleColor, ParagraphStyle = NSParagraphStyle.Default };
                    _tabBarAppearance.StackedLayoutAppearance.Selected.IconColor = foregroundColor;

                    _tabBarAppearance.InlineLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(13f), ForegroundColor = titleColor, ParagraphStyle = NSParagraphStyle.Default };
                    _tabBarAppearance.InlineLayoutAppearance.Selected.IconColor = foregroundColor;

                    _tabBarAppearance.CompactInlineLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(13f), ForegroundColor = titleColor, ParagraphStyle = NSParagraphStyle.Default };
                    _tabBarAppearance.CompactInlineLayoutAppearance.Selected.IconColor = foregroundColor;
                }
                else
                {
                    var foregroundColor = UITabBar.Appearance.TintColor;
                    var titleColor = effectiveSelectedBarTextColor ?? foregroundColor;
                    _tabBarAppearance.StackedLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(13f), ForegroundColor = titleColor, ParagraphStyle = NSParagraphStyle.Default };
                    _tabBarAppearance.StackedLayoutAppearance.Selected.IconColor = foregroundColor;

                    _tabBarAppearance.InlineLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(13f), ForegroundColor = titleColor, ParagraphStyle = NSParagraphStyle.Default };
                    _tabBarAppearance.InlineLayoutAppearance.Selected.IconColor = foregroundColor;

                    _tabBarAppearance.CompactInlineLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(13f), ForegroundColor = titleColor, ParagraphStyle = NSParagraphStyle.Default };
                    _tabBarAppearance.CompactInlineLayoutAppearance.Selected.IconColor = foregroundColor;
                }

                // Set UnselectedTabColor
                if (unselectedTabColor is not null)
                {
                    var foregroundColor = unselectedTabColor.ToPlatform();
                    var titleColor = effectiveUnselectedBarTextColor ?? foregroundColor;
                    _tabBarAppearance.StackedLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(13f), ForegroundColor = titleColor, ParagraphStyle = NSParagraphStyle.Default };
                    _tabBarAppearance.StackedLayoutAppearance.Normal.IconColor = foregroundColor;

                    _tabBarAppearance.InlineLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(13f), ForegroundColor = titleColor, ParagraphStyle = NSParagraphStyle.Default };
                    _tabBarAppearance.InlineLayoutAppearance.Normal.IconColor = foregroundColor;

                    _tabBarAppearance.CompactInlineLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(13f), ForegroundColor = titleColor, ParagraphStyle = NSParagraphStyle.Default };
                    _tabBarAppearance.CompactInlineLayoutAppearance.Normal.IconColor = foregroundColor;
                }
                else
                {
                    var foreground = UITabBar.Appearance.TintColor;
                    var titleColor = effectiveUnselectedBarTextColor ?? foreground;
                    _tabBarAppearance.StackedLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(13f), ForegroundColor = titleColor, ParagraphStyle = NSParagraphStyle.Default };
                    _tabBarAppearance.StackedLayoutAppearance.Normal.IconColor = foreground;

                    _tabBarAppearance.InlineLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(13f), ForegroundColor = titleColor, ParagraphStyle = NSParagraphStyle.Default };
                    _tabBarAppearance.InlineLayoutAppearance.Normal.IconColor = foreground;

                    _tabBarAppearance.CompactInlineLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(13f), ForegroundColor = titleColor, ParagraphStyle = NSParagraphStyle.Default };
                    _tabBarAppearance.CompactInlineLayoutAppearance.Normal.IconColor = foreground;
                }
                _tabBarAppearance.StackedLayoutAppearance.Normal.BadgeBackgroundColor = Colors.Transparent.ToPlatform();
                _tabBarAppearance.StackedLayoutAppearance.Normal.BadgeTextAttributes = new UIStringAttributes()
                {
                    ForegroundColor = Color.Parse("#FF5C39").ToPlatform(),
                };
                // Set the TabBarAppearance
                tabBar.StandardAppearance = tabBar.ScrollEdgeAppearance = _tabBarAppearance;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
