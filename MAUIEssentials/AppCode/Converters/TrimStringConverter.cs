using System.Globalization;

namespace MAUIEssentials.AppCode.Converters
{
    public class TrimStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && !string.IsNullOrWhiteSpace($"{value}")) {
				return value.ToString().Trim();
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}
