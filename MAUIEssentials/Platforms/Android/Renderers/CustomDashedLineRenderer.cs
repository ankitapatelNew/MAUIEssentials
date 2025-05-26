using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Microsoft.Maui.Controls.Platform;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using AView = Android.Views.View;
using Paint = Android.Graphics.Paint;
using Path = Android.Graphics.Path;
using Microsoft.Maui.Controls.Compatibility;
using MAUIEssentials.Platforms.Android.Renderers;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

[assembly: ExportRenderer(typeof(DashedLine), typeof(CustomDashedLineRenderer))]
namespace MAUIEssentials.Platforms.Android.Renderers
{
    public class CustomDashedLineRenderer : Microsoft.Maui.Controls.Handlers.Compatibility.ViewRenderer<DashedLine, AView>
    {
        private Paint _paint;
        private Path _path;
        private PathEffect _pathEffect;

        public CustomDashedLineRenderer(Context context) : base(context)
        {
            try
            {
                SetWillNotDraw(false); // Important for custom drawing
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<DashedLine> e)
        {
            try
            {
                base.OnElementChanged(e);

                if (Control == null && Element != null)
                {
                    // Create a native view that we can draw on
                    SetNativeControl(new AView(Context));
                }

                if (e.NewElement != null)
                {
                    InitializePaint();
                    Invalidate(); // Force redraw
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

                if (e.PropertyName == DashedLine.DashColorProperty.PropertyName ||
                    e.PropertyName == DashedLine.DashWidthProperty.PropertyName ||
                    e.PropertyName == DashedLine.DashGapProperty.PropertyName ||
                    e.PropertyName == DashedLine.OrientationProperty.PropertyName ||
                    e.PropertyName == VisualElement.HeightRequestProperty.PropertyName ||
                    e.PropertyName == VisualElement.WidthRequestProperty.PropertyName)
                {
                    InitializePaint();
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void InitializePaint()
        {
            try
            {
                if (Element == null)
                    return;

                _paint = new Paint
                {
                    Color = Element.DashColor.ToAndroid(),
                    StrokeWidth = (float)(Element.Orientation == StackOrientation.Horizontal ? Element.Height : Element.Width),
                    AntiAlias = true,
                    StrokeCap = Paint.Cap.Butt
                };

                _pathEffect = new DashPathEffect(
                    new float[] { Element.DashWidth, Element.DashGap }, 0);

                _paint.SetPathEffect(_pathEffect);
                _paint.SetStyle(Paint.Style.Stroke);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            try
            {
                base.OnDraw(canvas);

                if (Element == null || _paint == null)
                    return;

                // Clear any previous path
                _path = new Path();

                if (Element.Orientation == StackOrientation.Horizontal)
                {
                    // Center the line vertically
                    var y = Height / 2f;
                    _path.MoveTo(0, y);
                    _path.LineTo(Width, y);
                }
                else
                {
                    // Center the line horizontally
                    var x = Width / 2f;
                    _path.MoveTo(x, 0);
                    _path.LineTo(x, Height);
                }

                canvas.DrawPath(_path, _paint);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    _paint?.Dispose();
                    _path?.Dispose();
                    _pathEffect?.Dispose();
                }

                base.Dispose(disposing);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}
