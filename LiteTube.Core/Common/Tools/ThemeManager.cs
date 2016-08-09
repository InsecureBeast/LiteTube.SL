using System.Windows;
using System.Windows.Media;

namespace LiteTube.Core.Common.Tools
{
    public static class ThemeManager
    {
        private static SolidColorBrush _accentSolidColorBrush;
        private static SolidColorBrush _accentDarkSolidColorBrush;
        private static SolidColorBrush _accentLightSolidColorBrush;
        private static SolidColorBrush _inverseForegroundBrush;

        private static void GoToLightTheme()
        {
            _accentSolidColorBrush = Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
            if (_accentSolidColorBrush != null)
            {
                var accentColor = _accentSolidColorBrush.Color;
                var accentLightColor = accentColor.Lerp(Colors.White, 0.25f);
                var accentDarkColor = accentColor.Lerp(Colors.Black, 0.25f);

                _accentDarkSolidColorBrush = Application.Current.Resources["PhoneDarkAccentBrush"] as SolidColorBrush;
                if (_accentDarkSolidColorBrush != null)
                    _accentDarkSolidColorBrush.Color = accentDarkColor;

                _accentLightSolidColorBrush = Application.Current.Resources["PhoneLightAccentBrush"] as SolidColorBrush;
                if (_accentLightSolidColorBrush != null)
                    _accentLightSolidColorBrush.Color = accentLightColor;
            }
            
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

            var phoneSecondaryForegroundBrush = Application.Current.Resources["PhoneSecondaryForegroundBrush"] as SolidColorBrush;
            if (phoneSecondaryForegroundBrush != null)
                phoneSecondaryForegroundBrush.Color = Color.FromArgb(255, 102, 102, 102);

            var disabledBackgroundBrush = Application.Current.Resources["PhoneDisabledBrush"] as SolidColorBrush;
            if (disabledBackgroundBrush != null)
                disabledBackgroundBrush.Color = Color.FromArgb(255, 165, 165, 165);

            var phoneBorderBrush = Application.Current.Resources["PhoneBorderBrush"] as SolidColorBrush;
            if (phoneBorderBrush != null)
                phoneBorderBrush.Color = Color.FromArgb(178, 0, 0, 0); //75% of black

            var playlistSelectedBrush = Application.Current.Resources["PlaylistSelectedBrush"] as SolidColorBrush;
            if (playlistSelectedBrush != null)
                playlistSelectedBrush.Color = Color.FromArgb(255, 200, 200, 200);

            Visibility darkBackgroundVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
            if (darkBackgroundVisibility == Visibility.Visible)
            {
                //Theme is Dark
                _inverseForegroundBrush = Application.Current.Resources["InverseForegroundBrush"] as SolidColorBrush;
                if (_inverseForegroundBrush != null)
                    _inverseForegroundBrush.Color = Color.FromArgb(255, 255, 255, 255);
            }
            else
            {
                //Theme is Light
                _inverseForegroundBrush = Application.Current.Resources["InverseForegroundBrush"] as SolidColorBrush;
                if (_inverseForegroundBrush != null)
                    _inverseForegroundBrush.Color = Color.FromArgb(255, 0, 0, 0);
            }
        }

        public static SolidColorBrush AccentSolidColorBrush
        {
            get { return _accentSolidColorBrush; }
        }

        public static SolidColorBrush AccentDarkSolidColorBrush
        {
            get { return _accentDarkSolidColorBrush; }
        }

        public static SolidColorBrush AccentLightSolidColorBrush
        {
            get { return _accentLightSolidColorBrush; }
        }

        public static SolidColorBrush InverseForegroundBrush
        {
            get { return _inverseForegroundBrush; }
        }

        private static void GoToDarkTheme()
        {
            _accentSolidColorBrush = Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
            if (_accentSolidColorBrush != null)
            {
                var accentColor = _accentSolidColorBrush.Color;
                var accentLightColor = accentColor.Lerp(Colors.White, 0.25f);
                var accentDarkColor = accentColor.Lerp(Colors.Black, 0.25f);

                _accentDarkSolidColorBrush = Application.Current.Resources["PhoneDarkAccentBrush"] as SolidColorBrush;
                if (_accentDarkSolidColorBrush != null)
                    _accentDarkSolidColorBrush.Color = accentDarkColor;

                _accentLightSolidColorBrush = Application.Current.Resources["PhoneLightAccentBrush"] as SolidColorBrush;
                if (_accentLightSolidColorBrush != null)
                    _accentLightSolidColorBrush.Color = accentLightColor;
            }
            
            var backgroundBrush = Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;
            if (backgroundBrush != null)
                backgroundBrush.Color = Colors.Black;

            var mainForegroundBrush = Application.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush;
            if (mainForegroundBrush != null)
                mainForegroundBrush.Color = Colors.White;

            var contentForegroundBrush = Application.Current.Resources["PhoneSubtleBrush"] as SolidColorBrush;
            if (contentForegroundBrush != null)
                contentForegroundBrush.Color = Color.FromArgb(153, 255, 255, 255); //60% white

            var secondaryBackgroundBrush = Application.Current.Resources["PhoneSecondaryBackgroundBrush"] as SolidColorBrush;
            if (secondaryBackgroundBrush != null)
                secondaryBackgroundBrush.Color = Color.FromArgb(25, 255, 255, 255);

            var disabledBackgroundBrush = Application.Current.Resources["PhoneDisabledBrush"] as SolidColorBrush;
            if (disabledBackgroundBrush != null)
                disabledBackgroundBrush.Color = Color.FromArgb(255, 165, 165, 165);

            var phoneBorderBrush = Application.Current.Resources["PhoneBorderBrush"] as SolidColorBrush;
            if (phoneBorderBrush != null)
                phoneBorderBrush.Color = Color.FromArgb(178, 255, 255, 255); //75% of white

            var phoneSecondaryForegroundBrush = Application.Current.Resources["PhoneSecondaryForegroundBrush"] as SolidColorBrush;
            if (phoneSecondaryForegroundBrush != null)
                phoneSecondaryForegroundBrush.Color = Color.FromArgb(255, 255, 255, 255);

            var playlistSelectedBrush = Application.Current.Resources["PlaylistSelectedBrush"] as SolidColorBrush;
            if (playlistSelectedBrush != null)
                playlistSelectedBrush.Color = Color.FromArgb(255, 58, 58, 58);

            Visibility darkBackgroundVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
            if (darkBackgroundVisibility == Visibility.Visible)
            {
                //Theme is Dark
                _inverseForegroundBrush = Application.Current.Resources["InverseForegroundBrush"] as SolidColorBrush;
                if (_inverseForegroundBrush != null)
                    _inverseForegroundBrush.Color = Color.FromArgb(255, 255, 255, 255);
            }
            else
            {
                //Theme is Light
                _inverseForegroundBrush = Application.Current.Resources["InverseForegroundBrush"] as SolidColorBrush;
                if (_inverseForegroundBrush != null)
                    _inverseForegroundBrush.Color = Color.FromArgb(255, 0, 0, 0);
            }
        }

        public static void SetApplicationTheme(ApplicationTheme theme)
        {
            if (theme == ApplicationTheme.Dark)
                GoToDarkTheme();
            else
                GoToLightTheme();
        }
    }
}
