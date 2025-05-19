namespace MAUIEssentials.AppCode.Controls
{
    public class CommanEntry : Entry
    {
        readonly WeakEventManager backEventManager = new WeakEventManager();

        public event EventHandler OnBackButton
        {
            add => backEventManager.AddEventHandler(value);
            remove => backEventManager.RemoveEventHandler(value);
        }

        public delegate void BackButtonPressEventHandler(object sender, EventArgs e);

        public static readonly BindableProperty IsBorderProperty =
            BindableProperty.Create(nameof(IsBorder), typeof(bool), typeof(CommanEntry), false);

        public static readonly BindableProperty BorderWidthProperty =
            BindableProperty.Create(nameof(BorderWidth), typeof(int), typeof(CommanEntry), 1);

        public static readonly BindableProperty BorderRadiusProperty =
            BindableProperty.Create(nameof(BorderRadius), typeof(int), typeof(CommanEntry), 0);

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(CommanEntry), Colors.Gray);

        public static readonly BindableProperty IsCursorVisibleProperty =
            BindableProperty.Create(nameof(IsCursorVisible), typeof(bool), typeof(CommanEntry), true);

        public bool IsBorder
        {
            get => (bool)GetValue(IsBorderProperty);
            set => SetValue(IsBorderProperty, value);
        }

        public int BorderWidth
        {
            get => (int)GetValue(BorderWidthProperty);
            set => SetValue(BorderWidthProperty, value);
        }

        public int BorderRadius
        {
            get => (int)GetValue(BorderRadiusProperty);
            set => SetValue(BorderRadiusProperty, value);
        }

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public bool IsCursorVisible
        {
            get => (bool)GetValue(IsCursorVisibleProperty);
            set => SetValue(IsCursorVisibleProperty, value);
        }

        public void OnBackButtonPress()
        {
            backEventManager?.HandleEvent(this, null, nameof(OnBackButton));
        }
    }
}