using System.ComponentModel;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.ViewModels;

namespace MAUIEssentials.Pages
{
    public class BaseContentView : ContentView
    {
        BaseViewModel baseViewModel;
        bool _isFirstTime;
        public bool IsDelay = true;
        public int DelayTime = 100;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<PropertyChangedEventArgs> OnElementPropertyChanged;

        public static readonly BindableProperty SafeAreaTopColorProperty =
            BindableProperty.Create(nameof(SafeAreaTopColor), typeof(Color), typeof(BaseContentView), Colors.White,
                propertyChanged: (bindable, oldValue, newValue) => ((BaseContentView)bindable).UpdateSafeAreaColor(nameof(SafeAreaTopColor)));

        public static readonly BindableProperty SafeAreaBottomColorProperty =
            BindableProperty.Create(nameof(SafeAreaBottomColor), typeof(Color), typeof(BaseContentView), Colors.White,
                propertyChanged: (bindable, oldValue, newValue) => ((BaseContentView)bindable).UpdateSafeAreaColor(nameof(SafeAreaBottomColor)));

        public static readonly BindableProperty OrientationProperty =
            BindableProperty.Create(nameof(Orientation), typeof(GradientOrientation), typeof(BaseContentView), GradientOrientation.Horizontal,
                propertyChanged: (bindable, oldValue, newValue) => ((BaseContentView)bindable).UpdateSafeAreaColor(nameof(Orientation)));

        public static readonly BindableProperty StartColorProperty =
            BindableProperty.Create(nameof(StartColor), typeof(Color), typeof(BaseContentView), Colors.Transparent,
                propertyChanged: (bindable, oldValue, newValue) => ((BaseContentView)bindable).UpdateSafeAreaColor(nameof(StartColor)));

        public static readonly BindableProperty EndColorProperty =
            BindableProperty.Create(nameof(EndColor), typeof(Color), typeof(BaseContentView), Colors.Transparent,
                propertyChanged: (bindable, oldValue, newValue) => ((BaseContentView)bindable).UpdateSafeAreaColor(nameof(EndColor)));

        public Color SafeAreaTopColor
        {
            get => (Color)GetValue(SafeAreaTopColorProperty);
            set => SetValue(SafeAreaTopColorProperty, value);
        }

        public Color SafeAreaBottomColor
        {
            get => (Color)GetValue(SafeAreaBottomColorProperty);
            set => SetValue(SafeAreaBottomColorProperty, value);
        }

        public GradientOrientation Orientation
        {
            get => (GradientOrientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public Color StartColor
        {
            get => (Color)GetValue(StartColorProperty);
            set => SetValue(StartColorProperty, value);
        }

        public Color EndColor
        {
            get => (Color)GetValue(EndColorProperty);
            set => SetValue(EndColorProperty, value);
        }

        public BaseContentView()
        {
            Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(this, true);

            // FlowDirection = (FlowDirection)(Settings.AppLanguage?.FlowDirection);
            BackgroundColor = Colors.White;

            NavigationPage.SetHasNavigationBar(this, false);

            _isFirstTime = true;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            baseViewModel = (BaseViewModel)BindingContext;
            _ = OnAppearing();
        }
        public async Task OnAppearing()
        {

            if (_isFirstTime)
            {
                _isFirstTime = false;
                if (IsDelay)
                {
                    await Task.Delay(DelayTime);
                }
                baseViewModel?.OnFirstTimeAppearing();
            }
            else
            {
                baseViewModel?.OnAppearing();
            }
        }

        public void OnDisappearing()
        {
            baseViewModel?.OnDisappearing();

            //Task.Run(async () =>
            //{
            //    await Task.Delay(500);
            //    var navigationStack = await NavigationServices.GetAllPagesInStack();

            //    if (!navigationStack.Any(x => x != null && x.GetType() == GetType()))
            //    {
            //        baseViewModel?.OnDispose();
            //        OnDispose();
            //    }
            //}).ConfigureAwait(false);
        }

        private void UpdateSafeAreaColor(string propertyName)
        {
            OnElementPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnDispose() { }
    }
}
