using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace LiteTube.Common
{
    static class SettingsHelper
    {
        internal static string GetRegion()
        {
            var region = "US";
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Region"))
            {
                region = (string)(ApplicationData.Current.RoamingSettings.Values["Region"]);
            }

            return region;
        }

        internal static string GetRegionName()
        {
            var region = GetRegion();
            return I18nLanguages.GetRegionName(region);
        }

        internal static string SaveRegion(string regionName)
        {
            var region = I18nLanguages.CheckRegionName(regionName);
            ApplicationData.Current.RoamingSettings.Values["Region"] = region;
            return region;
        }

        internal static void SaveHistory(Dictionary<string, DateTime> _history)
        {
            var dic = JsonConvert.SerializeObject(_history);
            //ApplicationData.Current.LocalSettings.Values["History"] = dic;
        }

        internal static Dictionary<string, DateTime> LoadHistory()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("History"))
            {
                var str = (string)(ApplicationData.Current.LocalSettings.Values["History"]);
                if (!string.IsNullOrEmpty(str))
                {
                    var jObject = (JObject)JsonConvert.DeserializeObject(str);
                    var history = jObject.ToObject(typeof(Dictionary<string, DateTime>));
                    return history as Dictionary<string, DateTime>;
                }
            }

            return new Dictionary<string, DateTime>();
        }

        internal static void SaveQuality(string qualityName)
        {
            ApplicationData.Current.RoamingSettings.Values["Quality"] = qualityName;
        }

        internal static string GetQuality()
        {
            var quality = VideoQuality.DEFAULT_QUALITY_NAME;
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Quality"))
            {
                quality = (string)(ApplicationData.Current.RoamingSettings.Values["Quality"]);
            }

            return quality;
        }

        internal static void SaveUserRefreshToken(string refreshToken)
        {
            ApplicationData.Current.RoamingSettings.Values["RefreshToken"] = refreshToken;
        }

        internal static string GetRefreshToken()
        {
            var token = string.Empty;
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("RefreshToken"))
            {
                token = (string)(ApplicationData.Current.RoamingSettings.Values["RefreshToken"]);
            }

            return token;
        }

        internal static void SaveUserAccessToken(string accessToken)
        {
            ApplicationData.Current.RoamingSettings.Values["AccessToken"] = accessToken;
        }

        internal static string GetAccessToken()
        {
            var token = string.Empty;
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("AccessToken"))
            {
                token = (string)(ApplicationData.Current.RoamingSettings.Values["AccessToken"]);
            }

            return token;
        }

        internal static bool IsContainsAuthorizationData()
        {
            return !string.IsNullOrEmpty(GetAccessToken());
        }

        internal static string GetCurrentVideoId()
        {
            var videoId = string.Empty;
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("VideoId"))
            {
                videoId = (string)(ApplicationData.Current.RoamingSettings.Values["Region"]);
            }

            return videoId;
        }

        internal static void SaveCurrentVideoId(string videoId)
        {
            ApplicationData.Current.RoamingSettings.Values["VideoId"] = videoId;
        }
    }
}
