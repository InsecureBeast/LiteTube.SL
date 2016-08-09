using System;
using Microsoft.Phone.Shell;

namespace LiteTube.Core.Common.Helpers
{
    public static class ApplicationBarHelper
    {
        public static ApplicationBarIconButton CreateApplicationBarIconButton(string iconUri, string caption, EventHandler handler)
        {
            var button = new ApplicationBarIconButton
            {
                IconUri = new Uri(iconUri, UriKind.Relative),
                Text = caption
            };
            button.Click += handler;
            return button;
        }

        public static ApplicationBarMenuItem CreateAApplicationBarMenuItem(string caption, EventHandler handler)
        {
            var button = new ApplicationBarMenuItem
            {
                Text = caption
            };
            button.Click += handler;
            return button;
        }
    }
}
