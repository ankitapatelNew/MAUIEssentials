using System;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using MAUIEssentials.ViewModels;
using Mopups.Pages;
using Mopups.Services;

namespace MAUIEssentials.AppCode.Controls
{
    public class PopupBase : PopupPage
	{
		BaseViewModel baseViewModel;
		bool _isFirstTime;
        public bool IsDelay = true;
        public int DelayTime = 200;

        public PopupBase()
		{
            _isFirstTime = true;

			if (DeviceInfo.Platform == DevicePlatform.iOS)
			{
				HasKeyboardOffset = true;
				HasSystemPadding = false;
			}
            HideSoftInputOnTapped = true;
			FlowDirection = (FlowDirection)(Settings.AppLanguage?.FlowDirection);
            Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(this, false);
			BackgroundColor = AppColorResources.black50Opacity.ToColor();
        }

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			baseViewModel = (BaseViewModel)BindingContext;
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			if (_isFirstTime) {
                if (IsDelay)
                {
                    await Task.Delay(DelayTime);
                }
                _isFirstTime = false;
				baseViewModel?.OnFirstTimeAppearing();
			} else {
				baseViewModel?.OnAppearing();
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			baseViewModel?.OnDisappearing();

			Task.Run(async () => {
				await Task.Delay(500);
				var navigationStack = MopupService.Instance.PopupStack;

				if (!navigationStack.Any(x => x.GetType() == GetType())) {
					baseViewModel?.OnDispose();
				}
			});
		}
        protected override bool OnBackButtonPressed()
        {
			_ = NavigationServices.ClosePopupPage();
            return true;
        }
    }
}
