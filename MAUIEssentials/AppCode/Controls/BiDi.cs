using System.ComponentModel;

namespace MAUIEssentials.AppCode.Controls
{
    public static class BiDi
	{
		private static readonly Thickness ZeroThickness = new Thickness();

		public static readonly BindableProperty MarginProperty =
			BindableProperty.CreateAttached("Margin", typeof(Thickness), typeof(BiDi), ZeroThickness, propertyChanged: OnMarginPropertyChanged);

		private static void OnMarginPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (bindable is View view) {
				var oldMargin = (Thickness)oldValue;
				var newMargin = (Thickness)newValue;

				UpdateMargin(view);

				if (newMargin != ZeroThickness) {
					if (oldMargin == ZeroThickness) {
						OnElementAttached(view);
					}
				} else {
					OnElementDetached(view);
				}
			}
		}

		private static void UpdateMargin(View view)
		{
			var controller = (IVisualElementController)view;
			var margin = GetMargin(view);

			if (margin != ZeroThickness) {
				if (controller.EffectiveFlowDirection == EffectiveFlowDirection.RightToLeft) {
					margin = new Thickness(margin.Right, margin.Top, margin.Left, margin.Bottom);
				}

				view.SetValue(View.MarginProperty, margin);
			} else {
				view.ClearValue(View.MarginProperty);
			}
		}

		private static void OnElementAttached(View element)
		{
			// should not be a source of a memory leak, because the publisher is the one who holds the reference
			element.PropertyChanged += OnElementPropertyChanged;
		}

		private static void OnElementDetached(View element)
		{
			element.PropertyChanged -= OnElementPropertyChanged;
		}

		private static void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender is View view) {
				if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName) {
					UpdateMargin(view);
				}
			}
		}

		public static Thickness GetMargin(BindableObject bindable)
		{
			return (Thickness)bindable.GetValue(MarginProperty);
		}

		public static void SetMargin(BindableObject bindable, Thickness value)
		{
			if (value != ZeroThickness) {
				bindable.SetValue(MarginProperty, value);
			} else {
				bindable.ClearValue(MarginProperty);
			}
		}
	}
}
