using System.ComponentModel;
using CoreGraphics;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using UIKit;

namespace MAUIEssentials.Platforms.iOS.Renderers
{
    public class DashedLineRenderer : ViewRenderer<DashedLine, UIView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DashedLine> e)
        {
            try
            {
                base.OnElementChanged(e);

                if (e.NewElement != null)
                {
                    // Use a custom UIView that handles drawing
                    SetNativeControl(new DashedLineUIView(Element));
                    BackgroundColor = UIColor.Clear;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                base.OnElementPropertyChanged(sender, e);

                if (Control is DashedLineUIView dashedLineView)
                {
                    dashedLineView.SetNeedsDisplay(); // Force redraw when properties change
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }

    // Custom UIView that handles the dashed line drawing
    public class DashedLineUIView : UIView
    {
        private readonly DashedLine? _dashedLine;

        public DashedLineUIView(DashedLine? dashedLine)
        {
            try
            {
                _dashedLine = dashedLine;
                BackgroundColor = UIColor.Clear;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public override void Draw(CGRect rect)
        {
            try
            {
                base.Draw(rect);

                using (CGContext context = UIGraphics.GetCurrentContext())
                {
                    context.SetStrokeColor(_dashedLine.DashColor.ToCGColor());
                    context.SetLineWidth((nfloat)(_dashedLine.Orientation == StackOrientation.Horizontal
                        ? _dashedLine.HeightRequest
                        : _dashedLine.WidthRequest));

                    // Set dash pattern
                    var dashPattern = new nfloat[] { _dashedLine.DashWidth, _dashedLine.DashGap };
                    context.SetLineDash(0, dashPattern, dashPattern.Length);

                    // Draw the line
                    if (_dashedLine.Orientation == StackOrientation.Horizontal)
                    {
                        var y = Bounds.Height / 2;
                        context.MoveTo(0, y);
                        context.AddLineToPoint(Bounds.Width, y);
                    }
                    else
                    {
                        var x = Bounds.Width / 2;
                        context.MoveTo(x, 0);
                        context.AddLineToPoint(x, Bounds.Height);
                    }

                    context.StrokePath();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}
