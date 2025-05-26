namespace MAUIEssentials.AppCode.Controls
{
    public class CustomCollectionView : CollectionView
    {
        public CustomCollectionView()
        {
        }

        public static readonly BindableProperty ScrollDurationProperty =
            BindableProperty.Create(nameof(ScrollDuration), typeof(float), typeof(CustomCollectionView), 1000f);

        public float ScrollDuration
        {
            get => (float)GetValue(ScrollDurationProperty);
            set => SetValue(ScrollDurationProperty, value);
        }
    }
}
