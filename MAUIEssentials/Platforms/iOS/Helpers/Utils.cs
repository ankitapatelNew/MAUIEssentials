namespace MAUIEssentials.Platforms.iOS.Helpers
{
    public static class Utils
	{
        public static UIViewController GetTopViewController()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }
            return vc;
        }

        public static UIFont CustomFont(string fontFamily, FontAttributes attributes, float fontSize)
        {
            if (!string.IsNullOrEmpty(fontFamily))
            {
                return UIFont.FromName(fontFamily, fontSize);
            }

            switch (attributes)
            {
                case FontAttributes.Bold:
                    return UIFont.BoldSystemFontOfSize(fontSize);
                case FontAttributes.Italic:
                    return UIFont.ItalicSystemFontOfSize(fontSize);
                default:
                    return UIFont.SystemFontOfSize(fontSize);
            }
        }

        internal static UIFont WithTraitsOfFont(this UIFont self, UIFont font)
        {
            UIFontDescriptor fontDescriptor = self.FontDescriptor;
            UIFontDescriptorSymbolicTraits traits = fontDescriptor.SymbolicTraits;
            traits = traits | font.FontDescriptor.SymbolicTraits;
            UIFontDescriptor boldFontDescriptor = fontDescriptor.CreateWithTraits(traits);
            return UIFont.FromDescriptor(boldFontDescriptor, self.PointSize);
        }

        public static CAShapeLayer BottomBothCornerRadius(CGRect frame, float radius = 15)
        {
            frame = new CGRect(0, 0, frame.Width, frame.Height);
            CAShapeLayer maskLayer = new CAShapeLayer();
            CGSize cg = new CGSize(radius, radius);
            maskLayer.Path = UIBezierPath.FromRoundedRect(frame, UIRectCorner.BottomLeft | UIRectCorner.BottomRight, cg).CGPath;
            return maskLayer;
        }

        public static CAShapeLayer BothTopRadius(CGRect frame, float radius = 15)
        {
            frame = new CGRect(0, 0, frame.Width, frame.Height);
            CAShapeLayer maskLayer = new CAShapeLayer();
            CGSize cg = new CGSize(radius, radius);
            maskLayer.Path = UIBezierPath.FromRoundedRect(frame, UIRectCorner.TopLeft | UIRectCorner.TopRight, cg).CGPath;
            return maskLayer;
        }

        public static NSMutableAttributedString UnderLineAttributedString(string text, UIColor color = null)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var attrString = new NSMutableAttributedString(text);

                attrString.AddAttribute(UIStringAttributeKey.UnderlineStyle,
                                        NSNumber.FromInt32((int)NSUnderlineStyle.Single),
                                        new NSRange(0, attrString.Length));
                attrString.AddAttribute(UIStringAttributeKey.ForegroundColor, color != null ? color : UIColor.Black, new NSRange(0, attrString.Length));
                return attrString;
            }
            else
            {
                return new NSMutableAttributedString(string.Empty);
            }
        }

        public static UIImage GrayScaleImage(UIImage image)
        {
            try
            {
                CGImage cgImage = image.CGImage;
                uint inputWidth = (uint)cgImage.Width;
                int bytesPerPixel = 4;
                int bitsPerComponent = 8;

                nfloat ImageAspectRatio = image.Size.Width / image.Size.Height;
                uint targetWidth = inputWidth;
                CGSize imageSize = new CGSize(targetWidth, targetWidth / ImageAspectRatio);
                CGColorSpace colorSpace = CGColorSpace.CreateDeviceGray();
                int BytesPerRow = (int)bytesPerPixel * (int)imageSize.Width;
                var Pixels = new byte[(int)(BytesPerRow * imageSize.Height)];

                CGContext context = new CGBitmapContext(Pixels,
                                                             (nint)imageSize.Width, (nint)imageSize.Height,
                                                             bitsPerComponent, BytesPerRow, colorSpace, CGImageAlphaInfo.PremultipliedLast);
                context.DrawImage(new CGRect(0, 0, imageSize.Width, imageSize.Height), cgImage);

                CGImage newCGImage = context.AsBitmapContext().ToImage();
                UIImage processedImage = UIImage.FromImage(newCGImage);

                colorSpace.Dispose();
                context.Dispose();
                return processedImage;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return null;
        }

        internal static bool IsColorDark(this UIColor color)
        {
            color.GetRGBA(out var red, out var green, out var blue, out var alpha);
            var brightness = ((red * 299) + (green * 587) + (blue * 144)) / 1000;

            return brightness <= 0.5;
        }

        internal static bool IsColorDark(this CGColor color)
        {
            var components = color.Components;
            var brightness = ((components[0] * 299) + (components[1] * 587) + (components[2] * 144)) / 1000;

            return brightness <= 0.5;
        }
    }

    internal static class ColorExtensions
    {
        internal static bool IsEqualToColor(this UIColor self, UIColor otherColor)
        {
            nfloat r;
            nfloat g;
            nfloat b;
            nfloat a;

            self.GetRGBA(out r, out g, out b, out a);
            nfloat r2;
            nfloat g2;
            nfloat b2;
            nfloat a2;

            otherColor.GetRGBA(out r2, out g2, out b2, out a2);
            return r == r2 && g == g2 && b == b2 && a == a2;
        }
    }
}
