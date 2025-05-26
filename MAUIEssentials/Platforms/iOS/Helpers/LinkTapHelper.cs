using CoreGraphics;
using Foundation;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using UIKit;

namespace MAUIEssentials.Platforms.iOS.Helpers
{
    internal static class LinkTapHelper
    {
        public static readonly NSString CustomLinkAttribute = new NSString("LabelLink");

        public static void HandleLinkTap(this UILabel control, HtmlLabel element)
        {
            try
            {
                void TapHandler(UITapGestureRecognizer tap)
                {
                    var detectedUrl = DetectTappedUrl(tap, (UILabel)tap.View);
                    RendererHelper.HandleUriClick(element, detectedUrl);
                }

                var tapGesture = new UITapGestureRecognizer(TapHandler);
                control.AddGestureRecognizer(tapGesture);
                control.UserInteractionEnabled = true;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private static string? DetectTappedUrl(UIGestureRecognizer tap, UILabel control)
        {
            NSObject linkAttributeValue = new NSObject();
            try
            {
                CGRect bounds = control.Bounds;
                NSAttributedString attributedText = control.AttributedText;

                // Setup text container
                var textContainer = new NSTextContainer(bounds.Size)
                {
                    LineFragmentPadding = 0,
                    LineBreakMode = control.LineBreakMode,
                    MaximumNumberOfLines = (nuint)control.Lines
                };

                // Setup layout manager
                var layoutManager = new NSLayoutManager();
                layoutManager.AddTextContainer(textContainer);

                // Setup text storage
                var textStorage = new NSTextStorage();
                textStorage.SetString(attributedText);

                // Add font attribute to the text storage
                var fontAttributeName = new NSString("NSFont");
                var textRange = new NSRange(0, control.AttributedText.Length);
                textStorage.AddAttribute(fontAttributeName, control.Font, textRange);
                textStorage.AddLayoutManager(layoutManager);

                // Get the bounding box for the text
                //CGRect textBoundingBox = layoutManager.GetUsedRectForTextContainer(textContainer);
                CGRect textBoundingBox = layoutManager.GetUsedRect(textContainer);

                // Calculate alignment offset
                nfloat alignmentOffset = GetAlignOffset(control.TextAlignment);
                nfloat xOffset = (bounds.Size.Width - textBoundingBox.Size.Width) * alignmentOffset - textBoundingBox.Location.X;
                nfloat yOffset = (bounds.Size.Height - textBoundingBox.Size.Height) * alignmentOffset - textBoundingBox.Location.Y;

                // Find the tapped character
                CGPoint locationOfTouchInLabel = tap.LocationInView(control);
                var locationOfTouchInTextContainer = new CGPoint(locationOfTouchInLabel.X - xOffset, locationOfTouchInLabel.Y - yOffset);
                var characterIndex = (nint)layoutManager.GetCharacterIndex(locationOfTouchInTextContainer, textContainer);

                // Check if the character index is within the text range
                if (characterIndex >= attributedText.Length)
                {
                    return null;
                }

                // Try to get the URL attribute
                linkAttributeValue = attributedText.GetAttribute(CustomLinkAttribute, characterIndex, out NSRange range);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return linkAttributeValue is NSUrl url ? url.AbsoluteString : null;
        }

        private static nfloat GetAlignOffset(UITextAlignment textAlignment)
        {
            try
            {
                switch (textAlignment)
                {
                    case UITextAlignment.Center:
                        return 0.5f;
                    case UITextAlignment.Right:
                        return 1f;
                    default:
                        return 0f;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return 0f;
        }
    }
}