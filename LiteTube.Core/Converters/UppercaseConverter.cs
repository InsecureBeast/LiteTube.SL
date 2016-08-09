﻿using System;
using System.Globalization;
using System.Windows.Data;
#if SILVERLIGHT

#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif

namespace LiteTube.Core.Converters
{
    public class UppercaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = value as string;
            if (string.IsNullOrEmpty(str))
                return value;

            return str.ToUpper();
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
