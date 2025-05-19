namespace MAUIEssentials.AppCode.Converters
{
    public class IsBoldConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new Object();

            if (value is bool myBoolValue)
            {
                result = myBoolValue ? new FontAttributesConverter().ConvertFromInvariantString("Bold") : "";
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}