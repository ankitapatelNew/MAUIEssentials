using Android.Content;
using Android.Gms.Common.Apis;
using Android.Locations;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Plugin.Maui.Biometric;
using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;
using Bitmap = Android.Graphics.Bitmap;
using BitmapFactory = Android.Graphics.BitmapFactory;
using FileProvider = AndroidX.Core.Content.FileProvider;
using Platform = Microsoft.Maui.ApplicationModel.Platform;
using Size = Microsoft.Maui.Graphics.Size;
using Typeface = Android.Graphics.Typeface;
using MAUIEssentials.AppCode.Controls;
using Microsoft.Maui.Platform;
using Android.Graphics.Drawables;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using AndroidX.Core.App;

namespace MAUIEssentials.Platforms.Android.DepedencyServices
{
    public class CommonUtils : ICommonUtils
    {
        private static CommonUtils? _instance;

        public static CommonUtils Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommonUtils();
                }
                return _instance;
            }
        }

        public async Task<LocalBioMetricOption> BioMetricAuthAvailability()
        {
            var haveTouchId = await BiometricAuthenticationService.Default.GetEnrolledBiometricTypesAsync();
            foreach (var item in haveTouchId)
            {
                if (item != BiometricType.None)
                {
                    return LocalBioMetricOption.Fingerprint;
                }
            }
            return LocalBioMetricOption.None;
        }

        public void CloseApplication()
        {
            try
            {
                Platform.CurrentActivity.FinishAffinity();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public Task<bool> EnableLocation()
        {
            var activity = Platform.CurrentActivity as MauiAppCompatActivity;
            var listener = new ActivityResultListener(activity);

            Task.Run(async () =>
            {
                try
                {
                    var locationRequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                    var location = await Geolocation.Default.GetLocationAsync(locationRequest);

                    if (location != null)
                    {
                        Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                    }
                }
                catch (ApiException ex)
                {
                    switch (ex.StatusCode)
                    {
                        case CommonStatusCodes.ResolutionRequired:
                            try
                            {
                                ResolvableApiException resolvable = (ResolvableApiException)ex;
                                resolvable.StartResolutionForResult(activity, 100);
                            }
                            catch (Exception ex1)
                            {
                                Console.WriteLine(ex1.Message);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            return listener.Task;
        }

        string CreateDocumentDirectory(string folderName)
        {
            var document = GetDocumentDirectoryPath() + "/" + folderName + "/";
            if (!Directory.Exists(document))
            {
                Directory.CreateDirectory(document);
            }
            return document;
        }

        public string GetDocumentDirectoryPath()
        {
            return FileSystem.AppDataDirectory;
        }

        public async Task<Size> GetFileSize(byte[] fileData)
        {
            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            await BitmapFactory.DecodeByteArrayAsync(fileData, 0, fileData.Length, options);
            return new Size(Convert.ToDouble(options.OutWidth), Convert.ToDouble(options.OutHeight));
        }

        public bool IsIphone18OrAbove()
        {
            return false;
        }

        public bool IsIphoneX()
        {
            return false;
        }

        public bool IsLocationEnabled()
        {
            LocationManager locationManager = (LocationManager)Platform.CurrentActivity.GetSystemService(Context.LocationService);
            return locationManager.IsProviderEnabled(LocationManager.GpsProvider);
        }

        public Size MeasureTextSize(string text, double width, double fontSize, string fontName = null)
        {
            var textView = new TextView(Platform.AppContext);
            textView.SetText(text, TextView.BufferType.Normal);
            textView.SetTextSize(ComplexUnitType.Sp, fontSize.ToInt());
            textView.Typeface = GetTypeface(fontName);

            var density = Platform.AppContext.Resources.DisplayMetrics.Density;
            var widthDensity = width * density;

            int widthMeasureSpec = global::Android.Views.View.MeasureSpec.MakeMeasureSpec(widthDensity.ToInt(), MeasureSpecMode.AtMost);
            int heightMeasureSpec = global::Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);

            textView.Measure(widthMeasureSpec, heightMeasureSpec);
            return new Size(((double)textView.MeasuredWidth / density), ((double)textView.MeasuredHeight / density));
        }

        Typeface? textTypeface;
        private Typeface? GetTypeface(string fontName)
        {
            if (fontName == null)
            {
                return Typeface.Default;
            }

            if (textTypeface == null)
            {
                textTypeface = Typeface.Create(fontName, global::Android.Graphics.TypefaceStyle.Normal);
            }
            return textTypeface;
        }

        public void OpenDocument(string filePath)
        {
            try
            {
                global::Android.Net.Uri? uri = null;
                Java.IO.File? file = new Java.IO.File(filePath);
                var context = Platform.CurrentActivity;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    uri = FileProvider.GetUriForFile(context, context.PackageName + ".fileprovider", file);
                }
                else
                {
                    uri = global::Android.Net.Uri.FromFile(file);
                }

                Intent intent = new Intent(Intent.ActionView);
                string extension = MimeTypeMap.GetFileExtensionFromUrl(uri.ToString());
                string mimetype = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);

                if (string.IsNullOrEmpty(extension) || mimetype == null)
                {
                    intent.SetDataAndType(uri, "text/*");
                }
                else
                {
                    intent.SetDataAndType(uri, mimetype);
                }
                intent.SetFlags(ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);

                context.StartActivity(Intent.CreateChooser(intent, "Choose application:"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public byte[] ResizeImage(byte[] imageData, float width, float height, string extension = "")
        {
            // Load the bitmap 
            BitmapFactory.Options options = new BitmapFactory.Options();
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);

            float newHeight = 0;
            float newWidth = 0;

            var originalHeight = originalImage.Height;
            var originalWidth = originalImage.Width;

            if (originalHeight > originalWidth)
            {
                newHeight = height;
                float ratio = originalHeight / height;
                newWidth = originalWidth / ratio;
            }
            else
            {
                newWidth = width;
                float ratio = originalWidth / width;
                newHeight = originalHeight / ratio;
            }

            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, true);
            originalImage.Recycle();

            using (MemoryStream ms = new MemoryStream())
            {
                if (extension == ".jpg" || extension == ".jpeg")
                {
                    resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 95, ms);
                }
                else
                {
                    resizedImage.Compress(Bitmap.CompressFormat.Png, 95, ms);
                }
                resizedImage.Recycle();
                return ms.ToArray();
            }
        }

        public string SaveDocument(string folderName, string name, string base64File)
        {
            try
            {
                _ = LoaderPopup.Instance.ShowLoader("Saving");
                byte[] fileArray = Convert.FromBase64String(base64File);
                string filePath = Path.Combine(GetDocumentDirectoryPath(), name);

                File.WriteAllBytes(filePath, fileArray);
                _ = LoaderPopup.Instance.HideLoader();

                OpenDocument(filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _ = LoaderPopup.Instance.HideLoader();
                ex.LogException();
            }
            return string.Empty;
        }

        public string SaveDocument(string folderName, string name, byte[] fileArray)
        {
            try
            {
                _ = LoaderPopup.Instance.ShowLoader("Saving");
                string filePath = Path.Combine(GetDocumentDirectoryPath(), name);

                File.WriteAllBytes(filePath, fileArray);
                _ = LoaderPopup.Instance.HideLoader();

                OpenDocument(filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _ = LoaderPopup.Instance.HideLoader();
                ex.LogException();
            }
            return string.Empty;
        }

        public void StatusbarColor(Color color)
        {
            try
            {
                var activity = Platform.CurrentActivity;

                activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                activity.Window.SetStatusBarColor(color.ToPlatform());

                if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                {
                    var controller = activity.Window.InsetsController;

                    if (controller != null)
                    {
                        if (color == Colors.White)
                        {
                            controller.SetSystemBarsAppearance((int)WindowInsetsControllerAppearance.LightStatusBars, (int)WindowInsetsControllerAppearance.LightStatusBars);
                        }
                        else
                        {
                            controller.SetSystemBarsAppearance((int)WindowInsetsControllerAppearance.None, (int)WindowInsetsControllerAppearance.LightStatusBars);
                        }
                    }
                }
                else
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                    {
                        var decorView = activity.Window.DecorView;

#pragma warning disable CS0618 // Type or member is obsolete
                        if (color == Colors.White)
                        {
                            decorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                        }
                        else
                        {
                            decorView.SystemUiVisibility &= ~(StatusBarVisibility)SystemUiFlags.LightStatusBar;
                        }
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex?.Message);
            }
        }

        public long SystemClockERealtime()
        {
            return SystemClock.ElapsedRealtime();
        }

        public Size GetImageSize(string fileName)
        {
            var context = Platform.AppContext;

            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            fileName = fileName.Replace('-', '_').Replace(".png", "").Replace(".jpg", "");
            var resId = context.Resources.GetIdentifier(fileName, "drawable", context.PackageName);
            BitmapFactory.DecodeResource(context.Resources, resId, options);

            return new Size(Convert.ToDouble(options.OutWidth), Convert.ToDouble(options.OutHeight));
        }

        public void StatusbarColor(Color startColor, Color endColor, GradientOrientation orientation)
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    var activity = Platform.CurrentActivity;
                    if(activity == null)
                    {
                        return;
                    }
                    activity.Window?.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    activity.Window?.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    activity.Window?.SetStatusBarColor(global::Android.Graphics.Color.Transparent);

                    var drawable = new GradientDrawable();
                    drawable.SetGradientType(GradientType.LinearGradient);

                    drawable.SetColors(new int[] {
                        startColor.ToAndroid().ToArgb(),
                        endColor.ToAndroid().ToArgb()
                    });

                    switch (orientation)
                    {
                        case GradientOrientation.Horizontal:
                            drawable.SetOrientation(GradientDrawable.Orientation.LeftRight);
                            break;
                        case GradientOrientation.Vertical:
                            drawable.SetOrientation(GradientDrawable.Orientation.TopBottom);
                            break;
                        case GradientOrientation.Diagonal:
                            drawable.SetOrientation(GradientDrawable.Orientation.TlBr);
                            break;
                        case GradientOrientation.ReverseDiagonal:
                            drawable.SetOrientation(GradientDrawable.Orientation.BlTr);
                            break;
                    }

                    activity.Window?.SetBackgroundDrawable(drawable);

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                    {
                        var controller = activity.Window?.InsetsController;

                        if (controller != null)
                        {
                            if (startColor == Colors.White)
                            {
                                controller.SetSystemBarsAppearance((int)WindowInsetsControllerAppearance.LightStatusBars, (int)WindowInsetsControllerAppearance.LightStatusBars);
                            }
                            else
                            {
                                controller.SetSystemBarsAppearance((int)WindowInsetsControllerAppearance.None, (int)WindowInsetsControllerAppearance.LightStatusBars);
                            }
                        }
                    }
                    else
                    {
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                        {
                            var decorView = activity.Window.DecorView;

#pragma warning disable CS0618 // Type or member is obsolete
                            if (startColor == Colors.White)
                            {
                                decorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                            }
                            else
                            {
                                decorView.SystemUiVisibility &= ~(StatusBarVisibility)SystemUiFlags.LightStatusBar;
                            }
#pragma warning restore CS0618 // Type or member is obsolete
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex?.Message);
            }
        }

        public string GetDeviceId()
        {
            string id = string.Empty;

            try
            {
                var context = Platform.AppContext;
                id = global::Android.Provider.Settings.Secure.GetString(context.ContentResolver, global::Android.Provider.Settings.Secure.AndroidId);
            }
            catch (Exception ex)
            {
                Log.Warn("DeviceInfo", "Unable to get id: " + ex.ToString());
            }
            return id;
        }

        public async Task<bool> CheckNotificationPermission()
        {
            var context = Platform.AppContext;
            var notificationManager = NotificationManagerCompat.From(context);

            if (notificationManager != null && notificationManager.AreNotificationsEnabled())
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channels = notificationManager.NotificationChannels;

                    if (channels != null && channels.Any())
                    {
                        foreach (var item in channels)
                        {
                            if (item.Importance == global::Android.App.NotificationImportance.Unspecified
                                || item.Importance == global::Android.App.NotificationImportance.None)
                            {
                                return false;
                            }

                            if (Build.VERSION.SdkInt >= BuildVersionCodes.P && !string.IsNullOrEmpty(item.Group))
                            {
                                var channelGroup = notificationManager.GetNotificationChannelGroup(item.Group);

                                if (channelGroup != null && channelGroup.IsBlocked)
                                {
                                    return false;
                                }
                            }
                        }
                    }

                    return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public string GetExportsFolder()
        {
            string root = string.Empty;

            root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var document = root + "/Exports/";
            if (!Directory.Exists(document))
            {
                Directory.CreateDirectory(document);
            }
            return document;
        }
    }
}
