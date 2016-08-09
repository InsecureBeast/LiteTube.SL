using System;
using System.Globalization;
using System.Windows.Data;
#if SILVERLIGHT

#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif

namespace LiteTube.Core.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return string.Empty;

            if (value is DateTime)
            {
                var date = (DateTime)value;
                var now = DateTime.Now.Date;

                if (date.Date == now)
                    return date.ToString("t");

                if (date.Date.Year != now.Date.Year)
                    return date.ToString("g");

                var shortDate = date.ToString("d");
                var resDate = shortDate.Substring(0, shortDate.Length - 5);
                return string.Format("{0} {1}", resDate, date.ToString("t"));
            }

            return string.Empty;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture.EnglishName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
