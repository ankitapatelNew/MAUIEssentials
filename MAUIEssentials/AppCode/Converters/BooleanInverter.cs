using System.Globalization;

namespace MAUIEssentials.AppCode.Converters
{
    public class BooleanInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is bool)
                {
                    return !((bool)value);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }

    public class EmptyValueToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter != null) {
				if (parameter.ToString() == "invert") {
					return value == null || string.IsNullOrEmpty($"{value}");
				}
			}

			return value != null && !string.IsNullOrEmpty($"{value}");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}

    public class StringVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is string status)
                {
                    if (!string.IsNullOrEmpty(status))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }

    public class EmptyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is string status)
                {
                    if (!string.IsNullOrEmpty(status))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}