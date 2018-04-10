using System;
using System.Windows.Media;
using System.Globalization;

namespace SodukoGui
{
    public class BoolToColorConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return new SolidColorBrush(Colors.AliceBlue); ;
            bool input = (bool)value;

            if (input)
                return new SolidColorBrush(Colors.AliceBlue);
            else
                return new SolidColorBrush(Colors.PaleVioletRed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
