using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using MAUIEssentials.Models;
using Newtonsoft.Json;

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
            return Color.FromArgb(hexColor);
        }

        public static int ToInt(this string intValue)
        {
            return Convert.ToInt32(intValue);
        }

        public static decimal ToDecimal(this string decimalValue)
        {
            return Convert.ToDecimal(decimalValue);
        }

        public static double ToDouble(this string doubleValue)
        {
            return Convert.ToDouble(doubleValue);
        }

        public static double ToDouble(this int intValue)
        {
            return Convert.ToDouble(intValue);
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

        //TODO:Check this
        public static bool IsColorDark(this Color color)
        {
            if (color.IsDefault() || color.GetHue() < 0 || color.GetLuminosity() < 0 || color.GetSaturation() < 0)
            {
                return false;
            }

            var newColor = Color.FromHsla(color.GetHue(), color.GetSaturation(), color.GetLuminosity());
            return newColor.GetLuminosity() <= 0.5;
        }

        public static string ConvertNumerals(this string input, bool reverse = false)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            ;
            if (reverse)
            {
                return input.Replace('\u0660', '0')
                        .Replace('\u0661', '1')
                        .Replace('\u0662', '2')
                        .Replace('\u0663', '3')
                        .Replace('\u0664', '4')
                        .Replace('\u0665', '5')
                        .Replace('\u0666', '6')
                        .Replace('\u0667', '7')
                        .Replace('\u0668', '8')
                        .Replace('\u0669', '9')
                        .Replace('\u066B', '.');
            }

            if (Settings.AppLanguage?.Language == AppLanguage.Arabic)
            {
                return input.Replace('0', '\u0660')
                        .Replace('1', '\u0661')
                        .Replace('2', '\u0662')
                        .Replace('3', '\u0663')
                        .Replace('4', '\u0664')
                        .Replace('5', '\u0665')
                        .Replace('6', '\u0666')
                        .Replace('7', '\u0667')
                        .Replace('8', '\u0668')
                        .Replace('9', '\u0669')
                        .Replace('.', '\u066B');
            }
            else return input;
        }

        public static bool HasArabicText(this string input)
        {
            var regex = new Regex(@"[\u0600-\u06ff]|[\u0750-\u077f]|[\ufb50-\ufc3f]|[\ufe70-\ufefc]");
            var engRegex = new Regex(@"[A-Za-z0-9]");

            return regex.IsMatch(input) && !engRegex.IsMatch(input);
        }

        public static bool HasEnglishText(this string input)
        {
            var arabicRegex = new Regex(@"[\u0600-\u06ff]|[\u0750-\u077f]|[\ufb50-\ufc3f]|[\ufe70-\ufefc]");
            var regex = new Regex(@"[A-Za-z\d]");

            return regex.IsMatch(input) && !arabicRegex.IsMatch(input);
        }

        public static bool ContainsArabicText(this string input)
        {
            var regex = new Regex(@"[\u0600-\u06ff]|[\u0750-\u077f]|[\ufb50-\ufc3f]|[\ufe70-\ufefc]");
            return regex.IsMatch(input);
        }

        public static bool ContainsEnglishText(this string input)
        {
            var regex = new Regex(@"[A-Za-z0-9]");
            return regex.IsMatch(input);
        }

        public static bool IsArabicNumbers(this string input)
        {
            var regex = new Regex(@"[\u0660-\u0669]|[\u06d4]");
            return regex.IsMatch(input);
        }

        public static bool IsEnglishNumbers(this string input)
        {
            var regex = new Regex(@"[\d]|[.]");
            return regex.IsMatch(input);
        }

        public static string GetNumbersOnly(this string input)
        {
            var regex = new Regex(@"[\u0660-\u0669]|[\u06d4]|[\d]|[.]");
            var matches = regex.Matches(input);
            return string.Join("", matches.Select(x => x.Value).ToList());
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

        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.DaysInMonth());
        }

        public static bool IsInCurrentWeek(this DateTime date)
        {
            var startOfWeek = DateTime.Today.AddDays((int)DayOfWeek.Sunday - (int)DateTime.Today.DayOfWeek);
            var currentWeek = Enumerable.Range(0, 7).Select(x => startOfWeek.AddDays(x)).ToList();

            return currentWeek.Any(x => x.Date == date.Date);
        }

        public static List<DateTime> GetWeek(this DateTime date, DayOfWeek dayOfWeek)
        {
            var startOfWeek = date.AddDays((int)dayOfWeek - (int)date.DayOfWeek);
            return Enumerable.Range(0, 7).Select(x => startOfWeek.AddDays(x)).ToList();
        }

        public static bool IsJson(this string json)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject(json);
                return obj != null;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public static string ToTitleCase(this string input)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(input);
        }
        
        public static string FirstCharToUpper(this string input) =>
		input switch
		{
			null => throw new ArgumentNullException(nameof(input)),
			"" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
			_ => input[0].ToString().ToUpper() + input.Substring(1)
		};


        public static Color GetColor(this string colorName)
        {
            Color primaryColor = new Color();
            try
            {
                foreach (var item in Application.Current.Resources.MergedDictionaries)
                {
                    if (item.TryGetValue(colorName, out var color))
                    {
                        if (color is Color primary)
                        {
                            primaryColor = primary;
                            break; // Exit the loop when you find the color
                        }
                    }
                }
                return primaryColor;
            }
            catch (Exception ex)
            {
                ex.LogException();
                return primaryColor;
            }
        }
    }
}