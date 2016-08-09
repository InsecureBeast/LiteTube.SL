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
    public class TimeSpanFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return TimeSpan.FromSeconds(0).ToString();

            var timespan = (TimeSpan)value;
            if (timespan.TotalHours < 1)
                return timespan.ToString(@"mm\:ss");

            return timespan.ToString();
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
            throw new NotImplementedException();
        }
    }
}
