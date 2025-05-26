using System.Globalization;

namespace MAUIEssentials.AppCode.Converters
{
    public class DashConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()) && !value.ToString().  Equals("0"))
            {
                if (value.ToString() == DateTime.MinValue.ToString())
                {
                    return "-";
                }
                return value;
            }
            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}