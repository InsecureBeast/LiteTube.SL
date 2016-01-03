using Microsoft.Phone.Controls;
using System;
using System.Windows;

namespace LiteTube.Common
{
    /// <summary>
    /// This set of internal extension methods provide general solutions and 
    /// utilities in a small enough number to not warrant a dedicated extension
    /// methods class.
    /// </summary>
    internal static partial class Extensions
    {
        private const string ExternalAddress = "app://external/";

        /// <summary>
        /// An implementation of the Contains member of string that takes in a 
        /// string comparison. The traditional .NET string Contains member uses 
        /// StringComparison.Ordinal.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="value">The string value to search for.</param>
        /// <param name="comparison">The string comparison type.</param>
        /// <returns>Returns true when the substring is found.</returns>
        public static bool Contains(this string s, string value, StringComparison comparison)
        {
            return s.IndexOf(value, comparison) >= 0;
        }

        /// <summary>
        /// Returns whether the page orientation is in portrait.
        /// </summary>
        /// <param name="orientation">Page orientation</param>
        /// <returns>If the orientation is portrait</returns>
        public static bool IsPortrait(this PageOrientation orientation)
        {
            return (PageOrientation.Portrait == (PageOrientation.Portrait & orientation));
        }

        /// <summary>
        /// Returns whether the dark visual theme is currently active.
        /// </summary>
        /// <param name="resources">Resource Dictionary</param>
        public static bool IsDarkThemeActive(this ResourceDictionary resources)
        {
            return ((Visibility)resources["PhoneDarkThemeVisibility"] == Visibility.Visible);
        }

        /// <summary>
        /// Returns whether the uri is from an external source.
        /// </summary>
        /// <param name="uri">The uri</param>
        public static bool IsExternalNavigation(this Uri uri)
        {
            return (ExternalAddress == uri.ToString());
        }
    }
}
