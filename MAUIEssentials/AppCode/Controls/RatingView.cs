using System;
using System.Runtime.CompilerServices;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace MAUIEssentials.AppCode.Controls
{
    public class RatingView : SKCanvasView
    {
		public float Spacing { get; set; } = 8;
		public SKColor CanvasBackgroundColor { get; set; } = SKColors.Transparent;
		public float StrokeWidth { get; set; } = 0.1f;

		private PanGestureRecognizer panGestureRecognizer = new PanGestureRecognizer();
		private double touchX;
		private double touchY;

		private float ItemWidth { get; set; }
		private float ItemHeight { get; set; }
		private float CanvasScale { get; set; }
		private SKColor SKColorOn { get; set; } = MaterialColors.Amber;
		private SKColor SKColorOff { get; set; } = MaterialColors.Grey;
		private SKColor SKOutlineOnColor { get; set; } = SKColors.Transparent;
		private SKColor SKOutlineOffColor { get; set; } = MaterialColors.Grey;

		public RatingView()
		{
			BackgroundColor = Colors.Transparent;
			PaintSurface += Handle_PaintSurface;
			EnableTouchEvents = true;
			panGestureRecognizer.PanUpdated += PanGestureRecognizer_PanUpdated;
			GestureRecognizers.Add(panGestureRecognizer);

			ScaleX = FlowDirection == FlowDirection.RightToLeft ? -1 : 1;
		}

		~RatingView()
		{
			PaintSurface -= Handle_PaintSurface;

			if (panGestureRecognizer != null) {
				panGestureRecognizer.PanUpdated -= PanGestureRecognizer_PanUpdated;
			}
			GestureRecognizers?.Clear();
		}

		#region BindableProperties

		public static readonly BindableProperty ValueProperty =
			BindableProperty.Create(nameof(Value), typeof(double), typeof(RatingView), default(double), BindingMode.TwoWay, propertyChanged: OnValueChanged);

		public static readonly BindableProperty PathProperty =
			BindableProperty.Create(nameof(Path), typeof(string), typeof(RatingView), PathConstants.CustomStar, propertyChanged: OnPropertyChanged);

		public static readonly BindableProperty CountProperty =
			BindableProperty.Create(nameof(Count), typeof(int), typeof(RatingView), 5, propertyChanged: OnPropertyChanged);

		public static readonly BindableProperty ColorOnProperty =
			BindableProperty.Create(nameof(ColorOn), typeof(Color), typeof(RatingView), MaterialColors.Amber.ToMauiColor(), BindingMode.TwoWay, propertyChanged: ColorOnChanged);

		public static readonly BindableProperty ColorOffProperty =
			BindableProperty.Create(nameof(ColorOff), typeof(Color), typeof(RatingView), MaterialColors.Grey.ToMauiColor(), BindingMode.TwoWay, propertyChanged: ColorOffChanged);

		public static readonly BindableProperty OutlineOnColorProperty =
			BindableProperty.Create(nameof(OutlineOnColor), typeof(Color), typeof(RatingView), SKColors.Transparent.ToMauiColor(), BindingMode.TwoWay, propertyChanged: OutlineOnColorChanged);

		public static readonly BindableProperty OutlineOffColorProperty =
			BindableProperty.Create(nameof(OutlineOffColor), typeof(Color), typeof(RatingView), MaterialColors.Grey.ToMauiColor(), BindingMode.TwoWay, propertyChanged: OutlineOffColorChanged);

		public static readonly BindableProperty RatingTypeProperty =
			BindableProperty.Create(nameof(RatingType), typeof(RatingType), typeof(RatingView), RatingType.Floating, propertyChanged: OnPropertyChanged);

		public double Value {
			get => (double)GetValue(ValueProperty);
			set => SetValue(ValueProperty, ClampValue(value));
		}

		public string Path {
			get => (string)GetValue(PathProperty);
			set => SetValue(PathProperty, value);
		}

		public int Count {
			get => (int)GetValue(CountProperty);
			set => SetValue(CountProperty, value);
		}

		public Color ColorOn {
			get => (Color)GetValue(ColorOnProperty);
			set => SetValue(ColorOnProperty, value);
		}

		public Color ColorOff {
			get => (Color)GetValue(ColorOffProperty);
			set => SetValue(ColorOffProperty, value);
		}

		public Color OutlineOnColor {
			get => (Color)GetValue(OutlineOnColorProperty);
			set => SetValue(OutlineOnColorProperty, value);
		}

		public Color OutlineOffColor {
			get => (Color)GetValue(OutlineOffColorProperty);
			set => SetValue(OutlineOffColorProperty, value);
		}

		public RatingType RatingType {
			get => (RatingType)GetValue(RatingTypeProperty);
			set => SetValue(RatingTypeProperty, value);
		}

		#endregion

		protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (propertyName == FlowDirectionProperty.PropertyName) {
				ScaleX = FlowDirection == FlowDirection.RightToLeft ? -1 : 1;
			}
		}

		private void Handle_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
		}

		protected override void OnTouch(SKTouchEventArgs e)
		{
			touchX = e.Location.X;
			touchY = e.Location.Y;
			SetValue(touchX, touchY);
			InvalidateSurface();
		}

		private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
		{
			if (EnableTouchEvents) {
				var point = ConvertToPixel(new Point(e.TotalX, e.TotalY));
				if (e.StatusType != GestureStatus.Completed) {
					SetValue(touchX + point.X, touchY + e.TotalY);
					InvalidateSurface();
				}
			}
		}

		private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var view = bindable as RatingView;
			view?.InvalidateSurface();
		}

		private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var view = bindable as RatingView;
			view.Value = view.ClampValue((double)newValue);
			OnPropertyChanged(bindable, oldValue, newValue);
		}

		private static void ColorOnChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var view = bindable as RatingView;
			view.SKColorOn = ((Color)newValue).ToSKColor();
			OnPropertyChanged(bindable, oldValue, newValue);
		}

		private static void ColorOffChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var view = bindable as RatingView;
			view.SKColorOff = ((Color)newValue).ToSKColor();
			OnPropertyChanged(bindable, oldValue, newValue);
		}

		private static void OutlineOffColorChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var view = bindable as RatingView;
			view.SKOutlineOffColor = ((Color)newValue).ToSKColor();
			OnPropertyChanged(bindable, oldValue, newValue);
		}

		private static void OutlineOnColorChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var view = bindable as RatingView;
			view.SKOutlineOnColor = ((Color)newValue).ToSKColor();
			OnPropertyChanged(bindable, oldValue, newValue);
		}

		SKPoint ConvertToPixel(Point pt)
		{
			return new SKPoint((float)(CanvasSize.Width * pt.X / Width),
							   (float)(CanvasSize.Height * pt.Y / Height));
		}

		public double ClampValue(double val)
		{
			if (val < 0) {
				return 0;
			} else if (val > Count) {
				return Count;
			} else {
				return val;
			}
		}

		private double CalculateValue(double x)
		{
			if (x < ItemWidth)
				return (double)x / ItemWidth;
			else if (x < ItemWidth + Spacing)
				return 1;
			else
				return 1 + CalculateValue(x - (ItemWidth + Spacing));
		}

		public void SetValue(double x, double y)
		{
			var val = CalculateValue(x);
			switch (RatingType) {
				case RatingType.Full:
					Value = ClampValue((double)Math.Ceiling(val));
					break;
				case RatingType.Half:
					Value = ClampValue((double)Math.Round(val * 2) / 2);
					break;
				case RatingType.Floating:
					Value = ClampValue(val);
					break;
			}
		}

		public void Draw(SKCanvas canvas, int width, int height)
		{
			canvas.Clear(CanvasBackgroundColor);

			var path = SKPath.ParseSvgPathData(Path);

			var itemWidth = (width - (Count - 1) * Spacing) / Count;
			var scaleX = itemWidth / path.Bounds.Width;
			scaleX = (itemWidth - scaleX * StrokeWidth) / path.Bounds.Width;

			ItemHeight = height;
			var scaleY = ItemHeight / path.Bounds.Height;
			scaleY = (ItemHeight - scaleY * StrokeWidth) / path.Bounds.Height;

			CanvasScale = Math.Min(scaleX, scaleY);
			ItemWidth = path.Bounds.Width * CanvasScale;

			canvas.Scale(CanvasScale);
			canvas.Translate(StrokeWidth / 2, StrokeWidth / 2);
			canvas.Translate(-path.Bounds.Left, 0);
			canvas.Translate(0, -path.Bounds.Top);

			using (var fillBgPaint = new SKPaint {
				Style = SKPaintStyle.Fill,
				Color = SKColorOff,
				IsAntialias = true,
			})
			using (var strokePaint = new SKPaint {
				Style = SKPaintStyle.Stroke,
				Color = SKOutlineOnColor,
				StrokeWidth = StrokeWidth,
				StrokeJoin = SKStrokeJoin.Round,
				IsAntialias = true,
			})
			using (var fillPaint = new SKPaint {
				Style = SKPaintStyle.Fill,
				Color = SKColorOn,
				IsAntialias = true,
			}) {
				for (int i = 0; i < Count; i++) {
					if (i <= Value - 1) // Full
					{
						canvas.DrawPath(path, fillPaint);
						canvas.DrawPath(path, strokePaint);
					} else if (i < Value) //Partial
					  {
						float filledPercentage = (float)(Value - Math.Truncate(Value));
						strokePaint.Color = SKOutlineOffColor;
						canvas.DrawPath(path, strokePaint);

						using (var rectPath = new SKPath()) {
							var rect = SKRect.Create(path.Bounds.Left + path.Bounds.Width * filledPercentage, path.Bounds.Top, path.Bounds.Width * (1 - filledPercentage), ItemHeight);
							rectPath.AddRect(rect);
							canvas.ClipPath(rectPath, SKClipOperation.Difference);
							canvas.DrawPath(path, fillPaint);
						}
					} else //Empty
					  {
						canvas.DrawPath(path, fillBgPaint);
						strokePaint.Color = SKOutlineOffColor;
						canvas.DrawPath(path, strokePaint);
					}

					canvas.Translate((ItemWidth + Spacing) / CanvasScale, 0);
				}
			}
		}
	}

	public static class PathConstants
	{
		public const string Star = "M9 11.3l3.71 2.7-1.42-4.36L15 7h-4.55L9 2.5 7.55 7H3l3.71 2.64L5.29 14z";
		public const string RoundStar = "M121.215 44.212l-34.899-3.3c-2.2-0.2-4.101-1.6-5-3.7l-12.5-30.3c-2-5-9.101-5-11.101 0l-12.4 30.3 c-0.8 2.1-2.8 3.5-5 3.7l-34.9 3.3c-5.2 0.5-7.3 7-3.4 10.5l26.3 23.1c1.7 1.5 2.4 3.7 1.9 5.9l-7.9 32.399 c-1.2 5.101 4.3 9.3 8.9 6.601l29.1-17.101c1.9-1.1 4.2-1.1 6.1 0l29.101 17.101c4.6 2.699 10.1-1.4 8.899-6.601l-7.8-32.399 c-0.5-2.2 0.2-4.4 1.9-5.9l26.3-23.1C128.615 51.212 126.415 44.712 121.215 44.212z";
		public const string Heart = "M12 21.35l-1.45-1.32C5.4 15.36 2 12.28 2 8.5 2 5.42 4.42 3 7.5 3c1.74 0 3.41.81 4.5 2.09C13.09 3.81 14.76 3 16.5 3 19.58 3 22 5.42 22 8.5c0 3.78-3.4 6.86-8.55 11.54L12 21.35z";
		public const string Circle = "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z";
		public const string Bar = "M6 6h36v2H6z";
		public const string BatteryAlert = "M15.67 4H14V2h-4v2H8.33C7.6 4 7 4.6 7 5.33v15.33C7 21.4 7.6 22 8.33 22h7.33c.74 0 1.34-.6 1.34-1.33V5.33C17 4.6 16.4 4 15.67 4zM13 18h-2v-2h2v2zm0-4h-2V9h2v5z";
		public const string BatteryCharging = "M15.67 4H14V2h-4v2H8.33C7.6 4 7 4.6 7 5.33v15.33C7 21.4 7.6 22 8.33 22h7.33c.74 0 1.34-.6 1.34-1.33V5.33C17 4.6 16.4 4 15.67 4zM11 20v-5.5H9L13 7v5.5h2L11 20z";
		public const string Like = "M1 21h4V9H1v12zm22-11c0-1.1-.9-2-2-2h-6.31l.95-4.57.03-.32c0-.41-.17-.79-.44-1.06L14.17 1 7.59 7.59C7.22 7.95 7 8.45 7 9v10c0 1.1.9 2 2 2h9c.83 0 1.54-.5 1.84-1.22l3.02-7.05c.09-.23.14-.47.14-.73v-1.91l-.01-.01L23 10z";
		public const string Dislike = "M15 3H6c-.83 0-1.54.5-1.84 1.22l-3.02 7.05c-.09.23-.14.47-.14.73v1.91l.01.01L1 14c0 1.1.9 2 2 2h6.31l-.95 4.57-.03.32c0 .41.17.79.44 1.06L9.83 23l6.59-6.59c.36-.36.58-.86.58-1.41V5c0-1.1-.9-2-2-2zm4 0v12h4V3h-4z";
		public const string Theaters = "M18 3v2h-2V3H8v2H6V3H4v18h2v-2h2v2h8v-2h2v2h2V3h-2zM8 17H6v-2h2v2zm0-4H6v-2h2v2zm0-4H6V7h2v2zm10 8h-2v-2h2v2zm0-4h-2v-2h2v2zm0-4h-2V7h2v2z";
		public const string Problem = "M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z";
		public const string CustomStar = "M 268.101562 19.921875 L 333.230469 174.066406 L 499.960938 188.390625 C 511.527344 189.386719 516.230469 203.816406 507.453125 211.414062 L 380.984375 320.980469 L 418.882812 483.984375 C 421.511719 495.3125 409.238281 504.21875 399.300781 498.203125 L 256.011719 411.785156 L 112.722656 498.203125 C 102.761719 504.195312 90.515625 495.285156 93.144531 483.984375 L 131.042969 320.980469 L 4.546875 211.386719 C -4.230469 203.789062 0.445312 189.363281 12.039062 188.363281 L 178.769531 174.039062 L 243.898438 19.921875 C 248.417969 9.199219 263.582031 9.199219 268.101562 19.921875 Z M 268.101562 19.921875";
	}

	public enum RatingType
	{
		Full,
		Half,
		Floating
	}

	public static class MaterialColors
	{
		public static SKColor Red => SKColor.Parse("F44336");
		public static SKColor Pink => SKColor.Parse("E91E63");
		public static SKColor Purple => SKColor.Parse("9C27B0");
		public static SKColor DeepPurple => SKColor.Parse("673AB7");
		public static SKColor Indigo => SKColor.Parse("3F51B5");
		public static SKColor Blue => SKColor.Parse("2196F3");
		public static SKColor LightBlue => SKColor.Parse("03A9F4");
		public static SKColor Cyan => SKColor.Parse("00BCD4");
		public static SKColor Teal => SKColor.Parse("009688");
		public static SKColor Green => SKColor.Parse("4CAF50");
		public static SKColor LightGreen => SKColor.Parse("8BC34A");
		public static SKColor Lime => SKColor.Parse("CDDC39");
		public static SKColor Yellow => SKColor.Parse("FFEB3B");
		public static SKColor Amber => SKColor.Parse("FFC107");
		public static SKColor Orange => SKColor.Parse("FF9800");
		public static SKColor DeepOrange => SKColor.Parse("FF5722");
		public static SKColor Brown => SKColor.Parse("795548");
		public static SKColor Grey => SKColor.Parse("9E9E9E");
		public static SKColor BlueGrey => SKColor.Parse("607D8B");
		public static SKColor Black => SKColor.Parse("000000");
		public static SKColor White => SKColor.Parse("FFFFFF");
	}
}