using System;
using System.Globalization;
using System.Windows.Data;

namespace LiteTube.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            if (value is DateTime)
            {
                var date = (DateTime)value;
                var now = DateTime.Now.Date;

                if (date.Date == now)
                    return date.ToString("t", culture);

                if (date.Date.Year != now.Date.Year)
                    return date.ToString("g", culture);

                var shortDate = date.ToString("d", culture);
                var resDate = shortDate.Substring(0, shortDate.Length - 5);
                return string.Format("{0} {1}", resDate, date.ToString("t", culture));
            }

            return string.Empty;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
