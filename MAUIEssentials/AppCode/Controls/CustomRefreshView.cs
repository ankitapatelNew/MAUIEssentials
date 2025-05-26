using System.Runtime.CompilerServices;

namespace MAUIEssentials.AppCode.Controls
{
    public class CustomRefreshView : RefreshView
	{
		public static readonly BindableProperty HideDefaultRefreshIndicatorProperty =
			BindableProperty.Create(nameof(HideDefaultRefreshIndicator), typeof(bool), typeof(CustomRefreshView), false,
				propertyChanged: (bindable, oldValue, newValue) => (bindable as CustomRefreshView).HideRefreshView());

		public bool HideDefaultRefreshIndicator {
			get => (bool)GetValue(HideDefaultRefreshIndicatorProperty);
			set => SetValue(HideDefaultRefreshIndicatorProperty, value);
		}

		public CustomRefreshView()
		{
			HideRefreshView();
		}

		private void HideRefreshView()
		{
			if (HideDefaultRefreshIndicator) {
				RefreshColor = Colors.Transparent;
				BackgroundColor = Colors.Transparent;
			}
		}

		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (propertyName == IsRefreshingProperty.PropertyName) {
				if (IsRefreshing && HideDefaultRefreshIndicator) {
					RefreshColor = Colors.Transparent;
					BackgroundColor = Colors.Transparent;
				}
			}
		}
	}
}
