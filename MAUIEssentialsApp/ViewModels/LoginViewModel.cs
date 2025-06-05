using System;
using System.Windows.Input;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using MAUIEssentials.ViewModels;
using MAUIEssentialsApp.Pages;
using Plugin.Maui.Biometric;

namespace MAUIEssentialsApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public readonly IBiometric biometric;

        public LoginViewModel()
        {
            try
            {
                biometric = BiometricAuthenticationService.Default;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public ICommand LoginCommand => new Command(async () =>
        {
            try
            {
                if (CommonUtils.IsDoubleClick())
                {
                    return;
                }

                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await NavigationServices.OpenPopupPage(new NotificationPopup(
                        NotificationType.Error,
                        LocalizationResources.checkInternet,
                        string.Empty));
                    return;
                }

                if (await CheckValidation())
                {
                    return;
                }

                await SetFingerOrFaceprint();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        });

        private async Task<bool> CheckValidation()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await NavigationServices.OpenPopupPage(new NotificationPopup(NotificationType.Error, LocalizationResources.enterEmail, string.Empty));
                return true;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                await NavigationServices.OpenPopupPage(new NotificationPopup(NotificationType.Error, LocalizationResources.enterPassword, string.Empty));
                return true;
            }

            if (!CommonUtils.IsValidEmail(Email))
            {
                await NavigationServices.OpenPopupPage(new NotificationPopup(NotificationType.Error, LocalizationResources.validateEmail, string.Empty));
                return true;
            }

            return false;
        }

        private async Task SetFingerOrFaceprint()
        {
            try
            {
                var authOption = await DependencyService.Get<ICommonUtils>().BioMetricAuthAvailability();

                if (authOption != LocalBioMetricOption.None)
                {
                    var haveTouchId = await biometric.GetAuthenticationStatusAsync();

                    if (haveTouchId == BiometricHwStatus.Success)
                    {
                        var popup = new FingerprintVerificationPage();

                        popup.Result += async () =>
                        {
                            await OpenPage();
                        };

                        await NavigationServices.OpenPopupPage(popup);
                    }
                    else
                    {
                        await OpenPage();
                    }
                }
                else
                {
                    await OpenPage();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private async Task OpenPage()
        {
            try
            {
                await NavigationServices.OpenShellPage("MainPage", clearStack: true);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public ICommand PasswordVisibilityCommand => new Command(() => {
            IsPasswordVisible = !IsPasswordVisible;
        });

        string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EmailBorderColor));
                OnPropertyChanged(nameof(LoginButtonBgColor));
                OnPropertyChanged(nameof(LoginButtonTextColor));
            }
        }

        string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PasswordBorderColor));
                OnPropertyChanged(nameof(LoginButtonBgColor));
                OnPropertyChanged(nameof(LoginButtonTextColor));
            }
        }

        bool _isPasswordVisible;
        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(VisibilityImage));
            }
        }
        
        public string VisibilityImage => IsPasswordVisible ? "ic_visibility" : "ic_visibility_off";

        public Color EmailBorderColor => string.IsNullOrWhiteSpace(Email)
            ? AppColorResources.black.ToColor()
            : AppColorResources.blueColor.ToColor();

        public Color PasswordBorderColor => string.IsNullOrWhiteSpace(Password)
            ? AppColorResources.black.ToColor()
            : AppColorResources.blueColor.ToColor();

        public Color LoginButtonBgColor => string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || !CommonUtils.IsValidEmail(Email)
            ? AppColorResources.lightGrayColor.ToColor()
            : AppColorResources.redColor.ToColor();

        public Color LoginButtonTextColor => string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || !CommonUtils.IsValidEmail(Email)
            ? AppColorResources.blackTextColor.ToColor()
            : AppColorResources.white.ToColor();
    }
}
