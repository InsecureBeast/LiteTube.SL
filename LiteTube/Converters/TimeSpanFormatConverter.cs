using System;
using System.Globalization;
using System.Windows.Data;

namespace LiteTube.Converters
{
    public class TimeSpanFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return TimeSpan.FromSeconds(0).ToString();

            var timespan = (TimeSpan)value;
            if (timespan.TotalHours < 1)
                return timespan.ToString(@"mm\:ss");

            return timespan.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
