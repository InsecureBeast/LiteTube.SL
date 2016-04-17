using LiteTube.Resources;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LiteTube.Converters
{
    public class DateTimeToDisplayStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            if (value is DateTime)
            {
                var date = (DateTime)value;
                var now = DateTime.Now.Date;

                if (date.Date == now)
                    return AppResources.Today;

                if (date.Date == now - TimeSpan.FromDays(1))
                    return AppResources.Yesterday;

                if (date.Date == now - TimeSpan.FromDays(2))
                    return string.Format(AppResources.DaysAgo, 2);

                if (date.Date == now - TimeSpan.FromDays(3))
                    return string.Format(AppResources.DaysAgo, 3);

                if (date.Date == now - TimeSpan.FromDays(4))
                    return string.Format(AppResources.DaysAgo, 4);

                if (date.Date == now - TimeSpan.FromDays(5))
                    return string.Format(AppResources.Days5Ago, 5);

                if (date.Date <= now - TimeSpan.FromDays(6) && date.Date >= now - TimeSpan.FromDays(7))
                    return AppResources.WeekAgo;

                if (date.Date < now - TimeSpan.FromDays(7) && date.Date >= now - TimeSpan.FromDays(14))
                    return string.Format(AppResources.WeeksAgo, 2);

                if (date.Date < now - TimeSpan.FromDays(14) && date.Date >= now - TimeSpan.FromDays(21))
                    return string.Format(AppResources.WeeksAgo, 3);

                if (date.Date < now - TimeSpan.FromDays(21) && date.Date >= now - TimeSpan.FromDays(28))
                    return AppResources.MonthAgo;

                if (date.Date < now.AddMonths(-1) && date.Date >= now.AddMonths(-2))
                    return AppResources.MonthAgo;

                if (date.Date < now.AddMonths(-2) && date.Date >= now.AddMonths(-3))
                    return string.Format(AppResources.Months4Ago, 2);

                if (date.Date < now.AddMonths(-3) && date.Date >= now.AddMonths(-4))
                    return string.Format(AppResources.Months4Ago, 3);

                if (date.Date < now.AddMonths(-4) && date.Date >= now.AddMonths(-5))
                    return string.Format(AppResources.Months4Ago, 4);

                if (date.Date < now.AddMonths(-5) && date.Date >= now.AddMonths(-6))
                    return string.Format(AppResources.MonthsAgo, 5);

                if (date.Date < now.AddMonths(-6) && date.Date >= now.AddMonths(-7))
                    return string.Format(AppResources.MonthsAgo, 6);

                if (date.Date < now.AddMonths(-7) && date.Date >= now.AddMonths(-8))
                    return string.Format(AppResources.MonthsAgo, 7);

                if (date.Date < now.AddMonths(-8) && date.Date >= now.AddMonths(-9))
                    return string.Format(AppResources.MonthsAgo, 8);

                if (date.Date < now.AddMonths(-9) && date.Date >= now.AddMonths(-10))
                    return string.Format(AppResources.MonthsAgo, 9);

                if (date.Date < now.AddMonths(-10) && date.Date >= now.AddMonths(-11))
                    return string.Format(AppResources.MonthsAgo, 10);

                if (date.Date < now.AddMonths(-11) && date.Date >= now.AddMonths(-12))
                    return string.Format(AppResources.MonthsAgo, 11);

                if (date.Date <= now.AddYears(-1) && date.Date >= now.AddYears(-2))
                    return AppResources.YearAgo;

                if (date.Date < now.AddYears(-2) && date.Date >= now.AddYears(-3))
                    return string.Format(AppResources.Years4Ago, 2);

                if (date.Date < now.AddYears(-3) && date.Date >= now.AddYears(-4))
                    return string.Format(AppResources.Years4Ago, 3);

                if (date.Date < now.AddYears(-4) && date.Date >= now.AddYears(-5))
                    return string.Format(AppResources.Years4Ago, 4);

                if (date.Date < now.AddYears(-5) && date.Date >= now.AddYears(-6))
                    return string.Format(AppResources.YearsAgo, 5);

                if (date.Date < now.AddYears(-6) && date.Date >= now.AddYears(-7))
                    return string.Format(AppResources.YearsAgo, 6);

                if (date.Date < now.AddYears(-7) && date.Date >= now.AddYears(-8))
                    return string.Format(AppResources.YearsAgo, 7);

                if (date.Date < now.AddYears(-8) && date.Date >= now.AddYears(-9))
                    return string.Format(AppResources.YearsAgo, 8);

                if (date.Date < now.AddYears(-9) && date.Date >= now.AddYears(-10))
                    return string.Format(AppResources.YearsAgo, 9);

                if (date.Date < now.AddYears(-10) && date.Date >= now.AddYears(-11))
                    return string.Format(AppResources.YearsAgo, 10);

                return date.ToString("d", CultureInfo.CurrentCulture);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }

        private bool OnThisWeek(DateTime date)
        {
            var now = DateTime.Today;
            var today = (int)DateTime.Today.DayOfWeek;
            if (date.Date < now - TimeSpan.FromDays(today) &&  date.Date >= now - TimeSpan.FromDays(6 - today))
                return true;

            return false;
        }
    }
}
