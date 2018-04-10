using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SodukoGui
{
    public class BoolToItalic : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? boolValue = value as bool?;
            if (boolValue == null)
                return FontStyles.Normal;
            if ((bool)boolValue)
                return FontStyles.Italic;
            return FontStyles.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
