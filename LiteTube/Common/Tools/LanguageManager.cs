using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.Common.Tools
{
    class LanguageManager
    {
        private static readonly CultureInfo _ru = new CultureInfo("ru-RU");
        private static readonly CultureInfo _en = new CultureInfo("en-US");

        private static string RUSSIAN = _ru.DisplayName;
        private static string ENGLISH = _en.DisplayName;

        public static List<string> GetSupportedLanguages()
        {
            return new List<string> { ENGLISH, RUSSIAN };
        }

        public static void ChangeLanguage(string culture)
        {
            if (culture == RUSSIAN)
            {
                Thread.CurrentThread.CurrentUICulture = _ru;
                Thread.CurrentThread.CurrentCulture = _ru;
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = _en;
                Thread.CurrentThread.CurrentCulture = _en;
            }
        }
    }
}
