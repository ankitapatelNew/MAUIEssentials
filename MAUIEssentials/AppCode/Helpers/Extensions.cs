namespace MAUIEssentials.AppCode.Helpers
{
    public static class Extensions
    {
        public static void LogException(this Exception ex, bool showAlert = false, [CallerFilePath] string filePath = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            if (showAlert)
            {
            }

            System.Diagnostics.Debug.WriteLine(string.Format("App Log : FilePath: {0} MethodName: {1} Line: {2}\nExReport: {3}\n\nStackTrace: {4}",
                                                             filePath,
                                                             member,
                                                             line,
                                                             ex?.Message,
                                                             ex?.StackTrace));
        }

        public static string Encode(this string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            str = Encoding.UTF8.GetString(bytes);
            return str;
        }

        public static byte[] Decode(this byte[] bytes)
        {
            string value = Encoding.Default.GetString(bytes);
            bytes = Encoding.UTF8.GetBytes(value);
            return bytes;
        }

        public static string ToHex(this Color color)
        {
            return color.ToString();
        }

        public static Color ToColor(this string hexColor)
        {
            return Color.FromHex(hexColor);
        }

        public static int ToInt(this string intValue)
        {
            return Convert.ToInt32(intValue);
        }

        public static double ToDouble(this string doubleValue)
        {
            return Convert.ToDouble(doubleValue);
        }

        public static int ToInt(this double doubleValue)
        {
            return Convert.ToInt32(doubleValue);
        }

        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return Enum.TryParse<T>(value, true, out T result) ? result : defaultValue;
        }

        public static DateTime UtcToLocalDateTime(this DateTime utcDate)
        {
            utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);
            return utcDate.ToLocalTime();
        }

        public static void SetSelectedTab(this ImageButton image, string iconSource, bool isSelected)
        {
            var selectedSource = isSelected
                ? string.Format("BottomTab/{0}_selected", iconSource)
                : string.Format("BottomTab/{0}", iconSource);

            image.Source = selectedSource;
        }

        public static async Task ContinuesRotateAnimation(this VisualElement element, CancellationTokenSource tokenSource)
        {
            while (!tokenSource.IsCancellationRequested)
            {
                await element.RotateTo(360, 1200);
                await element.RotateTo(0, 0);
            }
        }

        public static bool IsHtml(this string text)
        {
            var regex = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            return regex.IsMatch(text);
        }

        public static string GetFileSize(this byte[] file)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = Convert.ToDouble(file.Length);
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }

        public static async Task<MemoryStream> GetMemoryStreamAsync(this FileResult result)
        {
            var memoryStream = new MemoryStream();

            using (Stream stream = await result.OpenReadAsync())
            {
                await stream.CopyToAsync(memoryStream);
            }
            return memoryStream;
        }
        
        public static string ToTitleCase(this string input)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(input);
        }
    }
}