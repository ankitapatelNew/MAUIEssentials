namespace MAUIEssentials.Platforms.iOS.Handlers
{
    public class HtmlLabelView : UILabel
    {
        public event EventHandler TraitCollectionDidChangeEvent;

        public HtmlLabelView()
        {
            // Initialize the label
            Lines = 0;
            LineBreakMode = UILineBreakMode.WordWrap;
            UserInteractionEnabled = true;
            BackgroundColor = UIColor.Clear;
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);

            // Notify subscribers that the trait collection has changed
            TraitCollectionDidChangeEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    public class CommanHtmlLabelHandler : ViewHandler<HtmlLabel, HtmlLabelView>
    {
        private NSMutableAttributedString mutableHtmlString;
        private bool _isDisposed;

        public static readonly PropertyMapper<HtmlLabel, CommanHtmlLabelHandler> PropertyMapper =
            new PropertyMapper<HtmlLabel, CommanHtmlLabelHandler>(ViewHandler.ViewMapper)
            {
                [nameof(HtmlLabel.Text)] = MapText,
                [nameof(HtmlLabel.LinkColor)] = MapLinkColor,
                [nameof(HtmlLabel.LineHeight)] = MapLineHeight,
                [nameof(HtmlLabel.UnderlineText)] = MapUnderlineText,
                [nameof(HtmlLabel.TextColor)] = MapTextColor,
                [nameof(HtmlLabel.FontAttributes)] = MapFontAttributes,
                [nameof(HtmlLabel.FontFamily)] = MapFontFamily,
                [nameof(HtmlLabel.FontSize)] = MapFontSize,
            };

        public CommanHtmlLabelHandler() : base(PropertyMapper)
        {
        }

        protected override HtmlLabelView CreatePlatformView()
        {
            return new HtmlLabelView();
        }

        protected override void ConnectHandler(HtmlLabelView platformView)
        {
            base.ConnectHandler(platformView);

            // Subscribe to trait collection changes
            platformView.TraitCollectionDidChangeEvent += OnTraitCollectionDidChange;

            DispatchQueue.MainQueue.DispatchAsync(ProcessText);
        }

        protected override void DisconnectHandler(HtmlLabelView platformView)
        {
            if (_isDisposed)
                return;

            // Unsubscribe from trait collection changes
            platformView.TraitCollectionDidChangeEvent -= OnTraitCollectionDidChange;
            base.DisconnectHandler(platformView);

            mutableHtmlString?.Dispose();
            mutableHtmlString = null;
            _isDisposed = true;
        }

        private void OnTraitCollectionDidChange(object? sender, EventArgs e)
        {
            if (!_isDisposed)
            {
                DispatchQueue.MainQueue.DispatchAsync(ProcessText);
            }
        }

        private void ProcessText()
        {
            if (_isDisposed || VirtualView == null || PlatformView == null)
            {
                return;
            }

            try
            {
                if (UIApplication.SharedApplication.ApplicationState == UIApplicationState.Background)
                    return;

                ProcessTextImpl();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void ProcessTextImpl()
        {
            try
            {
                if (string.IsNullOrEmpty(VirtualView?.Text))
                {
                    PlatformView.Text = string.Empty;
                    return;
                }

                var linkColor = VirtualView.LinkColor;
                if (linkColor != null && !linkColor.IsDefault())
                {
                    PlatformView.TintColor = linkColor.ToPlatform();
                }

                var styledHtml = new RendererHelper(VirtualView, VirtualView.Text, DeviceInfo.Platform.ToString(), false).ToString();

                if (!string.IsNullOrEmpty(styledHtml))
                {
                    SetText(styledHtml);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"ERROR: ", ex.Message);
            }
        }

        private void SetText(string html)
        {
            if (_isDisposed ||  PlatformView == null || string.IsNullOrEmpty(html))
                return;

            var stringType = new NSAttributedStringDocumentAttributes
            {
                DocumentType = NSDocumentType.HTML,
                StringEncoding = NSStringEncoding.UTF8,
            };

            var nsError = new NSError();
            var htmlData = NSData.FromString(html, NSStringEncoding.UTF8);

            if (htmlData == null)
            {
                Debug.WriteLine("Failed to create NSData from HTML string");
                return;
            }

            using (var htmlString = new NSAttributedString(htmlData, stringType, ref nsError))
            {
                if (htmlString == null)
                {
                    Debug.WriteLine($"Failed to create NSAttributedString: {nsError?.LocalizedDescription}");
                    return;
                }

                mutableHtmlString?.Dispose();
                mutableHtmlString = RemoveTrailingNewLines(htmlString);

                // Apply all styles
                ApplyBaseStyles();
                SetLinksStyles();
                SetLineHeight();

                PlatformView.AttributedText = mutableHtmlString;
            }

            if (!VirtualView.GestureRecognizers.Any())
            {
                PlatformView.HandleLinkTap(VirtualView);
            }
        }

        private void ApplyBaseStyles()
        {
            if (mutableHtmlString == null || mutableHtmlString.Length == 0)
                return;

            var range = new NSRange(0, mutableHtmlString.Length);
            var font = GetFontWithAttributes();
            var textColor = VirtualView?.TextColor?.ToPlatform() ?? UIColor.Label; 

            mutableHtmlString.AddAttributes(new UIStringAttributes
            {
                Font = font,
                ForegroundColor = textColor
            }, range);
        }

        private UIFont GetFontWithAttributes()
        {
            var fontSize = (nfloat)(VirtualView?.FontSize ?? 14);
            var font = UIFont.SystemFontOfSize(fontSize);

            // Handle font family if specified
            if (!string.IsNullOrEmpty(VirtualView?.FontFamily))
            {
                try
                {
                    var customFont = UIFont.FromName(VirtualView.FontFamily, fontSize);
                    if (customFont != null)
                    {
                        font = customFont;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(@"ERROR: ", ex.Message);
                }
            }

            // Apply font attributes
            if (VirtualView?.FontAttributes != FontAttributes.None)
            {
                var traits = UIFontDescriptorSymbolicTraits.ClassUnknown;

                if (VirtualView.FontAttributes.HasFlag(FontAttributes.Bold))
                    traits |= UIFontDescriptorSymbolicTraits.Bold;
                if (VirtualView.FontAttributes.HasFlag(FontAttributes.Italic))
                    traits |= UIFontDescriptorSymbolicTraits.Italic;

                var descriptor = font.FontDescriptor.CreateWithTraits(traits);
                font = UIFont.FromDescriptor(descriptor, fontSize);
            }

            return font;
        }

        private void SetLinksStyles()
        {
            if (mutableHtmlString == null || mutableHtmlString.Length == 0)
                return;

            using var linkAttributeName = new NSString("NSLink");
            var linkColor = VirtualView?.LinkColor?.ToPlatform() ?? UIColor.SystemBlue;
            var underlineStyle = VirtualView?.UnderlineText == true ? NSUnderlineStyle.Single : NSUnderlineStyle.None;

            var linkAttributes = new UIStringAttributes
            {
                ForegroundColor = linkColor,
                UnderlineStyle = underlineStyle,
            };

            mutableHtmlString.EnumerateAttribute(
                linkAttributeName,
                new NSRange(0, mutableHtmlString.Length),
                NSAttributedStringEnumeration.LongestEffectiveRangeNotRequired,
                (NSObject value, NSRange range, ref bool stop) =>
                {
                    if (value is NSUrl url)
                    {
                        mutableHtmlString.AddAttribute(LinkTapHelper.CustomLinkAttribute, url, range);
                        mutableHtmlString.RemoveAttribute("NSLink", range);
                        mutableHtmlString.AddAttributes(linkAttributes, range);
                    }
                }
            );
        }

        private void SetLineHeight()
        {
            if (mutableHtmlString == null || mutableHtmlString.Length == 0 || VirtualView == null)
                return;

            if (VirtualView.LineHeight < 0)
                return;

            var paragraphStyle = new NSMutableParagraphStyle
            {
                LineHeightMultiple = (nfloat)VirtualView.LineHeight,
                LineBreakMode = UILineBreakMode.WordWrap,
            };

            mutableHtmlString.AddAttribute(
                UIStringAttributeKey.ParagraphStyle,
                paragraphStyle,
                new NSRange(0, mutableHtmlString.Length));
        }

        private static NSMutableAttributedString RemoveTrailingNewLines(NSAttributedString htmlString)
        {
            if (htmlString == null || htmlString.Length == 0)
                return new NSMutableAttributedString();

            var count = 0;
            for (int i = 1; i <= htmlString.Length; i++)
            {
                if ("\n" != htmlString.Substring(htmlString.Length - i, 1).Value)
                    break;

                count++;
            }

            return count > 0
                ? new NSMutableAttributedString(htmlString.Substring(0, htmlString.Length - count))
                : new NSMutableAttributedString(htmlString);
        }

        private static void MapText(CommanHtmlLabelHandler handler, HtmlLabel label)
        {
            handler.ProcessText();
        }

        private static void MapLinkColor(CommanHtmlLabelHandler handler, HtmlLabel label)
        {
            handler.ProcessText();
        }

        private static void MapLineHeight(CommanHtmlLabelHandler handler, HtmlLabel label)
        {
            handler.ProcessText();
        }

        private static void MapUnderlineText(CommanHtmlLabelHandler handler, HtmlLabel label)
        {
            handler.ProcessText();
        }

        private static void MapTextColor(CommanHtmlLabelHandler handler, HtmlLabel label)
        {
            handler.ProcessText();
        }

        private static void MapFontAttributes(CommanHtmlLabelHandler handler, HtmlLabel label)
        {
            handler.ProcessText();
        }

        private static void MapFontFamily(CommanHtmlLabelHandler handler, HtmlLabel label)
        {
            handler.ProcessText();
        }

        private static void MapFontSize(CommanHtmlLabelHandler handler, HtmlLabel label)
        {
            handler.ProcessText();
        }
    }    
}
