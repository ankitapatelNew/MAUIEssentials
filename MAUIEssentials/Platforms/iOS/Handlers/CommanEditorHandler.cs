using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using UIKit;

namespace MAUIEssentials.Platforms.iOS.Handlers
{
    public partial class CommanEditorHandler : EditorHandler
    {
        protected override MauiTextView CreatePlatformView()
        {
            var platformView = base.CreatePlatformView();

            try
            {
                var editor = VirtualView as CommanEditor;
                if (editor != null)
                {
                    if (editor.IsBorder)
                    {
                        platformView.Layer.BorderWidth = 1f;
                        platformView.Layer.BorderColor = UIColor.FromRGB(230, 230, 230).CGColor;
                    }
                    SetBorder(editor);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }

            return platformView;
        }

        protected override void ConnectHandler(MauiTextView platformView)
        {
            try
            {
                base.ConnectHandler(platformView);

                var editor = VirtualView as CommanEditor;
                if (editor != null && editor.AttributedText)
                {
                    SetText(editor);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected override void DisconnectHandler(MauiTextView platformView)
        {
            try
            {
                base.DisconnectHandler(platformView);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void MapAttributedText(CommanEditorHandler handler, CommanEditor editor)
        {
            try
            {
                if (editor.AttributedText)
                {
                    handler.SetText(editor);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void MapBorderProperties(CommanEditorHandler handler, CommanEditor editor)
        {
            try
            {
                handler.SetBorder(editor);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void SetText(CommanEditor editor)
        {
            try
            {
                var attr = new NSAttributedStringDocumentAttributes();
                var nsError = new NSError();
                attr.DocumentType = NSDocumentType.HTML;

                string head = "<head><style>*{font-family:'" + editor.FontFamily + "';!important;}</style></head>";
                var html = ("<html>" + head + "<body>" + editor.Text + "</body></html>").Encode();

                var myHtmlData = NSData.FromString(html, NSStringEncoding.Unicode);
                var mutable = new NSAttributedString(myHtmlData, attr, ref nsError);

                if (PlatformView != null)
                {
                    PlatformView.AttributedText = mutable;
                    PlatformView.TextColor = editor.TextColor.ToPlatform();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void SetBorder(CommanEditor editor)
        {
            try
            {
                if (editor.IsBorder && PlatformView != null)
                {
                    PlatformView.Layer.CornerRadius = editor.BorderRadius;
                    PlatformView.Layer.BorderWidth = editor.BorderWidth;
                    PlatformView.Layer.BorderColor = editor.BorderColor.ToCGColor();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}
