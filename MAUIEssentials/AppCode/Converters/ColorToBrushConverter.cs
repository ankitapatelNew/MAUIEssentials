using System.Globalization;

namespace MAUIEssentials.AppCode.Converters
{
    public class ColorToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null) {
				if (value.GetType().Equals(typeof(Color))) {
					return new SolidColorBrush((Color)value);

				} else if (value.GetType().Equals(typeof(string))) {
					return new SolidColorBrush(Color.FromArgb((string)value));
				}
			}
			return Brush.Default;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
