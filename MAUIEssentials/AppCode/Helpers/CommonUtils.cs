using System.Text.RegularExpressions;
using Mopups.Services;
using MAUIEssentials.AppCode.AlertViews;
using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppResources;
using MAUIEssentials.Models;
using MAUIEssentials.Services;
using MAUIEssentials.AppCode.Controls;
using MonkeyCache.FileStore;
using System.Globalization;
using static Microsoft.Maui.ApplicationModel.Permissions;
using FirebaseEssentials.Shared;
using Newtonsoft.Json;
using MAUIEssentials.AppCode.Behaviors;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Text;

namespace MAUIEssentials.AppCode.Helpers
{
    public static class CommonUtils
    {
        static DateTime _lastFetchLocation = DateTime.Now;
        static long _lastClickTime = 0;
        public const int IntervelTime = 600;

        public static double DefaultLat = 51.518284;
        public static double DefaultLon = -0.149588;

        public static Location? UserLocation { get; set; }
        public static string? UserLocationAddress { get; set; }

        public static List<Placemark>? _placemarks;
        public static List<Placemark>? Placemarks
        {
            get
            {
                try
                {
                    if (_placemarks == null || !_placemarks.Any())
                    {
                        string? countryCode = App.mauiContext?.Services.GetService<IDeviceRegionService>()?.GetDeviceCountryCode();
                        if (!string.IsNullOrEmpty(countryCode))
                        {
                            var lst = new List<Placemark>();
                            lst.Add(new Placemark()
                            {
                                CountryCode = countryCode
                            });
                            return _placemarks = lst;
                        }
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    ex.LogException();
                }

                return _placemarks;
            }
            set
            {
                _placemarks = value;
            }
        }

        public static string GoogleMapApiKey => "AIzaSyAiL90vZ4SJwb5I4dWFUhXAqJ7MStXz3Z0";

        public static string UTCDateTimeFormat => "yyyy-MM-dd HH:mm:ss";
        public static string UTCDateTimeWithSecFormat => "yyyy-MM-dd HH:mm:ss";

        public static string DateFormat => Settings.AppLanguage?.Language == AppLanguage.Arabic
            ? "yyyy'/'MM'/'dd" : "dd'/'MM'/'yyyy";

        public static string DateTimeFormat => Settings.AppLanguage?.Language == AppLanguage.Arabic
            ? "hh:mm tt yyyy'/'MM'/'dd" : "dd'/'MM'/'yyyy hh:mm tt";

        public static CultureInfo CurrentCulture => Settings.AppLanguage?.Language == AppLanguage.English
            ? CultureInfo.CreateSpecificCulture("en-US") : CultureInfo.CreateSpecificCulture("ar-JO");

        public static string ArabicPrefixForThe => (Settings.AppLanguage?.Language == AppLanguage.Arabic) ? "ال" : "";

        public static LanguageModel EnLanguages => App.Languages.FirstOrDefault(x => x.Code == "en");
        public static LanguageModel ArLanguages => App.Languages.FirstOrDefault(x => x.Code == "ar");

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
                        OpenAppSettings();
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
                    var isHuawei = Settings.IsHuaweiApp;

                    var title = $"Permission denied";
                    var question = $"To use this feature, {permissionName} permission is required.";
                    var positive = "Settings";
                    var negative = "Maybe Later";

                    Task<bool>? task = null;
                    if (isHuawei)
                    {
                        permissionStatus = await App.mauiContext?.Services.GetService<IRequestPermission>()?.RequestAsync(permission);
                    }
                    else
                    {
                        task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
                    }

                    if (task == null)
                    {
                        return permissionStatus;
                    }

                    var result = await task;
                    if (result)
                    {
                        OpenAppSettings();
                    }

                    return permissionStatus;
                }
            }
            return permissionStatus;
        }

        private static async Task ShowPermissionRationale()
        {
            bool openSettings = await NavigationServices.DisplayAlert(
                LocalizationResources.permissionRequired,
                LocalizationResources.permissionRequiredForCamera,
                LocalizationResources.openSettings,
                LocalizationResources.cancel
            );

            if (openSettings)
            {
                OpenAppSettings();
            }
        }

        private static void OpenAppSettings()
        {
            try
            {
                AppInfo.ShowSettingsUI();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open settings: {ex.Message}");
            }
        }

        static bool isPermissionDialogVisible;
        private static async Task PermissionDeniedDialog(string permissionName)
        {
            if (isPermissionDialogVisible)
            {
                return;
            }

            var title = permissionName.Contains("location") ? "Location services is required" : "Permission denied";
            var question = permissionName.Contains("location") ? "For best user experience, please enable location services" : $"To use the feature, {permissionName} permission is required.";
            var positive = "Settings";
            var negative = "Maybe Later";
            var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);

            if (task == null)
            {
                isPermissionDialogVisible = false;
                return;
            }

            isPermissionDialogVisible = true;
            var result = await task;

            if (result)
            {
                AppInfo.ShowSettingsUI();
            }
            isPermissionDialogVisible = false;
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
                PositiveColor = AppColorResources.white.ToColor(),
                PositiveButtonColor = AppColorResources.blueColor.ToColor(),
                NegativeColor = AppColorResources.redColor.ToColor(),
                NegativeButtonColor = AppColorResources.white.ToColor(),
                SeparatorColor = AppColorResources.borderColor.ToColor(),
                NegativeBorderColor = AppColorResources.lightBorderColor2.ToColor(),
                MessageColor = AppColorResources.grayTextColor.ToColor(),
                TitleColor = AppColorResources.blackTextColor.ToColor(),
                MessageFontFamily = (string)Application.Current.Resources["FontRegular"],
                PositiveButtonFontFamily = (string)Application.Current.Resources["FontMedium"],
                NegativeButtonFontFamily = (string)Application.Current.Resources["FontRegular"],
                TitleFontFamily = (string)Application.Current.Resources["FontBold"],
            };
        }

        public static AlertConfig CommonAlertConfigDelete()
        {
            return new AlertConfig
            {
                CornerRadius = 10,
                Margin = DeviceInfo.Idiom == DeviceIdiom.Tablet ? new Thickness(150, 0) : new Thickness(30, 0),
                PositiveColor = AppColorResources.white.ToColor(),
                PositiveButtonColor = AppColorResources.blueColor.ToColor(),
                NegativeColor = AppColorResources.white.ToColor(),
                NegativeButtonColor = AppColorResources.redColor.ToColor(),
                SeparatorColor = AppColorResources.lightBorderColor2.ToColor(),
                NegativeBorderColor = AppColorResources.lightBorderColor2.ToColor(),
                MessageColor = AppColorResources.grayTextColor.ToColor(),
                TitleColor = AppColorResources.blackTextColor.ToColor(),
                MessageFontFamily = (string)Application.Current.Resources["FontRegular"],
                PositiveButtonFontFamily = (string)Application.Current.Resources["FontMedium"],
                NegativeButtonFontFamily = (string)Application.Current.Resources["FontMedium"],
                TitleFontFamily = (string)Application.Current.Resources["FontBold"],
            };
        }

        public static AlertConfig CommonAlertConfigForAccountDelete()
        {
            return new AlertConfig
            {
                CornerRadius = 10,
                Margin = DeviceInfo.Idiom == DeviceIdiom.Tablet ? new Thickness(150, 0) : new Thickness(30, 0),
                PositiveColor = AppColorResources.white.ToColor(),
                PositiveButtonColor = AppColorResources.redColor.ToColor(),
                NegativeColor = AppColorResources.white.ToColor(),
                NegativeButtonColor = AppColorResources.blueColor.ToColor(),
                SeparatorColor = AppColorResources.lightBorderColor2.ToColor(),
                NegativeBorderColor = AppColorResources.lightBorderColor2.ToColor(),
                MessageColor = AppColorResources.grayTextColor.ToColor(),
                TitleColor = AppColorResources.blackTextColor.ToColor(),
                MessageFontFamily = (string)Application.Current.Resources["FontRegular"],
                PositiveButtonFontFamily = (string)Application.Current.Resources["FontMedium"],
                NegativeButtonFontFamily = (string)Application.Current.Resources["FontMedium"],
                TitleFontFamily = (string)Application.Current.Resources["FontBold"],
            };
        }

        public static ActionSheetConfig CommonActionSheetConfig()
        {
            return new ActionSheetConfig
            {
                CancelTextColor = AppColorResources.redColor.ToColor(),
                ButtonsTextColor = AppColorResources.blueColor.ToColor(),
                CancelStartColor = AppColorResources.white.ToColor(),
                CancelEndColor = AppColorResources.white.ToColor(),
                ButtonsFontFamily = (string)Application.Current.Resources["FontMedium"],
                CancelFontFamily = (string)Application.Current.Resources["FontMedium"],
                TitleFontFamily = (string)Application.Current.Resources["FontRegular"]
            };
        }

        public static PickerDialogSettings PickerViewDialogConfig(string title = "")
        {
            return new PickerDialogSettings
            {
                CancelBackgroundColor = Colors.Transparent,
                CancelTextColor = AppColorResources.redColor.ToColor(),
                OkBackgroundColor = Colors.Transparent,
                OkTextColor = AppColorResources.blueColor.ToColor(),
                TitleTextColor = AppColorResources.blackTextColor.ToColor(),
                OkText = LocalizationResources.ok.ToUpper(),
                CancelText = LocalizationResources.cancel.ToUpper(),
                TitleText = title,
                IsSearchVisible = true,
                SearchPlaceholder = LocalizationResources.search,
                TintColor = AppColorResources.blackTextColor.ToColor(),
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

        public static async Task<Location> GetUserLocation()
        {
            Location location = null;
            try
            {
                var status = await CheckPermissions(new LocationWhenInUse());

                if (status == PermissionStatus.Denied)
                {
                    return null;
                }

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
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            finally
            {
                _lastFetchLocation = DateTime.Now;
            }
            return location;
        }

        public static async Task FetchLocation()
        {
            try
            {
                var timeSpan = DateTime.Now - _lastFetchLocation;

                if (timeSpan.TotalMinutes > 1 || UserLocation == null)
                {
                    await GetUserLocation();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static async Task<Tuple<string, Placemark>> GetAddress(double latitude, double longitude)
        {
            Placemark? placemark = null;
            string address = string.Empty;

            try
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    return new Tuple<string, Placemark>(address, placemark);
                }

                var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
                Placemarks = new List<Placemark>(placemarks);

                if (Placemarks != null && Placemarks.Any())
                {
                    placemark = Placemarks.FirstOrDefault();

                    if (!string.IsNullOrEmpty(placemark?.Thoroughfare))
                    {
                        address += placemark.Thoroughfare + ", ";
                    }

                    if (!string.IsNullOrEmpty(placemark?.SubLocality))
                    {
                        if (string.IsNullOrEmpty(placemark.Thoroughfare))
                        {
                            address += placemark.SubLocality + ", ";

                        }
                        else if (!placemark.Thoroughfare.Equals(placemark.SubLocality))
                        {
                            address += placemark.SubLocality + ", ";
                        }
                    }

                    if (!string.IsNullOrEmpty(placemark?.Locality))
                    {
                        address += placemark.Locality + ", ";
                    }

                    if (!string.IsNullOrEmpty(placemark?.AdminArea))
                    {
                        if (string.IsNullOrEmpty(placemark.Locality))
                        {
                            if (string.IsNullOrEmpty(placemark.Thoroughfare))
                            {
                                address += placemark.AdminArea + ", ";

                            }
                            else if (!placemark.Thoroughfare.Equals(placemark.AdminArea))
                            {
                                address += placemark.AdminArea + ", ";
                            }
                        }
                        else if (!placemark.Locality.Equals(placemark.AdminArea))
                        {
                            address += placemark.AdminArea + ", ";
                        }
                    }

                    if (!string.IsNullOrEmpty(placemark?.CountryName))
                    {
                        address += placemark.CountryName + ", ";
                    }

                    if (!string.IsNullOrEmpty(placemark?.CountryCode))
                    {
                        if (string.IsNullOrEmpty(placemark.PostalCode))
                        {
                            address += placemark.CountryCode;
                        }
                        else
                        {
                            address += placemark.CountryCode + ", " + placemark.PostalCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return new Tuple<string, Placemark>(address, placemark);
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

            public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
            {
                return CalcDistance(lat1, lng1, lat2, lng2, GeoCodeCalcMeasurement.Miles);
            }

            public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m)
            {
                double radius = EarthRadiusInMiles;

                if (m == GeoCodeCalcMeasurement.Kilometers) { radius = EarthRadiusInKilometers; }
                return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt(Math.Pow(Math.Sin(DiffRadian(lat1, lat2) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin(DiffRadian(lng1, lng2) / 2.0), 2.0))));
            }
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

        public static string GetEnglishWeekName(string name)
        {
            switch (name)
            {
                case "الأحد":
                    return DayOfWeek.Sunday.ToString();
                case "الاثنين":
                    return DayOfWeek.Monday.ToString();
                case "الثلاثاء":
                    return DayOfWeek.Tuesday.ToString();
                case "الأربعاء":
                    return DayOfWeek.Wednesday.ToString();
                case "الخميس":
                    return DayOfWeek.Thursday.ToString();
                case "الجمعة":
                    return DayOfWeek.Friday.ToString();
                case "السبت":
                    return DayOfWeek.Saturday.ToString();
                default:
                    return name;
            }
        }

        public static string GetWeekDaysName(DayOfWeek dayOfWeek, bool isTrimmed = false)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "الأحد" : isTrimmed ? dayOfWeek.ToString().Substring(0, 3) : dayOfWeek.ToString();
                case DayOfWeek.Monday:
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "الاثنين" : isTrimmed ? dayOfWeek.ToString().Substring(0, 3) : dayOfWeek.ToString();
                case DayOfWeek.Tuesday:
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "الثلاثاء" : isTrimmed ? dayOfWeek.ToString().Substring(0, 3) : dayOfWeek.ToString();
                case DayOfWeek.Wednesday:
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "الأربعاء" : isTrimmed ? dayOfWeek.ToString().Substring(0, 3) : dayOfWeek.ToString();
                case DayOfWeek.Thursday:
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "الخميس" : isTrimmed ? dayOfWeek.ToString().Substring(0, 3) : dayOfWeek.ToString();
                case DayOfWeek.Friday:
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "الجمعة" : isTrimmed ? dayOfWeek.ToString().Substring(0, 3) : dayOfWeek.ToString();
                case DayOfWeek.Saturday:
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "السبت" : isTrimmed ? dayOfWeek.ToString().Substring(0, 3) : dayOfWeek.ToString();
                default:
                    return isTrimmed ? dayOfWeek.ToString().Substring(0, 3) : dayOfWeek.ToString();
            }
        }

        public static string GetMonthName(DateTime monthName, string monthformat = "MMM")
        {
            switch (monthName.ToString("MMMM", CultureInfo.CreateSpecificCulture("en-US")))
            {
                case "January":
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "يناير" : monthName.ToString(monthformat, CultureInfo.InvariantCulture);
                case "February":
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "فبراير" : monthName.ToString(monthformat, CultureInfo.InvariantCulture);
                case "March":
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "مارس" : monthName.ToString(monthformat, CultureInfo.InvariantCulture);
                case "April":
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "ابريل" : monthName.ToString(monthformat, CultureInfo.InvariantCulture);
                case "May":
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "مايو" : monthName.ToString(monthformat, CultureInfo.InvariantCulture);
                case "June":
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "يونيو" : monthName.ToString(monthformat, CultureInfo.InvariantCulture);
                case "July":
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "يوليو" : monthName.ToString(monthformat, CultureInfo.InvariantCulture);
                case "August":
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "أغسطس" : monthName.ToString(monthformat, CultureInfo.InvariantCulture);
                case "September":
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "سبتمبر" : monthName.ToString(monthformat, CultureInfo.InvariantCulture);
                case "October":
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "أكتوبر" : monthName.ToString(monthformat, CultureInfo.InvariantCulture);
                case "November":
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "نوفمبر" : monthName.ToString(monthformat, CultureInfo.InvariantCulture);
                case "December":
                    return Settings.AppLanguage?.Language == AppLanguage.Arabic ? "ديسمبر" : monthName.ToString(monthformat, CultureInfo.InvariantCulture);
                default:
                    return monthName.ToString(monthformat, CultureInfo.InvariantCulture);
            }
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

                if (DeviceInfo.Platform == DevicePlatform.Android && Double.Parse(DeviceInfo.VersionString) > 13)
                {
                    status = await CheckPermissions(new StorageRead());
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
            try
            {
                var cameraStatus = await CheckPermissions(new Permissions.Camera());

                if (cameraStatus == PermissionStatus.Denied)
                {
                    await ShowPermissionRationale();
                }

                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var file = await MediaPicker.Default.CapturePhotoAsync();
                    return file;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
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

        public static async Task OpenMap(Location location, MapLaunchOptions options)
        {
            try
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS && App.mauiContext?.Services.GetService<ICheckMap>() != null)
                {
                    var isAppleMap = App.mauiContext.Services.GetService<ICheckMap>().IsAppleMap();
                    var isGoogleMap = App.mauiContext.Services.GetService<ICheckMap>().IsGoogleMap();

                    if (isAppleMap && isGoogleMap)
                    {
                        var actionSheet = await NavigationServices.DisplayActionSheet(LocalizationResources.showOnMap, LocalizationResources.cancel,
                                CommonActionSheetConfig(), LocalizationResources.appleMap, LocalizationResources.googleMap);

                        if (actionSheet == LocalizationResources.googleMap)
                        {
                            App.mauiContext?.Services.GetService<ICheckMap>()?.OpenGoogleMap(location.Latitude, location.Longitude);
                        }
                        else if (actionSheet == LocalizationResources.appleMap)
                        {
                            await Map.OpenAsync(location, options);
                        }
                    }
                    else
                    {
                        await Map.OpenAsync(location, options);
                    }
                }
                else
                {
                    var isHuawei = Settings.IsHuaweiApp;
                    if (isHuawei)
                    {
                        await Map.OpenAsync(location, options);
                    }
                    else
                    {
                        OpenBrowser(string.Format("https://maps.google.com?q={0},{1}", location.Latitude, location.Longitude));
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static async void OpenBrowser(string url, BrowserLaunchMode launchMode = BrowserLaunchMode.SystemPreferred)
        {
            try
            {
                await Browser.OpenAsync(url, new BrowserLaunchOptions
                {
                    LaunchMode = launchMode,
                    TitleMode = BrowserTitleMode.Show,
                    PreferredControlColor = AppColorResources.white.ToColor(),
                    PreferredToolbarColor = AppColorResources.headerbluecolor.ToColor(),
                });
                NavigationServices.SetBarTextColor(Colors.White.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task OpenBrowserAsync(string url, BrowserLaunchMode launchMode = BrowserLaunchMode.SystemPreferred)
        {
            try
            {
                await Browser.OpenAsync(url, new BrowserLaunchOptions
                {
                    LaunchMode = launchMode,
                    TitleMode = BrowserTitleMode.Show,
                    PreferredControlColor = AppColorResources.white.ToColor(),
                    PreferredToolbarColor = AppColorResources.headerbluecolor.ToColor(),
                });
                NavigationServices.SetBarTextColor(Colors.White.ToString());
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        static bool isLoaderVisible;
        public static async Task ShowLoader(bool isDelay = true)
        {
            try
            {
                if (isLoaderVisible)
                {
                    return;
                }

                isLoaderVisible = true;
                await NavigationServices.OpenPopupPage(new LoaderPopup(LocalizationResources.pleaseWait));
                if (isDelay)
                {
                    await Task.Delay(200);
                }
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
                        ButtonTextColor = AppColorResources.redColor.ToColor(),
                        ButtonWidth = 80,
                        ButtonFontFamily = (OnPlatform<string>)Application.Current.Resources["AllerRegular"],
                        MessageFontFamily = (OnPlatform<string>)Application.Current.Resources["AllerRegular"],
                        ButtonCommand = new Command(() =>
                        {
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

        public static Color SetErrorBorderColor(bool isValid)
        {
            return isValid ? AppColorResources.lightBorderColor2.ToColor() : AppColorResources.redColor.ToColor();
        }

        public static async Task<byte[]> GetResizedByteArray(byte[] fileData, string filePath)
        {
            byte[]? resizeFile = null;
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

        public static string GetIFrameSource(string videoLink, double height = 0, double width = 0)
        {
            if (height == 0)
            {
                height = (ScreenSize.ScreenWidth / 1.5) - 20;
            }
            if (width == 0)
            {
                width = ScreenSize.ScreenWidth - (DeviceInfo.Platform == DevicePlatform.Android ? 10 : 15);
            }

            return $"<html>" +
                $"<head><meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0, user-scalable=no'></head>" +
                $"<body>" +
                $"<iframe src=\"{videoLink}\" height=\"{height}\" width=\"{width}\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" />" +
                $"</body>" +
                $"</html>";
        }

        public static string GetIFrameFullScreenSource(string videoLink)
        {
            return $"<html>" +
                $"<head><meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0, user-scalable=no'></head>" +
                $"<body style='margin: 0; padding: 0; overflow: hidden;'>" +
                $"<iframe height=\"100%\" width=\"100%\" style=\"border: 0\" frameborder=\"0\" loading=\"lazy\" allowfullscreen src=\"{videoLink}\" />" +
                $"</body>" +
                $"</html>";
        }

        public static double GetStatusBarHeight()
        {
            try
            {
                var statusBarHeight = Application.Current?.Resources.MergedDictionaries
                       .Where(dictionary => dictionary.ContainsKey("StatusBarHeight"))
                       .Select(dictionary => dictionary["StatusBarHeight"])
                       .FirstOrDefault();
                if (statusBarHeight != null)
                {
                    return Convert.ToDouble((OnPlatform<double>)statusBarHeight);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }

            return 20;
        }

        public static List<T> UpdateDatabase<T>(string key, List<T> data) where T : new()
        {
            if (Barrel.Current.Exists(key))
            {
                Barrel.Current.Empty(key);
            }

            var encryptedData = DataSecurity.Encrypt(JsonConvert.SerializeObject(data));
            Barrel.Current.Add(key, encryptedData, TimeSpan.FromDays(7));

            var list = GetDatabase<List<T>>(key);
            return list;
        }

        public static T GetDatabase<T>(string key) where T : new()
        {
            var data = Barrel.Current.Get<string>(key);

            if (string.IsNullOrEmpty(data))
            {
                return new T();
            }

            var decryptData = DataSecurity.Decrypt(data);
            var result = JsonConvert.DeserializeObject<T>(decryptData, new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            });
            return result;
        }

        public static double GetiPhoneXHeight()
        {
            try
            {
                var iPhoneXHeight = Application.Current?.Resources.MergedDictionaries
                       .Where(dictionary => dictionary.ContainsKey("iPhoneXHeight"))
                       .Select(dictionary => dictionary["iPhoneXHeight"])
                       .FirstOrDefault();
                if (iPhoneXHeight != null)
                {
                    return Convert.ToDouble((OnPlatform<double>)iPhoneXHeight);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }

            return 55;
        }

        public static string GetMapEmbedSource(double latitude, double longitude)
        {
            var source = $"https://www.google.com/maps/embed/v1/place?key={"Enter the google map key"}&q={latitude},{longitude}&language={Settings.AppLanguage?.Code}";

            return $"<html>" +
                $"<head><meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0, user-scalable=no'></head>" +
                $"<body>" +
                $"<iframe height=\"100%\" width=\"100%\" style=\"border: 0\" loading=\"lazy\" allowfullscreen src=\"{source}\" />" +
                $"</body>" +
                $"</html>";
        }

        public static List<List<DateTime>> GetWeeksInMonth()
        {
            var startDate = DateTime.Today.FirstDayOfMonth();
            var endDate = DateTime.Today.LastDayOfMonth();

            while (startDate.DayOfWeek != DayOfWeek.Sunday)
            {
                startDate = startDate.AddDays(1);
            }

            var list = new List<List<DateTime>>();

            for (DateTime result = startDate; result < endDate; result = result.AddDays(7))
            {
                list.Add(result.GetWeek(DayOfWeek.Monday));
            }

            return list;
        }

        public static async Task AdsFailedToLoad(AddsView adView, bool hideWhenFail = true)
        {
            if (hideWhenFail)
                adView.IsVisible = false;
        }

        public static async void SetAds(AddsView adView, bool isShown)
        {
            if (!isShown)
            {
                adView.IsVisible = false;
                return;
            }
            try
            {
                var isHuawei = Settings.IsHuaweiApp;
                if (Settings.AppType == AppTypeEnum.Owner)
                {
                    adView.AdsId = DeviceInfo.Platform == DevicePlatform.Android
                        ? isHuawei ? ServiceConstants.OwnerBannerAdHuawei : ServiceConstants.OwnerBannerAdAndroid
                        : ServiceConstants.OwnerBannerAdiOS;
                }
                else
                {
                    adView.AdsId = DeviceInfo.Platform == DevicePlatform.Android
                        ? isHuawei ? ServiceConstants.BannerAdHuawei : ServiceConstants.BannerAdAndroid
                        : ServiceConstants.BannerAdiOS;
                }
                // var user = await SecureSettings.GetLoginUser();
                // if (Settings.AdsSettings != null && user.ShowAds)
                // {
                //     adView.IsVisible = isShown;
                // }
                // else
                // {
                //     adView.IsVisible = false;
                // }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static async Task<bool> CheckLocation()
        {
            var permissionStatus = await CheckPermissions(new LocationWhenInUse());

            if (permissionStatus == PermissionStatus.Denied)
            {
                var status = await NavigationServices.DisplayAlert(string.Empty, LocalizationResources.locationPermissionInfoMsg,
                    LocalizationResources.allow, LocalizationResources.cancel, CommonAlertConfig());

                if (!status)
                {
                    return false;
                }

                AppInfo.ShowSettingsUI();
                return false;
            }

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                var locationEnabled = await DependencyService.Get<ICommonUtils>().EnableLocation();

                if (!locationEnabled)
                {
                    return false;
                }
            }

            return true;
        }

        public static string formatDuration(int fromHours, int fromMinutes, int toHours = 0, int toMinutes = 0)
        {
            if ((toHours > 0 || toMinutes > 0) && (fromHours > 0 || fromMinutes > 0))
            {
                if (fromHours == toHours)
                {
                    if (fromMinutes > toMinutes)
                    {
                        return (toHours > 0 ? toHours.ToString().ConvertNumerals() + " " + LocalizationResources.hrStr + " " : string.Empty) + (toMinutes > 0 ? toMinutes.ToString().ConvertNumerals() + " " + LocalizationResources.mins : string.Empty) + " - " + (fromHours > 0 ? fromHours.ToString().ConvertNumerals() + " " + LocalizationResources.hrStr + " " : string.Empty) + (fromMinutes > 0 ? fromMinutes.ToString().ConvertNumerals() + " " + LocalizationResources.mins : string.Empty);
                    }
                    else if (fromMinutes == toMinutes)
                    {
                        return (fromHours > 0 ? fromHours.ToString().ConvertNumerals() + " " + LocalizationResources.hrStr + " " : string.Empty) + (fromMinutes > 0 ? fromMinutes.ToString().ConvertNumerals() + " " + LocalizationResources.mins : string.Empty);
                    }
                    else
                    {
                        return (fromHours > 0 ? fromHours.ToString().ConvertNumerals() + " " + LocalizationResources.hrStr + " " : string.Empty) + (fromMinutes > 0 ? fromMinutes.ToString().ConvertNumerals() + " " + LocalizationResources.mins : string.Empty) + " - " + (toHours > 0 ? toHours.ToString().ConvertNumerals() + " " + LocalizationResources.hrStr + " " : string.Empty) + (toMinutes > 0 ? toMinutes.ToString().ConvertNumerals() + " " + LocalizationResources.mins : string.Empty);
                    }
                }
                else
                {
                    if (fromHours > toHours || fromMinutes > toMinutes)
                    {
                        return (toHours > 0 ? toHours.ToString().ConvertNumerals() + " " + LocalizationResources.hrStr + " " : string.Empty) + (toMinutes > 0 ? toMinutes.ToString().ConvertNumerals() + " " + LocalizationResources.mins : string.Empty) + " - " + (fromHours > 0 ? fromHours.ToString().ConvertNumerals() + " " + LocalizationResources.hrStr + " " : string.Empty) + (fromMinutes > 0 ? fromMinutes.ToString().ConvertNumerals() + " " + LocalizationResources.mins : string.Empty);
                    }
                    else
                    {
                        return (fromHours > 0 ? fromHours.ToString().ConvertNumerals() + " " + LocalizationResources.hrStr + " " : string.Empty) + (fromMinutes > 0 ? fromMinutes.ToString().ConvertNumerals() + " " + LocalizationResources.mins : string.Empty) + " - " + (toHours > 0 ? toHours.ToString().ConvertNumerals() + " " + LocalizationResources.hrStr + " " : string.Empty) + (toMinutes > 0 ? toMinutes.ToString().ConvertNumerals() + " " + LocalizationResources.mins : string.Empty);
                    }
                }
            }
            else if (toHours > 0 || toMinutes > 0)
            {
                return (toHours > 0 ? toHours.ToString().ConvertNumerals() + " " + LocalizationResources.hrStr + " " : string.Empty) + (toMinutes > 0 ? toMinutes.ToString().ConvertNumerals() + " " + LocalizationResources.mins : string.Empty);
            }
            else
            {
                return (fromHours > 0 ? fromHours.ToString().ConvertNumerals() + " " + LocalizationResources.hrStr + " " : string.Empty) + (fromMinutes > 0 ? fromMinutes.ToString().ConvertNumerals() + " " + LocalizationResources.mins : string.Empty);

            }
        }

        public static void SetToolTip(BindableObject bindableObject)
        {
            try
            {
                if (TooltipEffect.GetHasTooltip(bindableObject))
                {
                    TooltipEffect.SetHasTooltip(bindableObject, false);
                    TooltipEffect.SetHasTooltip(bindableObject, true);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static void SetTappedTooltip(ContentView content, BindableObject bindableObject, string classId)
        {
            try
            {
                if (content.ClassId != classId && TooltipEffect.GetHasTooltip(bindableObject))
                {
                    TooltipEffect.SetHasTooltip(bindableObject, false);
                    TooltipEffect.SetHasTooltip(bindableObject, true);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static async Task SendEmail(List<string> recipients, string subject, string body)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                ToastPopup.Instance.ShowMessage(LocalizationResources.emailNotSupport);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static Span GetSpan(string text, AlertConfig alertConfig)
        {
            double fontSize = (Application.Current?.Resources["DeviceStyle"].ToString() == "Default") ? Application.Current.Resources["FontSize14"] as OnIdiom<double> :
                   fontSize = Application.Current?.Resources["FontSize14"] as OnPlatform<double>;

            return new Span
            {
                Text = text,
                TextColor = alertConfig.MessageColor,
                FontSize = fontSize,
                FontFamily = alertConfig.MessageFontFamily
            };
        }

        public static bool IsValidLatitudeLongitude(double Latitude, double Longitude)
        {
            if (Latitude < -90 || Latitude > 90 || Longitude < -180 || Longitude > 180)
            {
                return false;
            }
            return true;
        }

        public static string FormatAmount(this double amount)
        {
            return amount.ToString("N2");
        }

        public static string FormatAmount(this decimal amount)
        {
            return amount.ToString("N2");
        }

        public static string FormatAmountAndInt(this decimal? amount)
        {
            if (amount == null)
                return string.Empty;
            if (amount == (int)amount)
            {
                return amount.GetValueOrDefault().ToString("N0");
            }
            return amount.GetValueOrDefault().ToString("N2");
        }

        public static string FormatAmountAndInt(this double? amount)
        {
            if (amount == null)
                return string.Empty;
            if (amount == (int)amount)
            {
                return amount.GetValueOrDefault().ToString("N0");
            }
            return amount.GetValueOrDefault().ToString("N2");
        }

        public static string FormatAmountAndInt(this decimal amount)
        {
            if (amount == null)
                return string.Empty;
            if (amount == (int)amount)
            {
                return amount.ToString("N0");
            }
            return amount.ToString("N2");
        }

        public static decimal? RoundDigits(this decimal? amount)
        {
            if (amount == null)
                return amount;
            return RoundDigits(amount.GetValueOrDefault());
        }

        public static decimal RoundDigits(this decimal amount)
        {
            return Math.Round(amount, 2);
        }

        public static string GetHour(string hr)
        {
            return hr == "1" ? LocalizationResources.hrStr : LocalizationResources.hrsStr;
        }

        public static DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }

        public static double GetNavigationBarHeight()
        {
            try
            {
                var navigationBarHeight = Application.Current?.Resources.MergedDictionaries
                       .Where(dictionary => dictionary.ContainsKey("NavigationBarHeight"))
                       .Select(dictionary => dictionary["NavigationBarHeight"])
                       .FirstOrDefault();
                if (navigationBarHeight != null && navigationBarHeight is OnPlatform<double> onPlatformIdiom)
                {
                    return Convert.ToDouble(onPlatformIdiom);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }

            return 55;
        }

        public static List<string> GetDaysOfWeek()
        {
            List<string> daysOfWeek = new List<string>();

            try
            {
                // Get all values in the DayOfWeek enumeration
                Array values = System.Enum.GetValues(typeof(DayOfWeek));
                // Iterate through the values and get the name of each day
                for (int i = 0; i < values.Length; i++)
                {
                    DayOfWeek dayOfWeek = (DayOfWeek)values.GetValue(i);
                    daysOfWeek.Add(dayOfWeek.ToString());
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }

            return daysOfWeek;
        }

        public static List<string> GetDaysOfWeekShort()
        {
            List<string> daysOfWeek = new List<string>();

            try
            {
                Array values = System.Enum.GetValues(typeof(DayOfWeek));
                for (int i = 0; i < values.Length; i++)
                {
                    DayOfWeek dayOfWeek = (DayOfWeek)values.GetValue(i);
                    daysOfWeek.Add(dayOfWeek.ToString().Substring(0, 3));
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }

            return daysOfWeek;
        }

        public static void ChangeLanguageSetData()
        {
            try
            {
                Settings.SubscriptionPlanLevelList = null;
                Settings.SubscriptionResponse = null;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public static bool IsSubscriptionValid(string PurchaseToken)
        {
            try
            {
                if (string.IsNullOrEmpty(PurchaseToken))
                    return false;

                byte[] purchaseTokenData = Convert.FromBase64String(PurchaseToken);
                if (purchaseTokenData == null || purchaseTokenData.Length == 0)
                    return false;

                Dictionary<string, string> receiptDic = ParsePropertyList(purchaseTokenData);
                if (receiptDic == null || !receiptDic.ContainsKey("purchase-info"))
                    return false;

                string purchaseInfoContent = receiptDic["purchase-info"];
                if (string.IsNullOrEmpty(purchaseInfoContent))
                    return false;

                byte[] purchaseInfoData = Convert.FromBase64String(purchaseInfoContent);
                if (purchaseInfoData == null || purchaseInfoData.Length == 0)
                    return false;

                Dictionary<string, string> purchaseInfoDic = ParsePropertyList(purchaseInfoData);
                if (purchaseInfoDic == null || !purchaseInfoDic.ContainsKey("expires-date"))
                    return false;

                string msStr = purchaseInfoDic["expires-date"];
                if (string.IsNullOrEmpty(msStr))
                    return false;

                long milliseconds = Convert.ToInt64(msStr);
                DateTime expireDate = new DateTime(1970, 1, 1).AddMilliseconds(milliseconds);

                return DateTime.UtcNow < expireDate;

            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            // Add handling for other platforms if needed

            return false;
        }

        static Dictionary<string, string> ParsePropertyList(byte[] data)
        {
            try
            {
                Dictionary<string, string> dect = new Dictionary<string, string>();
                string plistString = Encoding.UTF8.GetString(data).Replace("\n", "").Replace("\t", "");
                var split = plistString.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in split)
                {
                    if (item.Contains("="))
                    {
                        var index = item.IndexOf("=");
                        if (index != -1)
                        {
                            var key = item.Substring(1, index - 1).Replace("=", "").Replace("\"", "").Trim();
                            var value = item.Substring(index + 1).Replace("\"", "").Trim();
                            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                            {
                                dect.Add(key, value);
                            }
                        }
                    }
                }
                return dect;
            }
            catch (Exception ex)
            {
                ex.LogException();
                return null;
            }
        }

        public static DateTime ParseDateTime(IDictionary<string, object> data, string key)
        {
            try
            {
                if (data.TryGetValue(key, out var value) && value != null)
                {
                    if (DateTime.TryParseExact(value.ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US"), System.Globalization.DateTimeStyles.None, out var dateTime))
                    {
                        return dateTime;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return DateTime.MinValue;
        }

        public static long ParseLong(IDictionary<string, object> data, string key)
        {
            try
            {
                if (data.TryGetValue(key, out var value) && value != null)
                {
                    if (long.TryParse(value.ToString(), out var result))
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return 0;
        }
    }
}