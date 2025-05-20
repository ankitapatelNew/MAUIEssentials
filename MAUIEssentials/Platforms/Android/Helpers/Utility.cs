namespace MAUIEssentials.Platforms.Android.Helpers
{
    public static class Utility
    {
        public static float DpToPixels(int dp)
        {
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, Platform.CurrentActivity?.Resources?.DisplayMetrics);
        }

        public static float DpToPixels(float dp)
        {
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, Platform.CurrentActivity?.Resources?.DisplayMetrics);
        }

        public static int GetDrawableResId(string resName)
        {
            try
            {
                var activity = Platform.CurrentActivity;
                int resID = activity.Resources.GetIdentifier(resName, "drawable", activity.PackageName);
                if (resID == 0)
                {
                    resID = (int)typeof(global::Android.Resource.Drawable).GetField(resName).GetValue(null);
                }
                return resID;
            }
            catch (Exception ex)
            {
                ex.LogException();
                return 0;
            }
        }

        public static Color SetColorAlpha(string color, int alpha)
        {
            string alphaColorString = color.Insert(1, alpha.ToString("00"));
            Color alphaColor = Color.ParseColor(alphaColorString);
            return alphaColor;
        }

        public static string GetHexString(this Color color)
        {
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var alpha = (int)(color.A * 255);

            //var hex = $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";
            var hex = $"#{red:X2}{green:X2}{blue:X2}";

            return hex;
        }

        public static Drawable CustomDrawable(Color color, int cornerRadius)
        {
            GradientDrawable drawable = new GradientDrawable();
            drawable.SetShape(ShapeType.Rectangle);
            drawable.SetCornerRadius(DpToPixels(cornerRadius));
            drawable.SetColor(color);
            return drawable;
        }

        public static Drawable CustomDrawable(Color strokeColor, int cornerRadius, int strokeWidth, string fillColor = "", bool isOval = false)
        {
            GradientDrawable drawable = new GradientDrawable();
            drawable.SetShape(isOval ? ShapeType.Oval : ShapeType.Rectangle);
            drawable.SetStroke((int)DpToPixels(strokeWidth), strokeColor);

            if (!isOval)
            {
                drawable.SetCornerRadius(DpToPixels(cornerRadius));
            }

            if (!string.IsNullOrEmpty(fillColor))
            {
                drawable.SetColor(Color.ParseColor(fillColor));
            }
            return drawable;
        }

        public static Drawable CustomDrawable(Color color, int leftTopRadius, int rightTopRadius, int rightBottomRadius, int leftBottomRadius, int height = 0, int width = 0)
        {
            GradientDrawable drawable = new GradientDrawable();
            drawable.SetShape(ShapeType.Rectangle);
            drawable.SetColor(color);
            drawable.SetCornerRadii(new float[] { DpToPixels(leftTopRadius), DpToPixels(leftTopRadius), DpToPixels(rightTopRadius), DpToPixels(rightTopRadius), DpToPixels(rightBottomRadius), DpToPixels(rightBottomRadius), DpToPixels(leftBottomRadius), DpToPixels(leftBottomRadius) });

            if (height > 0 && width > 0)
            {
                drawable.SetSize((int)DpToPixels(width), (int)DpToPixels(height));
            }
            return drawable;
        }

        public static Drawable CustomGradientDrawable(Color startColor, Color centerColor, Color endColor)
        {
            GradientDrawable drawable = new GradientDrawable();
            drawable.SetShape(ShapeType.Rectangle);
            drawable.SetGradientType(GradientType.LinearGradient);
            drawable.SetGradientCenter(100, 0);
            drawable.SetOrientation(GradientDrawable.Orientation.LeftRight);
            drawable.SetColors(new int[] { startColor.ToArgb(), endColor.ToArgb() });
            return drawable;
        }
    }
}