using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LiteTube.Converters
{
    public class CommentMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                var isReplayComment = (bool)value;
                if (isReplayComment)
                    return new Thickness(50, 4, 0, 0);
            }

            return new Thickness(0, 4, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
