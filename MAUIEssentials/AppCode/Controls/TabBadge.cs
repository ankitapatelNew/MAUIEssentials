namespace MAUIEssentials.AppCode.Controls
{
    public static class TabBadge
	{
        public static BindableProperty BadgeTextProperty =
            BindableProperty.CreateAttached("BadgeText", typeof(string), typeof(TabBadge), default(string), BindingMode.TwoWay);

        public static BindableProperty BadgeColorProperty =
            BindableProperty.CreateAttached("BadgeColor", typeof(Color), typeof(TabBadge), new Color(), BindingMode.TwoWay);

        public static BindableProperty BadgeTextColorProperty =
            BindableProperty.CreateAttached("BadgeTextColor", typeof(Color), typeof(TabBadge), new Color(), BindingMode.TwoWay);

        public static BindableProperty BadgeFontSizeProperty =
            BindableProperty.CreateAttached("BadgeFontSize", typeof(double), typeof(TabBadge), 15d, BindingMode.TwoWay);

        public static BindableProperty BadgeFontFamilyProperty =
            BindableProperty.CreateAttached("BadgeFontFamily", typeof(string), typeof(TabBadge), string.Empty, BindingMode.TwoWay);

        public static BindableProperty BadgeFontAttributesProperty =
            BindableProperty.CreateAttached("BadgeFontAttribute", typeof(FontAttributes), typeof(TabBadge), FontAttributes.None, BindingMode.TwoWay);

        public static BindableProperty BadgePositionProperty =
            BindableProperty.CreateAttached("BadgePosition", typeof(BadgePosition), typeof(TabBadge), BadgePosition.PositionTopRight, BindingMode.TwoWay);

        public static BindableProperty BadgeMarginProperty =
            BindableProperty.CreateAttached("BadgeMargin", typeof(Thickness), typeof(TabBadge), DefaultMargins, BindingMode.TwoWay);

		#region BadgeText
		public static string GetBadgeText(BindableObject view)
        {
            return (string)view.GetValue(BadgeTextProperty);
        }

        public static void SetBadgeText(BindableObject view, string value)
        {
            view.SetValue(BadgeTextProperty, value);
        }
		#endregion

		#region BadgeColor
		public static Color GetBadgeColor(BindableObject view)
        {
            return (Color)view.GetValue(BadgeColorProperty);
        }

        public static void SetBadgeColor(BindableObject view, Color value)
        {
            view.SetValue(BadgeColorProperty, value);
        }
		#endregion

		#region BadgeTextColor
		public static Color GetBadgeTextColor(BindableObject view)
        {
            return (Color)view.GetValue(BadgeTextColorProperty);
        }

        public static void SetBadgeTextColor(BindableObject view, Color value)
        {
            view.SetValue(BadgeTextColorProperty, value);
        }
		#endregion

		#region BadgeFontSize
		public static double GetBadgeFontSize(BindableObject view)
        {
            return (double)view.GetValue(BadgeFontSizeProperty);
        }

        public static void SetBadgeFontSize(BindableObject view, double value)
        {
            view.SetValue(BadgeFontSizeProperty, value);
        }
        #endregion

        #region BadgeFontFamily
        public static string GetBadgeFontFamily(BindableObject view)
        {
            return (string)view.GetValue(BadgeFontFamilyProperty);
        }

        public static void SetBadgeFontFamily(BindableObject view, string value)
        {
            view.SetValue(BadgeFontFamilyProperty, value);
        }
        #endregion

        #region BadgeFontAttributes
        public static FontAttributes GetBadgeFontAttribute(BindableObject view)
        {
            return (FontAttributes)view.GetValue(BadgeFontAttributesProperty);
        }

        public static void SetBadgeFontAttribute(BindableObject view, FontAttributes value)
        {
            view.SetValue(BadgeFontAttributesProperty, value);
        }
		#endregion

		#region BadgePosition
		public static BadgePosition GetBadgePosition(BindableObject view)
        {
            return (BadgePosition)view.GetValue(BadgePositionProperty);
        }

        public static void SetBadgePosition(BindableObject view, BadgePosition value)
        {
            view.SetValue(BadgePositionProperty, value);
        }
		#endregion

		#region BadgeMargin
		public static Thickness GetBadgeMargin(BindableObject view)
        {
            return (Thickness)view.GetValue(BadgeMarginProperty);
        }

        public static void SetBadgeMargin(BindableObject view, Thickness value)
        {
            view.SetValue(BadgeMarginProperty, value);
        }
		#endregion

		public static Thickness DefaultMargins {
            get
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    return new Thickness(-10, -5);
                }
                
                // Default for all other platforms
                return new Thickness(0);
            }
        }

        public static Page GetChildPageWithBadge(this TabbedPage parentTabbedPage, int tabIndex)
        {
            var element = parentTabbedPage.Children[tabIndex];
            return GetPageWithBadge(element);
        }

        public static Page GetPageWithBadge(this Page element)
        {
            if (GetBadgeText(element) != (string)BadgeTextProperty.DefaultValue) {
                return element;
            }

            if (element is NavigationPage navigationPage) {
                //if the child page is a navigation page get its root page
                return navigationPage.RootPage;
            }

            return element;
        }
    }

    public enum BadgePosition
    {
        PositionTopRight = 0,
        PositionTopLeft = 1,
        PositionBottomRight = 2,
        PositionBottomLeft = 3,
        PositionCenter = 4,
        PositionTopCenter = 5,
        PositionBottomCenter = 6,
        PositionLeftCenter = 7,
        PositionRightCenter = 8,
    }
}