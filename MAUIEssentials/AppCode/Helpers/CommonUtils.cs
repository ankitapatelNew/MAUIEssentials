namespace MAUIEssentials.AppCode.Helpers
{
    public static class CommonUtils
    {
        static long _lastClickTime = 0;
        public const int IntervelTime = 600;

        public static string GoogleMapApiKey => "AIzaSyAiL90vZ4SJwb5I4dWFUhXAqJ7MStXz3Z0";

        public static bool IsDoubleClick()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                var clockTime = DependencyService.Get<ICommonUtils>().SystemClockERealtime();
                if (clockTime - _lastClickTime < IntervelTime)
                {
                    return true;
                }
                _lastClickTime = clockTime;
            }
            return false;
        }

        public static async Task<PermissionStatus> CheckPermissions<T>(T permission) where T : Permissions.BasePermission
        {
            var permissionStatus = await permission.CheckStatusAsync();
            bool request = false;
            var permissionName = permission.GetType().Name.ToLower();

            if (permissionStatus == PermissionStatus.Denied)
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {

                    var title = $"Permission denied";
                    var question = $"To use this feature, {permissionName} permission is required.\nPlease go into Settings and turn on {permissionName} for the app.";
                    var positive = "Settings";
                    var negative = "Maybe Later";
                    var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);

                    if (task == null)
                    {
                        return permissionStatus;
                    }

                    var result = await task;
                    if (result)
                    {
                        AppInfo.ShowSettingsUI();
                    }

                    return permissionStatus;
                }
                request = true;
            }

            if (request || permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await permission.RequestAsync();

                if (permissionStatus != PermissionStatus.Granted)
                {
                    var title = $"Permission denied";
                    var question = $"To use this feature, {permissionName} permission is required.";
                    var positive = "Settings";
                    var negative = "Maybe Later";
                    var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);

                    if (task == null)
                    {
                        return permissionStatus;
                    }

                    var result = await task;
                    if (result)
                    {
                        AppInfo.ShowSettingsUI();
                    }

                    return permissionStatus;
                }
            }
            return permissionStatus;
        }

        public static string SetNewLine(int newLineCount = 1)
        {
            string newLine = string.Empty;

            for (int i = 0; i < newLineCount; i++)
            {
                newLine += Environment.NewLine;
            }

            return newLine;
        }

        public static AlertConfig CommonAlertConfig()
        {
            return new AlertConfig
            {
                CornerRadius = 10,
                Margin = DeviceInfo.Idiom == DeviceIdiom.Tablet ? new Thickness(150, 0) : new Thickness(40, 0),
                PositiveColor = (Color)Application.Current.Resources["redColor"],
                NegativeColor = (Color)Application.Current.Resources["black54Opacity"],
                SeparatorColor = (Color)Application.Current.Resources["borderColor"]
            };
        }

        public static ActionSheetConfig CommonActionSheetConfig()
        {
            return new ActionSheetConfig
            {
                CancelTextColor = (Color)Application.Current.Resources["redColor"],
                ButtonsTextColor = (Color)Application.Current.Resources["redColor"],
                CancelStartColor = (Color)Application.Current.Resources["white"],
                CancelEndColor = (Color)Application.Current.Resources["white"],
            };
        }

        public static PickerDialogSettings PickerViewDialogConfig(string title = "")
        {
            return new PickerDialogSettings
            {
                CancelBackgroundColor = Colors.Transparent,
                CancelTextColor = (Color)Application.Current.Resources["blueColor"],
                OkBackgroundColor = Colors.Transparent,
                OkTextColor = (Color)Application.Current.Resources["redColor"],
                TitleTextColor = (Color)Application.Current.Resources["blackTextColor"],
                OkText = LocalizationResources.ok.ToUpper(),
                CancelText = LocalizationResources.cancel.ToUpper(),
                TitleText = title,
                IsSearchVisible = true,
                SearchPlaceholder = LocalizationResources.search,
                TintColor = (Color)Application.Current.Resources["blackTextColor"],
            };
        }

        public static bool IsValidEmail(string email)
        {
            var pattern = @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";

            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsValidPassword(string password)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password);
        }

        public static bool IsValidMobileNumber(string number)
        {
            var pattern = @"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?\d{4})|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$";

            if (string.IsNullOrEmpty(number))
            {
                return false;
            }
            return Regex.IsMatch(number, pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsValidPhoneNumber(string number)
        {
            var pattern = @"/^[\(]?[\+]?(\d{2}|\d{3})[\)]?[\s]?((\d{6}|\d{8})|(\d{3}[\*\.\-\s]){3}|(\d{2}[\*\.\-\s]){4}|(\d{4}[\*\.\-\s]){2})|\d{8}|\d{10}|\d{12}$/";

            if (string.IsNullOrEmpty(number))
            {
                return false;
            }
            return Regex.IsMatch(number, pattern, RegexOptions.IgnoreCase);
        }

        public static string FormatDate(DateTime dt)
        {
            string suffix = string.Empty;

            if (new[] { 11, 12, 13 }.Contains(dt.Day))
            {
                suffix = "th";
            }
            else if (dt.Day % 10 == 1)
            {
                suffix = "st";
            }
            else if (dt.Day % 10 == 2)
            {
                suffix = "nd";
            }
            else if (dt.Day % 10 == 3)
            {
                suffix = "rd";
            }
            else
            {
                suffix = "th";
            }
            return string.Format("{1:MMM} {0}{2} {1:yyyy}", dt.Day, dt, suffix);
        }

        public static async Task<Location> GetUserLocation()
        {
            Location location = null;
            try
            {
                var status = await CheckPermissions(new Permissions.LocationWhenInUse());

                if (status == PermissionStatus.Granted)
                {
                    if (!DependencyService.Get<ICommonUtils>().IsLocationEnabled())
                    {
                        if (DeviceInfo.Platform == DevicePlatform.Android)
                        {
                            var result = await DependencyService.Get<ICommonUtils>().EnableLocation();

                            if (result)
                            {
                                location = await GetLocationAsync();
                            }
                        }
                        else
                        {
                            await NavigationServices.OpenPopupPage(new NotificationPopup(NotificationType.Error, "Please turn on location.", string.Empty));
                        }
                        return location;
                    }

                    location = await GetLocationAsync();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return location;
        }

        private static async Task<Location> GetLocationAsync()
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));
            var userLocation = await Geolocation.Default.GetLocationAsync(request);

            if (userLocation == null)
            {
                userLocation = await Geolocation.Default.GetLastKnownLocationAsync();
            }

            return userLocation;
        }

        public static class GeoCodeCalc
        {
            public const double EarthRadiusInMiles = 3956.0;
            public const double EarthRadiusInKilometers = 6367.0;

            public static double ToRadian(double val) { return val * (Math.PI / 180); }
            public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }
        }

        public static Color SetTintColor(bool isSelected)
        {
            return isSelected ? (Color)Application.Current.Resources["redColor"] : (Color)Application.Current.Resources["grayColor"];
        }

        public static FileImageSource SetCheckImage(bool isSelected)
        {
            return new FileImageSource { File = isSelected ? "ic_check_selected" : "ic_check_unselected" };
        }

        public static FileImageSource SetRadioImage(bool isSelected)
        {
            return new FileImageSource { File = isSelected ? "ic_radio_checked" : "ic_radio_unchecked" };
        }

        public static Color SetErrorColor(bool isSelected)
        {
            return isSelected ? (Color)Application.Current.Resources["redColor"] : (Color)Application.Current.Resources["lightBorderColor"];
        }

        public static async Task<FileResult> OpenGallery()
        {
            try
            {
                PermissionStatus status = PermissionStatus.Unknown;

                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    status = await CheckPermissions(new Permissions.StorageRead());
                }
                else
                {
                    status = await CheckPermissions(new Permissions.Photos());
                }

                if (status == PermissionStatus.Granted)
                {
                    var file = await MediaPicker.PickPhotoAsync();
                    return file;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return null;
        }

        public static async Task<FileResult> TakePhoto()
        {
            var cameraStatus = await CheckPermissions(new Permissions.Camera());

            if (cameraStatus == PermissionStatus.Granted)
            {
                var file = await MediaPicker.CapturePhotoAsync();
                return file;
            }
            return null;
        }

        public static async Task<FileResult> OpenAttachmentDialog(bool isCameraOptionShow = true, bool isFileOptionShow = true)
        {
            FileResult file = null;
            try
            {
                var takeNow = isCameraOptionShow ? LocalizationResources.takeNow : null;
                var selectDocument = isFileOptionShow ? LocalizationResources.selectDocument : null;

                var action = await NavigationServices.DisplayActionSheet(LocalizationResources.selectFile, LocalizationResources.cancel,
                    CommonActionSheetConfig(), takeNow, LocalizationResources.chooseFromGallery, selectDocument);

                if (action != null)
                {
                    if (action == LocalizationResources.takeNow)
                    {
                        file = await TakePhoto();
                    }
                    else if (action == LocalizationResources.chooseFromGallery)
                    {
                        file = await OpenGallery();
                    }
                    else if (action == LocalizationResources.selectDocument)
                    {
                        file = await FilePicker.PickAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return file;
        }

        public static async void OpenBrowser(string url)
        {
            try
            {
                await Browser.OpenAsync(url, new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Show,
                    PreferredToolbarColor = (Color)Application.Current.Resources["redColor"],
                    PreferredControlColor = Colors.White
                });
                NavigationServices.SetBarTextColor(Colors.White.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static bool isLoaderVisible;
        public static async Task ShowLoader()
        {
            try
            {
                if (isLoaderVisible)
                {
                    return;
                }

                isLoaderVisible = true;
                await NavigationServices.OpenPopupPage(new LoaderPopup());
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static async Task HideLoader()
        {
            try
            {
                if (MopupService.Instance.PopupStack.Any() && MopupService.Instance.PopupStack.LastOrDefault() is LoaderPopup)
                {
                    await NavigationServices.ClosePopupPage();
                    isLoaderVisible = false;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
        
        static bool isInternetDialogVisible;
        public static async void CheckInternetAccess(ConnectivityChangedEventArgs e)
        {
            try
            {
                if (e.NetworkAccess == NetworkAccess.Internet)
                {
                    if (isInternetDialogVisible)
                    {
                        isInternetDialogVisible = false;
                        await NavigationServices.ClosePopupPage();
                        MessagingCenter.Send("refresh", "RefreshedToken");
                    }
                }
                else
                {
                    if (isInternetDialogVisible)
                    {
                        return;
                    }

                    isInternetDialogVisible = true;
                    await NavigationServices.DisplaySnackbar(new SnackbarConfig
                    {
                        ButtonText = LocalizationResources.retry,
                        Message = LocalizationResources.checkInternet,
                        ButtonTextColor = (Color)Application.Current.Resources["redColor"],
                        ButtonWidth = 80,
                        ButtonFontFamily = (OnPlatform<string>)Application.Current.Resources["AllerRegular"],
                        MessageFontFamily = (OnPlatform<string>)Application.Current.Resources["AllerRegular"],
                        ButtonCommand = new Command(() => {
                            CheckInternetAccess(new ConnectivityChangedEventArgs(Connectivity.NetworkAccess, Connectivity.ConnectionProfiles));
                        })
                    });
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }


        public static async Task<byte[]> GetResizedByteArray(byte[] fileData, string filePath)
        {
            byte[] resizeFile = null;
            try
            {
                var extension = Path.GetExtension(filePath);
                var size = await DependencyService.Get<ICommonUtils>().GetFileSize(fileData);

                var ratioX = (double)2000 / size.Width;
                var ratioY = (double)2000 / size.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(size.Width * ratio);
                var newHeight = (int)(size.Height * ratio);


                resizeFile = DependencyService.Get<ICommonUtils>().ResizeImage(fileData, newWidth, newHeight, extension ?? string.Empty);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return resizeFile;
        }

        public static string GetIFrameSource(string videoLink)
        {
            var width = ScreenSize.ScreenWidth - (DeviceInfo.Platform == DevicePlatform.Android ? 10 : 15);
            var height = (ScreenSize.ScreenWidth / 1.5) - 20;

            return $"<html>" +
                $"<head><meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0, user-scalable=no'></head>" +
                $"<body>" +
                $"<iframe src=\"{videoLink}\" height=\"{height}\" width=\"{width}\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" />" +
                $"</body>" +
                $"</html>";
        }
    }
}