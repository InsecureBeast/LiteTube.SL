using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
#if SILVERLIGHT

#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif

namespace LiteTube.Core.Converters
{
    public class CommentMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                var isReplayComment = (bool)value;
                if (isReplayComment)
                    return new Thickness(50, 4, 0, 0);
            }

            return new Thickness(0, 4, 0, 0);
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
            return ConvertBack(value, targetType, parameter, culture.EnglishName);
        }
    }
}
