using LiteTube.Resources;
using LiteTube.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LiteTube.Converters
{
    public class LoginStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
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
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
