namespace MAUIEssentials.AppCode.Controls
{
    public class RendererHelper
    {
        private readonly Label _label;
        private readonly string _runtimePlatform;
        private readonly bool _isRtl;
        private readonly string _text;
        private readonly IList<KeyValuePair<string, string>> _styles;

        private static readonly string[] SupportedProperties =
        {
            Label.TextProperty.PropertyName,
            Label.FontAttributesProperty.PropertyName,
            Label.FontFamilyProperty.PropertyName,
            Label.FontSizeProperty.PropertyName,
            Label.HorizontalTextAlignmentProperty.PropertyName,
            Label.TextColorProperty.PropertyName,
            HtmlLabel.LinkColorProperty.PropertyName
        };

        public RendererHelper(Label label, string text, string runtimePlatform, bool isRtl)
        {
            _label = label ?? throw new ArgumentNullException(nameof(label));
            _runtimePlatform = runtimePlatform;
            _isRtl = isRtl;
            _text = text?.Trim();
            _styles = new List<KeyValuePair<string, string>>();
        }

        public void AddFontAttributesStyle(FontAttributes fontAttributes)
        {
            if (fontAttributes == FontAttributes.Bold)
            {
                AddStyle("font-weight", "bold");
            }
            else if (fontAttributes == FontAttributes.Italic)
            {
                AddStyle("font-style", "italic");
            }
        }

        public void AddFontFamilyStyle(string fontFamily)
        {
            string defaultFont = _runtimePlatform switch
            {
                "iOS" => "-apple-system",
                "Android" => "Roboto",
                _ => "system-ui"
            };

            var fontFamilyValue = string.IsNullOrWhiteSpace(fontFamily) ? defaultFont : fontFamily;
            AddStyle("font-family", $"'{fontFamilyValue}'");
        }

        public void AddFontSizeStyle(double fontSize)
        {
            AddStyle("font-size", $"{fontSize}px");
        }

        public void AddTextColorStyle(Color color)
        {
            if (color == null)
            {
                return;
            }

            var red = (int)(color.Red * 255);
            var green = (int)(color.Green * 255);
            var blue = (int)(color.Blue * 255);
            var alpha = color.Alpha;
            var hex = $"#{red:X2}{green:X2}{blue:X2}";
            var rgba = $"rgba({red},{green},{blue},{alpha})";
            AddStyle("color", hex);
            AddStyle("color", rgba);
        }

        public void AddHorizontalTextAlignStyle(TextAlignment textAlignment)
        {
            string alignment = textAlignment switch
            {
                TextAlignment.Start => _isRtl ? "right" : "left",
                TextAlignment.Center => "center",
                TextAlignment.End => _isRtl ? "left" : "right",
                _ => "left"
            };

            AddStyle("text-align", alignment);
        }

        public void AddVerticalTextAlignStyle(TextAlignment textAlignment)
        {
            string alignment = textAlignment switch
            {
                TextAlignment.Start => "top",
                TextAlignment.Center => "middle",
                TextAlignment.End => "bottom",
                _ => "top"
            };

            AddStyle("vertical-align", alignment);
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(_text))
            {
                return null;
            }

            // Add null checks for _label
            if (_label == null)
            {
                return WebUtility.HtmlEncode(_text);
            }

            AddFontAttributesStyle(_label.FontAttributes);
            AddFontFamilyStyle(_label.FontFamily);
            AddTextColorStyle(_label.TextColor);
            AddHorizontalTextAlignStyle(_label.HorizontalTextAlignment);
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                AddVerticalTextAlignStyle(_label.VerticalTextAlignment);
            }
            AddFontSizeStyle(_label.FontSize);

            var style = GetStyle();
            return $"<div style=\"{style}\" dir=\"auto\">{_text}</div>";
        }

        public string GetStyle()
        {
            var builder = new StringBuilder();

            foreach (var style in _styles)
            {
                builder.Append($"{style.Key}:{style.Value};");
            }

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                var css = builder.ToString();
                if (_styles.Any())
                {
                    css = css.Substring(0, css.Length - 1);
                }

                return css;
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                return builder.ToString();
            }
            return "";
        }

        public static bool RequireProcess(string propertyName) => SupportedProperties.Contains(propertyName);

        public static bool HandleUriClick(HtmlLabel label, string url)
        {
            if (url == null || !Uri.IsWellFormedUriString(WebUtility.UrlEncode(url), UriKind.RelativeOrAbsolute))
            {
                if (label.IsOverrideLink)
                {
                    label.OnOverrideLinkClick();
                    return true;
                }

                return false;
            }

            var args = new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = url }, url);
            label.SendNavigating(args);

            if (args.Cancel)
            {
                // Uri is handled because it is cancled;
                return true;
            }

            if (!url.IsUri() && label.IsOverrideLink)
            {
                label.OnOverrideLinkClick();
                return true;
            }

            var uri = new Uri(url);
            bool result = false;

            if (uri.IsHttp())
            {
                uri.LaunchBrowser(label.BrowserLaunchOptions);
                result = true;
            }
            else if (uri.IsEmail())
            {
                result = uri.LaunchEmail();
            }
            else if (uri.IsTel())
            {
                result = uri.LaunchTel();
            }
            else if (uri.IsSms())
            {
                result = uri.LaunchSms();
            }
            else if (uri.IsGeo())
            {
                result = uri.LaunchMaps();
            }
            else
            {
                result = Launcher.TryOpenAsync(uri).Result;
            }

            label.SendNavigated(args);
            return result;
        }

        private void AddStyle(string selector, string value)
        {
            _styles.Add(new KeyValuePair<string, string>(selector, value));
        }
    }
}