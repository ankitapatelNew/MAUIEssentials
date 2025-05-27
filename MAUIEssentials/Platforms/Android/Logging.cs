using System.Globalization;
using Android.Content;
using Android.OS;
using Android.Webkit;
using Debug = System.Diagnostics.Debug;

namespace MAUIEssentials
{
    public partial class Logging
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
            var path = Platform.CurrentActivity?.GetExternalFilesDir(null)?.AbsolutePath;
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
                Android.Net.Uri? uri = null;
                Java.IO.File file = new Java.IO.File(path);
                var context = Platform.CurrentActivity;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    uri = AndroidX.Core.Content.FileProvider.GetUriForFile(context, context?.PackageName + ".fileprovider", file);
                }
                else
                {
                    uri = Android.Net.Uri.FromFile(file);
                }

                Intent intent = new Intent(Intent.ActionView);
                string extension = MimeTypeMap.GetFileExtensionFromUrl(uri?.ToString()) ?? string.Empty;
                string mimetype = MimeTypeMap.Singleton?.GetMimeTypeFromExtension(extension) ?? string.Empty;

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

                context?.StartActivity(Intent.CreateChooser(intent, "Choose application:"));
            }
        }

        public partial void WriteLog(string title, string log)
        {
            try
            {
                string text = string.Empty;

                text += "--------------------------------" + System.Environment.NewLine;
                text += "Timestamp: " + DateTime.Now.ToString("HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US")) + System.Environment.NewLine + System.Environment.NewLine;
                text += title + System.Environment.NewLine;
                text += log + System.Environment.NewLine;
                text += "--------------------------------" + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine;

                fileText += text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
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
}
