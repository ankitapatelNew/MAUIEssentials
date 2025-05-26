namespace MAUIEssentials.AppCode.Controls
{
    public class CustomScrollView : ScrollView
	{
		public static readonly BindableProperty DisableBounceProperty =
			BindableProperty.Create(nameof(DisableBounce), typeof(bool), typeof(CustomScrollView), true);

		public bool DisableBounce
		{
			get => (bool)GetValue(DisableBounceProperty);
			set => SetValue(DisableBounceProperty, value);
		}
	}
}
