using System.Globalization;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.AppCode.Converters
{
    public class GregorianDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var date = (DateTime)value;
                return date.ToString("dd/MM/yyyy", CommonUtils.CurrentCulture);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
