using System.ComponentModel;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using MAUIEssentials.ViewModels;
using Mopups.Services;
using NavigationPage = Microsoft.Maui.Controls.NavigationPage;

namespace MAUIEssentials.Pages
{
    public class BaseContentPage : ContentPage
    {
        BaseViewModel baseViewModel;
        protected bool _isFirstTime;
        public bool IsDelay = true;
        public int DelayTime = 500;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<PropertyChangedEventArgs> OnElementPropertyChanged;

        public static readonly BindableProperty SafeAreaTopColorProperty =
            BindableProperty.Create(nameof(SafeAreaTopColor), typeof(Color), typeof(BaseContentPage), Colors.White,
                propertyChanged: (bindable, oldValue, newValue) => ((BaseContentPage)bindable).UpdateSafeAreaColor(nameof(SafeAreaTopColor)));

        public static readonly BindableProperty SafeAreaBottomColorProperty =
            BindableProperty.Create(nameof(SafeAreaBottomColor), typeof(Color), typeof(BaseContentPage), Colors.White,
                propertyChanged: (bindable, oldValue, newValue) => ((BaseContentPage)bindable).UpdateSafeAreaColor(nameof(SafeAreaBottomColor)));

        public static readonly BindableProperty OrientationProperty =
            BindableProperty.Create(nameof(Orientation), typeof(GradientOrientation), typeof(BaseContentPage), GradientOrientation.Horizontal,
                propertyChanged: (bindable, oldValue, newValue) => ((BaseContentPage)bindable).UpdateSafeAreaColor(nameof(Orientation)));

        public static readonly BindableProperty StartColorProperty =
            BindableProperty.Create(nameof(StartColor), typeof(Color), typeof(BaseContentPage), Colors.Transparent,
                propertyChanged: (bindable, oldValue, newValue) => ((BaseContentPage)bindable).UpdateSafeAreaColor(nameof(StartColor)));

        public static readonly BindableProperty EndColorProperty =
            BindableProperty.Create(nameof(EndColor), typeof(Color), typeof(BaseContentPage), Colors.Transparent,
                propertyChanged: (bindable, oldValue, newValue) => ((BaseContentPage)bindable).UpdateSafeAreaColor(nameof(EndColor)));

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

        public BaseContentPage()
        {
            Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(this, true);
            if (Settings.AppType == AppTypeEnum.Customer)
            {
                DelayTime = 300;
            }
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                this.Behaviors.Add(new StatusBarBehavior
                {
                    StatusBarColor = AppColorResources.headerbluecolor.ToColor(),
                    StatusBarStyle = StatusBarStyle.LightContent
                });
            }
            //Shell.SetTabBarIsVisible(this, false);
            Shell.SetNavBarIsVisible(this, false);
            // FlowDirection = (FlowDirection)(Settings.AppLanguage?.FlowDirection);
            BackgroundColor = AppColorResources.white.ToColor();

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            _isFirstTime = true;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            baseViewModel = (BaseViewModel)BindingContext;
            if (baseViewModel != null)
            {
                baseViewModel.PageInstance = this;
            }
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            if (baseViewModel != null && this.Handler != null)
            {
                App.mauiContext = this.Handler.MauiContext;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

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

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            baseViewModel?.OnDisappearing();

            Task.Run(async () =>
            {
                await Task.Delay(500);
                var navigationStack = await NavigationServices.GetAllPagesInStack();

                if (!navigationStack.Any(x => x != null && x.GetType() == GetType()))
                {
                    baseViewModel?.OnDispose();
                    OnDispose();
                }
            }).ConfigureAwait(false);
        }

        private void UpdateSafeAreaColor(string propertyName)
        {
            OnElementPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnDispose() { }

        protected override bool OnBackButtonPressed()
        {
            if (MopupService.Instance.PopupStack.Count > 0)
            {
               _= NavigationServices.ClosePopupPage();
                return true; // Back button handled
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }
    }
}
