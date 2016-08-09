using System;
using System.Globalization;
using System.Windows.Data;
using LiteTube.Core.Resources;
using LiteTube.Core.ViewModels;
#if SILVERLIGHT

#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif

namespace LiteTube.Core.Converters
{
    public class LoginStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#if SILVERLIGHT
            var status = value as LoginStatus?;
            if (status == null)
                return string.Empty;

            if (status == LoginStatus.Logged)
                return string.Empty;

            if (status == LoginStatus.NotFound)
                return AppResources.ChannelNotFoundTooltip;

            if (status == LoginStatus.NotLogged)
                return AppResources.SignInTooltip;

            throw new NotSupportedException();
#endif
            throw new NotImplementedException();
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
