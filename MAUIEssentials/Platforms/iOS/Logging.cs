    using System.Diagnostics;
using System.Globalization;
using QuickLook;
using UIKit;

namespace MAUIEssentials
{
    public partial class Logging //: ILogging
    {
        private string fileText = string.Empty;

        public Logging()
        {
            DeleteAllFiles();
        }

        private void DeleteAllFiles()
        {
            try
            {
                var todayFile = GetFile();
                var files = Directory.GetFiles(GetLogDirectory());

                if (files.Any())
                {
                    var list = files.OfType<string>().ToList();
                    list.Remove(todayFile);

                    foreach (var item in list)
                    {
                        File.Delete(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private string GetLogDirectory()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var logDirectory = string.Format("{0}/log", path);

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            return logDirectory;
        }

        public partial string GetFile()
        {
            return string.Format("{0}/log_{1}.txt", GetLogDirectory(), DateTime.Today.ToString("dd'-'MM'-'yyyy", CultureInfo.CreateSpecificCulture("en-US")));
        }

        public partial void ClearLog()
        {
            var file = GetFile();

            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        public partial void OpenLog()
        {
            var path = GetFile();

            if (File.Exists(path))
            {
                FileInfo fi = new FileInfo(path);
                UIViewController? controller = GetTopViewController();

                QLPreviewController previewController = new QLPreviewController
                {
                    DataSource = new PDFPreviewControllerDataSource(fi.FullName, fi.Name)
                };

                if (controller != null)
                {
                    controller.PresentViewController(previewController, true, null);
                }
            }
        }

        public partial void WriteLog(string title, string log)
        {
            try
            {
                string text = string.Empty;

                text += "--------------------------------" + Environment.NewLine;
                text += "Timestamp: " + DateTime.Now.ToString("HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")) + Environment.NewLine + Environment.NewLine;
                text += title + Environment.NewLine;
                text += log + Environment.NewLine;
                text += "--------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine;

                fileText += text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private UIViewController? GetTopViewController()
        {
            var window = UIApplication.SharedApplication?.KeyWindow;
            var vc = window?.RootViewController;

            while (vc?.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            if (vc is UINavigationController navController)
            {
                vc = navController?.ViewControllers?.Last();
            }
            return vc;
        }

        public async partial void WriteLogFile()
        {
            try
            {
                var file = GetFile();
                await File.WriteAllTextAsync(file, fileText);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
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

        //public override string ItemTitle
        //{
        //    get { return _title; }
        //}

        //public override NSUrl ItemUrl
        //{
        //    get { return NSUrl.FromFilename(_uri); }
        //}
    }
}
