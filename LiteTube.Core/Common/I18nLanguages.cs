using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteTube.Core.Common
{
    static class I18nLanguages
    {
        private static HashSet<Tuple<string, string, string>> _regions;

        static I18nLanguages()
        {
            _regions = new HashSet<Tuple<string, string, string>>();

            //CultureInfo.CurrentCulture.Name
            _regions.Add(new Tuple<string, string, string>("US", "en", "Worldwide"));
            _regions.Add(new Tuple<string, string, string>("DZ", "ar", "Algeria"));
            _regions.Add(new Tuple<string, string, string>("AR", "es-419", "Argentina"));
            _regions.Add(new Tuple<string, string, string>("AU", "en", "Australia"));
            _regions.Add(new Tuple<string, string, string>("AT", "de", "Austria"));
            _regions.Add(new Tuple<string, string, string>("BH", "ar", "Bahrain"));
            _regions.Add(new Tuple<string, string, string>("BE", "nl", "Belgium"));
            _regions.Add(new Tuple<string, string, string>("BA", "sr", "Bosnia and Herzegovina"));
            _regions.Add(new Tuple<string, string, string>("BR", "pt", "Brazil"));
            _regions.Add(new Tuple<string, string, string>("BG", "bg", "Bulgaria"));
            _regions.Add(new Tuple<string, string, string>("CA", "en", "Canada"));
            _regions.Add(new Tuple<string, string, string>("CA", "fr-CA", "Canada (fr)"));
            _regions.Add(new Tuple<string, string, string>("CL", "es-419", "Chile"));
            _regions.Add(new Tuple<string, string, string>("CO", "es-419", "Colombia"));
            _regions.Add(new Tuple<string, string, string>("HR", "hr", "Croatia"));
            //_regions.Add(new Tuple<string, string, string>("CZ", "sc", "Czech Republic"));
            _regions.Add(new Tuple<string, string, string>("DK", "da", "Denmark"));
            _regions.Add(new Tuple<string, string, string>("EG", "ar", "Egypt"));
            _regions.Add(new Tuple<string, string, string>("EE", "et", "Estonia"));
            _regions.Add(new Tuple<string, string, string>("FI", "fi", "Finland"));
            _regions.Add(new Tuple<string, string, string>("FR", "fr", "France"));
            _regions.Add(new Tuple<string, string, string>("DE", "de", "Germany"));
            _regions.Add(new Tuple<string, string, string>("GH", "af", "Ghana"));
            _regions.Add(new Tuple<string, string, string>("GR", "el", "Greece"));
            _regions.Add(new Tuple<string, string, string>("HK", "zh-HK", "Hong Kong"));
            _regions.Add(new Tuple<string, string, string>("HU", "hu", "Hungary"));
            _regions.Add(new Tuple<string, string, string>("IS", "is", "Iceland"));
            _regions.Add(new Tuple<string, string, string>("IN", "hi", "India"));
            _regions.Add(new Tuple<string, string, string>("ID", "id", "Indonesia"));
            _regions.Add(new Tuple<string, string, string>("IE", "en", "Ireland"));
            _regions.Add(new Tuple<string, string, string>("IL", "en", "Israel"));
            _regions.Add(new Tuple<string, string, string>("IT", "it", "Italy"));
            _regions.Add(new Tuple<string, string, string>("JP", "ja", "Japan"));
            _regions.Add(new Tuple<string, string, string>("JO", "ar", "Jordan"));
            _regions.Add(new Tuple<string, string, string>("KE", "af", "Kenya"));
            _regions.Add(new Tuple<string, string, string>("KW", "ar", "Kuwait"));
            _regions.Add(new Tuple<string, string, string>("LV", "lv", "Latvia"));
            _regions.Add(new Tuple<string, string, string>("LB", "ar", "Lebanon"));
            _regions.Add(new Tuple<string, string, string>("LY", "ar", "Libya"));
            _regions.Add(new Tuple<string, string, string>("LT", "lt", "Lithuania"));
            _regions.Add(new Tuple<string, string, string>("LU", "de", "Luxembourg"));
            _regions.Add(new Tuple<string, string, string>("MK", "mk", "Macedonia"));
            _regions.Add(new Tuple<string, string, string>("MY", "ms", "Malaysia"));
            _regions.Add(new Tuple<string, string, string>("MX", "es", "Mexico"));
            _regions.Add(new Tuple<string, string, string>("ME", "sr", "Montenegro"));
            _regions.Add(new Tuple<string, string, string>("MA", "ar", "Morocco"));
            _regions.Add(new Tuple<string, string, string>("NL", "nl", "Netherlands"));
            _regions.Add(new Tuple<string, string, string>("NZ", "en", "New Zealand"));
            _regions.Add(new Tuple<string, string, string>("NG", "af", "Nigeria"));
            _regions.Add(new Tuple<string, string, string>("NO", "no", "Norway"));
            _regions.Add(new Tuple<string, string, string>("OM", "ar", "Oman"));
            _regions.Add(new Tuple<string, string, string>("PE", "es-419", "Peru"));
            _regions.Add(new Tuple<string, string, string>("PH", "en", "Philippines"));
            _regions.Add(new Tuple<string, string, string>("PL", "pl", "Poland"));
            _regions.Add(new Tuple<string, string, string>("PT", "pt-PT", "Portugal"));
            _regions.Add(new Tuple<string, string, string>("PR", "es", "Puerto Rico"));
            _regions.Add(new Tuple<string, string, string>("QA", "ar", "Qatar"));
            _regions.Add(new Tuple<string, string, string>("RO", "ro", "Romania"));
            _regions.Add(new Tuple<string, string, string>("RU", "ru", "Russia"));
            _regions.Add(new Tuple<string, string, string>("SA", "ar", "Saudi Arabia"));
            _regions.Add(new Tuple<string, string, string>("SN", "en", "Senegal"));
            _regions.Add(new Tuple<string, string, string>("RS", "sr", "Serbia"));
            _regions.Add(new Tuple<string, string, string>("SG", "en", "Singapore"));
            _regions.Add(new Tuple<string, string, string>("SK", "sk", "Slovakia"));
            _regions.Add(new Tuple<string, string, string>("SI", "sl", "Slovenia"));
            _regions.Add(new Tuple<string, string, string>("ZA", "en", "South Africa"));
            _regions.Add(new Tuple<string, string, string>("KR", "ko", "South Korea"));
            _regions.Add(new Tuple<string, string, string>("ES", "es", "Spain"));
            _regions.Add(new Tuple<string, string, string>("SE", "sv", "Sweden"));
            _regions.Add(new Tuple<string, string, string>("CH", "de", "Switzerland (de)"));
            _regions.Add(new Tuple<string, string, string>("CH", "fr", "Switzerland (fr)"));
            _regions.Add(new Tuple<string, string, string>("TW", "zh-TW", "Taiwan"));
            _regions.Add(new Tuple<string, string, string>("TZ", "af", "Tanzania"));
            _regions.Add(new Tuple<string, string, string>("TH", "th", "Thailand"));
            _regions.Add(new Tuple<string, string, string>("TN", "ar", "Tunisia"));
            _regions.Add(new Tuple<string, string, string>("TR", "tr", "Turkey"));
            _regions.Add(new Tuple<string, string, string>("UG", "af", "Uganda"));
            _regions.Add(new Tuple<string, string, string>("UA", "uk", "Ukraine"));
            _regions.Add(new Tuple<string, string, string>("AE", "ar", "United Arab Emirates"));
            _regions.Add(new Tuple<string, string, string>("GB", "en-GB", "United Kingdom"));
            _regions.Add(new Tuple<string, string, string>("VN", "vi", "Vietnam"));
            _regions.Add(new Tuple<string, string, string>("YE", "ar", "Yemen"));
            _regions.Add(new Tuple<string, string, string>("ZW", "af", "Zimbabwe"));
        }

        internal static string GetRegionName(string region)
        {
            var code = _regions.FirstOrDefault(r => r.Item1 == region);
            if (code == null)
                return "Worldwide";

            return code.Item3;
        }

        public static string GetRegionCode(string regionCode)
        {
            var code = _regions.FirstOrDefault(r => r.Item1 == regionCode);
            if (code == null)
                return "US";
            
            return code.Item1;
        }

        public static string GetHl(string regionCode)
        {
            var code = _regions.FirstOrDefault(r => r.Item1 == regionCode);
            if (code == null)
                return "en";

            return code.Item2;
        }

        public static string CheckRegionName(string regionName)
        {
            var region = _regions.FirstOrDefault(r => r.Item3 == regionName);
            if (region == null)
                return "US";

            return region.Item1;
        }

        public static IEnumerable<string> Languages
        {
            get { return _regions.Select(r => r.Item3); }
        }
    }
}
