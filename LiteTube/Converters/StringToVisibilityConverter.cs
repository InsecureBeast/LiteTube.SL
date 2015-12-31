using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LiteTube.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;
            if (val == null)
                return Visibility.Collapsed;

            return string.IsNullOrEmpty(val) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
