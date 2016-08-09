using System;
using System.Globalization;
using System.Windows.Data;

namespace LiteTube.Core.Converters
{
    public class RelatedVideoMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = value as bool?;
            if (isVisible == null)
                return 2000;

            if (isVisible == false)
                return 2000;

            return 189;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
