using System.Text.RegularExpressions;
using Android.Graphics;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.Platforms.Android.Helpers;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using TextAlignment = Microsoft.Maui.TextAlignment;

namespace MAUIEssentials.Platforms.Android.Handlers
{
    public partial class HtmlLabelHandler : ViewHandler<HtmlLabel, TextView>
    {
        private const string _tagUlRegex = "[uU][lL]";
        private const string _tagOlRegex = "[oO][lL]";
        private const string _tagLiRegex = "[lL][iI]";

        public static IPropertyMapper<HtmlLabel, HtmlLabelHandler> PropertyMapper = new PropertyMapper<HtmlLabel, HtmlLabelHandler>(ViewHandler.ViewMapper)
        {
            [nameof(HtmlLabel.Text)] = MapText,
            [nameof(HtmlLabel.LinkColor)] = MapLinkColor,
            [nameof(HtmlLabel.TextColor)] = MapTextColor,
            [nameof(HtmlLabel.FontAttributes)] = MapFontAttributes,
            [nameof(HtmlLabel.FontFamily)] = MapFontFamily,
            [nameof(HtmlLabel.FontSize)] = MapFontSize
        };

        public HtmlLabelHandler() : base(PropertyMapper)
        {
        }

        public HtmlLabelHandler(IPropertyMapper mapper) : base(mapper)
        {
        }

        protected override TextView CreatePlatformView()
        {
            var textView = new TextView(Context);

            // Set default gravity for vertical and horizontal alignment
            textView.Gravity = GravityFlags.Center;

            // Force light text color (or your preferred color) regardless of dark mode
            if (VirtualView?.TextColor != null)
            {
                textView.SetTextColor(VirtualView.TextColor.ToPlatform());
            }
            else
            {
                // Set default color that won't change in dark mode
                textView.SetTextColor(global::Android.Graphics.Color.Black); // Or your preferred default
            }

            return textView;
        }

        protected override void ConnectHandler(TextView platformView)
        {
            base.ConnectHandler(platformView);
            ProcessText();
        }

        protected override void DisconnectHandler(TextView platformView)
        {
            platformView.Dispose();
            base.DisconnectHandler(platformView);
        }

        private static void MapText(HtmlLabelHandler handler, HtmlLabel label)
        {
            handler.ProcessText();
        }

        private static void MapLinkColor(HtmlLabelHandler handler, HtmlLabel label)
        {
            if (!label.LinkColor.IsDefault())
            {
                handler.PlatformView?.SetLinkTextColor(label.LinkColor.ToPlatform());
            }
        }

        private static void MapFontAttributes(HtmlLabelHandler handler, HtmlLabel label)
        {
            if (handler.PlatformView != null)
            {
                var typeface = handler.PlatformView.Typeface;
                var newTypeface = typeface;

                // Handle bold
                var bold = (label.FontAttributes & FontAttributes.Bold) != 0;
                var italic = (label.FontAttributes & FontAttributes.Italic) != 0;

                if (bold && italic)
                {
                    newTypeface = Typeface.Create(typeface, TypefaceStyle.BoldItalic);
                }
                else if (bold)
                {
                    newTypeface = Typeface.Create(typeface, TypefaceStyle.Bold);
                }
                else if (italic)
                {
                    newTypeface = Typeface.Create(typeface, TypefaceStyle.Italic);
                }
                else
                {
                    newTypeface = Typeface.Create(typeface, TypefaceStyle.Normal);
                }

                handler.PlatformView.Typeface = newTypeface;
                handler.ProcessText(); // Reprocess to apply to HTML content
            }
        }

        private static void MapFontFamily(HtmlLabelHandler handler, HtmlLabel label)
        {
            if (handler.PlatformView != null && !string.IsNullOrEmpty(label.FontFamily))
            {
                var typeface = Typeface.CreateFromAsset(
                    handler.MauiContext?.Context?.Assets,
                    label.FontFamily + ".ttf");

                handler.PlatformView.Typeface = typeface;
                handler.ProcessText(); // Reprocess to apply to HTML content
            }
        }

        private static void MapFontSize(HtmlLabelHandler handler, HtmlLabel label)
        {
            if (handler.PlatformView != null)
            {
                handler.PlatformView.TextSize = (float)label.FontSize;
                handler.ProcessText(); // Reprocess to apply to HTML content
            }
        }

        private static void MapTextColor(HtmlLabelHandler handler, HtmlLabel label)
        {
            if (handler.PlatformView != null)
            {
                // Explicitly set the text color, ignoring system theme
                handler.PlatformView.SetTextColor(label.TextColor.ToPlatform());
            }
        }

        private void ProcessText()
        {
            if (PlatformView == null || VirtualView == null)
            {
                return;
            }

            // Set gravity based on alignment properties
            var gravity = GravityFlags.NoGravity;

            // Handle horizontal alignment
            gravity |= VirtualView.HorizontalTextAlignment switch
            {
                TextAlignment.Start => GravityFlags.Start,
                TextAlignment.Center => GravityFlags.CenterHorizontal,
                TextAlignment.End => GravityFlags.End,
                _ => GravityFlags.Start
            };

            // Handle vertical alignment
            gravity |= VirtualView.VerticalTextAlignment switch
            {
                TextAlignment.Start => GravityFlags.Top,
                TextAlignment.Center => GravityFlags.CenterVertical,
                TextAlignment.End => GravityFlags.Bottom,
                _ => GravityFlags.CenterVertical
            };

            PlatformView.Gravity = gravity;

            if (!VirtualView.LinkColor.IsDefault())
            {
                PlatformView.SetLinkTextColor(VirtualView.LinkColor.ToPlatform());
            }

            PlatformView.SetIncludeFontPadding(false);
            var styledHtml = new RendererHelper(VirtualView, VirtualView.Text, DeviceInfo.Platform.ToString(), false).ToString();

            styledHtml = styledHtml?
                .ReplaceTag(_tagUlRegex, ListTagHandler.TagUl)?
                .ReplaceTag(_tagOlRegex, ListTagHandler.TagOl)?
                .ReplaceTag(_tagLiRegex, ListTagHandler.TagLi);

            if (styledHtml != null)
            {
                SetText(styledHtml);
            }
        }

        private void SetText(string html)
        {
            var listTagHandler = new ListTagHandler(VirtualView.AndroidListIndent);
            var imageGetter = new UrlImageParser(Context, PlatformView);

            var sequence = Html.FromHtml(
                html,
                FromHtmlOptions.ModeLegacy,
                imageGetter,
                listTagHandler);

            using (var strBuilder = new SpannableStringBuilder(sequence))
            {
                if (!VirtualView.GestureRecognizers.Any())
                {
                    PlatformView.MovementMethod = LinkMovementMethod.Instance;
                    var urls = strBuilder.GetSpans(0, sequence.Length(), Java.Lang.Class.FromType(typeof(URLSpan)))
                        .Cast<URLSpan>()
                        .ToArray();

                    foreach (var span in urls)
                    {
                        MakeLinkClickable(strBuilder, span);
                    }
                }

                using (var value = RemoveTrailingNewLines(strBuilder))
                {
                    PlatformView.SetText(value, TextView.BufferType.Spannable);
                }
            }
        }

        private static ISpanned RemoveTrailingNewLines(Java.Lang.ICharSequence text)
        {
            var builder = new SpannableStringBuilder(text);
            var count = 0;

            for (int i = 1; i <= text.Length(); i++)
            {
                if (!'\n'.Equals(text.CharAt(text.Length() - i)))
                    break;

                count++;
            }

            if (count > 0)
            {
                builder.Delete(text.Length() - count, text.Length());
            }

            return builder;
        }

        private void MakeLinkClickable(ISpannable strBuilder, URLSpan span)
        {
            var start = strBuilder.GetSpanStart(span);
            var end = strBuilder.GetSpanEnd(span);
            var flags = strBuilder.GetSpanFlags(span);
            var clickable = new HtmlLabelClickableSpan(VirtualView, span);
            strBuilder.SetSpan(clickable, start, end, flags);
            strBuilder.RemoveSpan(span);
        }

        private class HtmlLabelClickableSpan : ClickableSpan
        {
            private readonly HtmlLabel _label;
            private readonly URLSpan _span;

            public HtmlLabelClickableSpan(HtmlLabel label, URLSpan span)
            {
                _label = label;
                _span = span;
            }

            public override void UpdateDrawState(TextPaint ds)
            {
                base.UpdateDrawState(ds);
                ds.UnderlineText = _label.UnderlineText;
            }

            public override void OnClick(global::Android.Views.View widget)
            {
                RendererHelper.HandleUriClick(_label, _span.URL);
            }
        }
    }

    internal static class StringExtensions
    {
        public static string ReplaceTag(this string html, string oldTagRegex, string newTag) =>
            Regex.Replace(html, @"(<\s*\/?\s*)" + oldTagRegex + @"((\s+[\w\-\,\.\(\)\=""\:\;]*)*>)", "$1" + newTag + "$2");
    }
}
