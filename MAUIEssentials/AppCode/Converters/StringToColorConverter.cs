using System.Globalization;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.AppCode.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                {
                    return null;
                }
                var color = value.ToString();
                return Color.FromArgb(color);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
