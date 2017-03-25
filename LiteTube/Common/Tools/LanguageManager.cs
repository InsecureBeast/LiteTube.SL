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
        private static readonly CultureInfo _ptBR = new CultureInfo("pt-BR");

        private static string RUSSIAN = _ru.DisplayName;
        private static string ENGLISH = _en.DisplayName;
        private static string PORTUGUESE_BR = _ptBR.DisplayName;

        public static CultureInfo Ru
        {
            get { return _ru; }
        }

        public static CultureInfo En
        {
            get { return _en; }
        }

        public static CultureInfo PtBr
        {
            get { return _ptBR; }
        }

        public static List<string> GetSupportedLanguages()
        {
            return new List<string> { ENGLISH, RUSSIAN, PORTUGUESE_BR };
        }

        public static void ChangeLanguage(string culture)
        {
            if (culture == RUSSIAN)
            {
                Thread.CurrentThread.CurrentUICulture = _ru;
                Thread.CurrentThread.CurrentCulture = _ru;
            }
            else if (culture == PORTUGUESE_BR)
            {
                Thread.CurrentThread.CurrentUICulture = _ptBR;
                Thread.CurrentThread.CurrentCulture = _ptBR;
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = _en;
                Thread.CurrentThread.CurrentCulture = _en;
            }
        }
    }
}
