using System.Windows;
using System.Windows.Media;

namespace LiteTube.Common.Tools
{
    static class ThemeManager
    {
        public static void GoToLightTheme()
        {
            var accentBrush = Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
            if (accentBrush != null)
            {
                var accentColor = accentBrush.Color;
                var accentLightColor = accentColor.Lerp(Colors.White, 0.25f);
                var accentDarkColor = accentColor.Lerp(Colors.Black, 0.25f);

                var accentDarkBrush = Application.Current.Resources["PhoneDarkAccentBrush"] as SolidColorBrush;
                if (accentDarkBrush != null)
                    accentDarkBrush.Color = accentDarkColor;

                var accentLightBrush = Application.Current.Resources["PhoneDarkAccentBrush"] as SolidColorBrush;
                if (accentLightBrush != null)
                    accentLightBrush.Color = accentLightColor;

                var backgroundBrush = Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;
                if (backgroundBrush != null)
                    backgroundBrush.Color = Color.FromArgb(255, 241, 240, 238); //#FFF1F0EE

                var mainForegroundBrush = Application.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush;
                if (mainForegroundBrush != null)
                    mainForegroundBrush.Color = Colors.Black;

                var contentForegroundBrush = Application.Current.Resources["PhoneSubtleBrush"] as SolidColorBrush;
                if (contentForegroundBrush != null)
                    contentForegroundBrush.Color = Color.FromArgb(255, 93, 93, 93);

                var secondaryBackgroundBrush = Application.Current.Resources["PhoneSecondaryBackgroundBrush"] as SolidColorBrush;
                if (secondaryBackgroundBrush != null)
                    secondaryBackgroundBrush.Color = Color.FromArgb(255, 230, 231, 232);

                var disabledBackgroundBrush = Application.Current.Resources["PhoneDisabledBrush"] as SolidColorBrush;
                if (disabledBackgroundBrush != null)
                    disabledBackgroundBrush.Color = Color.FromArgb(255, 165, 165, 165);

                var phoneBorderBrush = Application.Current.Resources["PhoneBorderBrush"] as SolidColorBrush;
                if (phoneBorderBrush != null)
                    phoneBorderBrush.Color = Color.FromArgb(178, 0, 0, 0); //75% of black
            }
        }

        public static void GoToDarkTheme()
        {
            var accentBrush = Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
            if (accentBrush != null)
            {
                var accentColor = accentBrush.Color;
                var accentLightColor = accentColor.Lerp(Colors.White, 0.25f);
                var accentDarkColor = accentColor.Lerp(Colors.Black, 0.25f);

                var accentDarkBrush = Application.Current.Resources["PhoneDarkAccentBrush"] as SolidColorBrush;
                if (accentDarkBrush != null)
                    accentDarkBrush.Color = accentDarkColor;

                var accentLightBrush = Application.Current.Resources["PhoneDarkAccentBrush"] as SolidColorBrush;
                if (accentLightBrush != null)
                    accentLightBrush.Color = accentLightColor;

                var backgroundBrush = Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;
                if (backgroundBrush != null)
                    backgroundBrush.Color = Colors.Black;

                var mainForegroundBrush = Application.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush;
                if (mainForegroundBrush != null)
                    mainForegroundBrush.Color = Colors.White;

                var contentForegroundBrush = Application.Current.Resources["PhoneSubtleBrush"] as SolidColorBrush;
                if (contentForegroundBrush != null)
                    contentForegroundBrush.Color = Color.FromArgb(153, 255, 255, 255); //#99FFFFFF

                var secondaryBackgroundBrush = Application.Current.Resources["PhoneSecondaryBackgroundBrush"] as SolidColorBrush;
                if (secondaryBackgroundBrush != null)
                    secondaryBackgroundBrush.Color = Color.FromArgb(255/2, 0, 0, 0);

                var disabledBackgroundBrush = Application.Current.Resources["PhoneDisabledBrush"] as SolidColorBrush;
                if (disabledBackgroundBrush != null)
                    disabledBackgroundBrush.Color = Color.FromArgb(255, 165, 165, 165);

                var phoneBorderBrush = Application.Current.Resources["PhoneBorderBrush"] as SolidColorBrush;
                if (phoneBorderBrush != null)
                    phoneBorderBrush.Color = Color.FromArgb(178, 255, 255, 255); //75% of white
            }
        }
    }
}
