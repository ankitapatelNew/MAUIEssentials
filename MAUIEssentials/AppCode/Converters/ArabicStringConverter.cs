using System.Globalization;
using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.AppCode.Converters
{
    public class ArabicStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && !string.IsNullOrEmpty($"{value}")) {
				return value.ToString().ConvertNumerals(parameter != null && parameter.ToString() == "invert");
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}
