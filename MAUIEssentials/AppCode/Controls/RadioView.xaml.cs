using System.Windows.Input;

namespace MAUIEssentials.AppCode.Controls
{
	public partial class RadioView : Grid
	{
		TapGestureRecognizer _gestureRecognizer;
        readonly AsyncAwaitBestPractices.WeakEventManager clickEventManager = new AsyncAwaitBestPractices.WeakEventManager();

        public event EventHandler Clicked
        {
            add => clickEventManager.AddEventHandler(value);
            remove => clickEventManager.RemoveEventHandler(value);
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(RadioView), string.Empty);

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(RadioView), Colors.Black);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(RadioView), 14d);

        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(RadioView), string.Empty);

        public static readonly BindableProperty FontAttributesProperty =
            BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(RadioView), FontAttributes.None);

        public static readonly BindableProperty IsSelectedFullColorProperty =
            BindableProperty.Create(nameof(IsSelectedFullColor), typeof(bool), typeof(RadioView), true,
                propertyChanged: (bindable, oldValue, newValue) => (bindable as RadioView)?.SetRadioView());

        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(RadioView), false,
                propertyChanged: (bindable, oldValue, newValue) => (bindable as RadioView)?.SetRadioView());

        public static readonly BindableProperty SelectedColorProperty =
            BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(RadioView), Colors.Black,
                propertyChanged: (bindable, oldValue, newValue) => (bindable as RadioView)?.SetRadioView());

        public static readonly BindableProperty UnselectedColorProperty =
            BindableProperty.Create(nameof(UnselectedColor), typeof(Color), typeof(RadioView), Colors.Black,
                propertyChanged: (bindable, oldValue, newValue) => (bindable as RadioView)?.SetRadioView());

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(RadioView));

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(RadioView));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public FontAttributes FontAttributes
        {
            get => (FontAttributes)GetValue(FontAttributesProperty);
            set => SetValue(FontAttributesProperty, value);
        }

        public bool IsSelectedFullColor
        {
            get => (bool)GetValue(IsSelectedFullColorProperty);
            set => SetValue(IsSelectedFullColorProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public Color UnselectedColor
        {
            get => (Color)GetValue(UnselectedColorProperty);
            set => SetValue(UnselectedColorProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public RadioView()
        {
            InitializeComponent();
            SetRadioView();

            SetClickView();
        }

        private void SetClickView()
        {
            if (_gestureRecognizer == null)
            {
                _gestureRecognizer = new TapGestureRecognizer()
                {
                    Command = new Command(OnClick)
                };
            }

            GestureRecognizers.Clear();
            GestureRecognizers.Add(_gestureRecognizer);
        }

        protected virtual void OnClick()
        {
            clickEventManager?.RaiseEvent(this, EventArgs.Empty, nameof(Clicked));
            Command?.Execute(CommandParameter);
        }

        private void SetRadioView()
        {
            frame.Stroke = IsSelected ? IsSelectedFullColor ? SelectedColor : UnselectedColor : UnselectedColor;
            boxView.Color = IsSelected ? SelectedColor : Colors.Transparent;
        }
	}
}