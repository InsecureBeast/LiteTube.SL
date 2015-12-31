using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LiteTube.Converters
{
    public class CountToVisibilityConverter : IValueConverter
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
            var val = value as IList;
            if (val == null)
                return Visibility.Collapsed;

            return val.Count > 0 ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
