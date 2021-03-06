﻿using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Windows.Storage;
using LiteTube.Common.Tools;

namespace LiteTube.Common.Helpers
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

            if (quality == "1080p")
            {
                SaveQuality("720p");
                return "720p";
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

        internal static bool GetIsAutoPlayVideo()
        {
            var isAutoplay = true;
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("IsAutoplayVideo"))
            {
                isAutoplay = (bool)(ApplicationData.Current.RoamingSettings.Values["IsAutoplayVideo"]);
            }

            return isAutoplay;
        }

        internal static void SaveAutoplayVideo(bool isAutoplayVideo)
        {
            ApplicationData.Current.RoamingSettings.Values["IsAutoplayVideo"] = isAutoplayVideo;
        }

        internal static void SaveIsRevokedAccessToken(bool isRevoked)
        {
            ApplicationData.Current.RoamingSettings.Values["IsRevokedAccessToken"] = isRevoked;
        }

        internal static bool GetIsRevokedAccessToken()
        {
            var isRevoked = false;
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("IsRevokedAccessToken"))
            {
                isRevoked = (bool)(ApplicationData.Current.RoamingSettings.Values["IsRevokedAccessToken"]);
            }

            return isRevoked;
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
                videoId = (string)(ApplicationData.Current.RoamingSettings.Values["VideoId"]);
            }

            return videoId;
        }

        internal static void SaveCurrentVideoId(string videoId)
        {
            ApplicationData.Current.RoamingSettings.Values["VideoId"] = videoId;
        }

        public static void ClearDeactivationSettings()
        {
#if SILVERLIGHT
            var settings = IsolatedStorageSettings.ApplicationSettings;
            RemoveValue("DeactivateTime");
            RemoveValue("SessionType");
            settings.Save();
#else
            throw new NotImplementedException();
#endif
        }

        public static void SaveDeactivateTime(DateTimeOffset dateTimeOffset)
        {
#if SILVERLIGHT
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (AddOrUpdateValue("DeactivateTime", dateTimeOffset))
            {
                settings.Save();
            }
#else
            throw new NotImplementedException();
#endif
        }

        public static void SaveSessionType(SessionType sessionType)
        {
#if SILVERLIGHT
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (AddOrUpdateValue("SessionType", sessionType))
            {
                settings.Save();
            }
#else
            throw new NotImplementedException();
#endif
        }

        public static DateTimeOffset GetDeactivateTime()
        {
#if SILVERLIGHT
            var deactivationTime = DateTimeOffset.Now; 
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("DeactivateTime"))
            {
                deactivationTime = (DateTimeOffset)settings["DeactivateTime"];
            }

            return deactivationTime;
#else
            throw new NotImplementedException();
#endif
        }

        public static SessionType GetSessionType()
        {
#if SILVERLIGHT
            var settings = IsolatedStorageSettings.ApplicationSettings;
            var sessionType = SessionType.None;
            if (settings.Contains("SessionType"))
            {
                sessionType = (SessionType)settings["SessionType"];
            }

            return sessionType;
#else
            throw new NotImplementedException();
#endif

        }

        //TODO for prov version set false
        public static bool IsAdvVisible
        {
            get { return true; }
        }

        internal static void SaveTheme(ApplicationTheme theme)
        {
            try
            {
                ApplicationData.Current.RoamingSettings.Values["Theme"] = (int)theme;
            }
            catch (Exception)
            {
            }
        }

        internal static void SaveLanguage(string culture)
        {
            try
            {
                ApplicationData.Current.RoamingSettings.Values["Culture"] = (string)culture;
            }
            catch (Exception)
            {
            }
        }

        internal static ApplicationTheme GetTheme()
        {
            var theme = ApplicationTheme.Light;
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Theme"))
            {
                theme = (ApplicationTheme)(int)(ApplicationData.Current.RoamingSettings.Values["Theme"]);
            }

            return theme;
        }

        internal static string GetLanguage()
        {
            var language = string.Empty;
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Culture"))
            {
                language = (string)(ApplicationData.Current.RoamingSettings.Values["Culture"]);
            }

            return language;
        }

        // Helper method for removing a key/value pair from isolated storage
        private static void RemoveValue(string Key)
        {
#if SILVERLIGHT
            var settings = IsolatedStorageSettings.ApplicationSettings;
            // If the key exists
            if (settings.Contains(Key))
            {
                settings.Remove(Key);
            }
#else
            throw new NotImplementedException();
#endif

        }

        // Helper method for adding or updating a key/value pair in isolated storage
        private static bool AddOrUpdateValue(string Key, Object value)
        {
#if SILVERLIGHT
            bool valueChanged = false;
            var settings = IsolatedStorageSettings.ApplicationSettings;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
#else
            throw new NotImplementedException();
#endif
        }

        internal static bool GetIsLargeItems()
        {
            var isLargeItems = true;
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("IsLargeItems"))
            {
                isLargeItems = (bool)(ApplicationData.Current.RoamingSettings.Values["IsLargeItems"]);
            }

            return isLargeItems;
        }

        internal static void SaveIsLargeItems(bool isLargeItems)
        {
            ApplicationData.Current.RoamingSettings.Values["IsLargeItems"] = isLargeItems;
        }
    }
}
