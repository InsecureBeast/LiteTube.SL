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
    public class BooleanToVisibilityConverter : IValueConverter
    {
        private Visibility _trueValue = Visibility.Visible;
        private Visibility _falseValue = Visibility.Collapsed;

        public Visibility TrueValue
        {
            get { return _trueValue; }
            set { _trueValue = value; }
        }

        public Visibility FalseValue
        {
            get { return _falseValue; }
            set { _falseValue = value; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture.EnglishName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetType, parameter, culture.EnglishName);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                return (bool)value ? TrueValue : FalseValue;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (Equals(value, TrueValue))
            {
                return true;
            }

            if (Equals(value, FalseValue))
            {
                return false;
            }

            return null;
        }
    }
}