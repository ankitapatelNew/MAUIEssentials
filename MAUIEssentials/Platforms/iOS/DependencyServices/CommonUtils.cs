using CoreGraphics;
using CoreLocation;
using Foundation;
using Plugin.Maui.Biometric;
using QuickLook;
using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;
using UIKit;
using MAUIEssentials.AppCode.Controls;
using UserNotifications;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;

namespace MAUIEssentials.Platforms.iOS.DependencyServices
{
    public class CommonUtils : ICommonUtils
    {
        public static CommonUtils? _instance;

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
            set
            {
                _instance = value;
            }
        }

        public bool IsIphoneX()
        {
            return ScreenSize.ScreenHeight > 800 && UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad;
        }

        public long SystemClockERealtime()
        {
            return 0;
        }

        public void StatusbarColor(Color color)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                UIView statusBar = UIApplication.SharedApplication.KeyWindow?.ViewWithTag(101);

                if (statusBar == null && UIApplication.SharedApplication.KeyWindow != null)
                {
                    statusBar = new UIView(UIApplication.SharedApplication.KeyWindow.WindowScene.StatusBarManager.StatusBarFrame);
                    statusBar.Tag = 101;
                    UIApplication.SharedApplication.KeyWindow?.AddSubview(statusBar);
                }

                if (statusBar != null)
                {
                    statusBar.BackgroundColor = color.ToUIColor();
                }
            }
            else
            {
                var statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBar")) as UIView;
                statusBar.BackgroundColor = color.ToUIColor();
            }
        }

        public async Task<bool> EnableLocation()
        {
            return false;
        }

        public bool IsLocationEnabled()
        {
            return CLLocationManager.LocationServicesEnabled;
        }

        public async Task<LocalBioMetricOption> BioMetricAuthAvailability()
        {
            var haveTouchId = await BiometricAuthenticationService.Default.GetEnrolledBiometricTypesAsync();
            foreach (var item in haveTouchId)
            {
                var context = new LocalAuthentication.LAContext();
                context.CanEvaluatePolicy(LocalAuthentication.LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out NSError error);
                if (context.BiometryType == LocalAuthentication.LABiometryType.TouchId)
                {
                    return LocalBioMetricOption.Fingerprint;
                }
                else if (context.BiometryType == LocalAuthentication.LABiometryType.FaceId)
                {
                    return LocalBioMetricOption.Face;
                }
            }
            return LocalBioMetricOption.None;
        }

        public void CloseApplication()
        {
            Thread.CurrentThread.Abort();
        }

        public Size MeasureTextSize(string text, double width, double fontSize, string fontName = null)
        {
            var nsText = new NSString(text);
            var boundSize = new System.Drawing.SizeF((float)width, float.MaxValue);
            var options = NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin;

            if (fontName == null)
            {
                fontName = "HelveticaNeue";
            }

            var attributes = new UIStringAttributes
            {
                Font = UIFont.FromName(fontName, (float)fontSize)
            };

            var sizeF = nsText.GetBoundingRect(boundSize, options, attributes, null).Size;
            return new Size(sizeF.Width, sizeF.Height);
        }

        string CreateDocumentDirectory(string folderName)
        {
            var document = Path.Combine(GetDocumentDirectoryPath(), folderName);
            if (!Directory.Exists(document))
            {
                Directory.CreateDirectory(document);
            }
            return document;
        }

        public string GetDocumentDirectoryPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public string SaveDocument(string folderName, string name, string base64File)
        {
            try
            {
                _ = LoaderPopup.Instance.ShowLoader("Saving");
                byte[] fileArray = Convert.FromBase64String(base64File);
                string filePath = Path.Combine(CreateDocumentDirectory(folderName), name);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

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
                string filePath = Path.Combine(CreateDocumentDirectory(folderName), name);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

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

        public void OpenDocument(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            UINavigationController controller = FindNavigationController();

            QLPreviewController previewController = new QLPreviewController
            {
                DataSource = new PDFPreviewControllerDataSource(fi.FullName, fi.Name)
            };

            if (controller != null)
            {
                controller.PresentViewController(previewController, true, null);
            }
        }

        private UINavigationController FindNavigationController()
        {
            foreach (var window in UIApplication.SharedApplication.Windows)
            {
                if (window.RootViewController != null)
                {
                    if (window.RootViewController.NavigationController != null)
                    {
                        return window.RootViewController.NavigationController;
                    }
                    else
                    {
                        UINavigationController val = CheckSubs(window.RootViewController.ChildViewControllers);
                        if (val != null)
                        {
                            return val;
                        }
                    }
                }
            }
            return null;
        }

        private UINavigationController CheckSubs(UIViewController[] controllers)
        {
            foreach (var controller in controllers)
            {
                if (controller.NavigationController != null)
                    return controller.NavigationController;
                else
                {
                    UINavigationController val = CheckSubs(controller.ChildViewControllers);
                    if (val != null)
                        return val;
                }
            }
            return null;
        }

        public UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            return new UIImage(NSData.FromArray(data));
        }

        public async Task<Size> GetFileSize(byte[] fileData)
        {
            UIImage image = ImageFromByteArray(fileData);
            return new Size(Convert.ToDouble(image.Size.Width), Convert.ToDouble(image.Size.Height));
        }

        public byte[] ResizeImage(byte[] imageData, float width, float height, string extension)
        {
            UIImage originalImage = ImageFromByteArray(imageData);

            var originalHeight = originalImage.Size.Height;
            var originalWidth = originalImage.Size.Width;

            nfloat newHeight = 0;
            nfloat newWidth = 0;

            if (originalHeight > originalWidth)
            {
                newHeight = height;
                nfloat ratio = originalHeight / height;
                newWidth = originalWidth / ratio;
            }
            else
            {
                newWidth = width;
                nfloat ratio = originalWidth / width;
                newHeight = originalHeight / ratio;
            }

            width = (float)newWidth;
            height = (float)newHeight;

            // TO BE CLEANED
            UIGraphics.BeginImageContext(new CGSize(width, height));
            originalImage.Draw(new CGRect(0, 0, width, height));

            var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            byte[] bytesImagen;

            if (extension == ".jpg" || extension == ".jpeg")
            {
                bytesImagen = resizedImage.AsJPEG().ToArray();
            }
            else
            {
                bytesImagen = resizedImage.AsPNG().ToArray();
            }

            resizedImage.Dispose();
            return bytesImagen;
        }

        public bool IsIphone18OrAbove()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                return UIDevice.CurrentDevice.CheckSystemVersion(18, 0);
            }
            return false;
        }

        public Size GetImageSize(string fileName)
        {
            UIImage image = UIImage.FromFile(fileName);
            return new Size(Convert.ToDouble(image?.Size.Width), Convert.ToDouble(image?.Size.Height));
        }

        public void StatusbarColor(Color startColor, Color endColor, GradientOrientation orientation)
        {
        }

        public string GetDeviceId()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor.ToString();
        }

        public Task<bool> CheckNotificationPermission()
        {
            var tcs = new TaskCompletionSource<bool>();

            var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
            UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => {
                if (error != null)
                {
                    tcs.TrySetResult(false);
                }
                else if (!granted)
                {
                    tcs.TrySetResult(false);
                }
                else
                {
                    tcs.TrySetResult(true);
                }
            });

            return tcs.Task;
        }
        public  string GetExportsFolder()
        {
            return CreateDocumentDirectory("Exports");
        }
    }

    public class PDFPreviewControllerDataSource : QLPreviewControllerDataSource
    {
        string _url = "";
        string _fileName = "";

        public PDFPreviewControllerDataSource(string url, string fileName)
        {
            _url = url;
            _fileName = fileName;
        }

        public override IQLPreviewItem GetPreviewItem(QLPreviewController controller, nint index)
        {
            return new PDFItem(_fileName, _url);
        }

        public override nint PreviewItemCount(QLPreviewController controller)
        {
            return 1;
        }
    }

    public class PDFItem : QLPreviewItem
    {
        string _title;
        string _uri;

        public PDFItem(string title, string uri)
        {
            _title = title;
            _uri = uri;
        }

        public override NSUrl PreviewItemUrl
        {
            get { return NSUrl.FromFilename(_uri); }
        }

        public override string PreviewItemTitle
        {
            get { return _title; }
        }
    }
}
