using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Phone.Tasks;
using MyToolkit.Command;
using LiteTube.Common.Helpers;
using LiteTube.Common.Tools;

namespace LiteTube.Tools
{
    public static class HyperlinkHighlighter
    {
        public static void HighlightUrls(string text, RichTextBox textBlock)
        {
            var paragraph = GetParagraph(text, textBlock);
            textBlock.Blocks.Clear();
            textBlock.Blocks.Add(paragraph);
        }

        private static Paragraph GetParagraph(string text, RichTextBox textBlock)
        {
            var paragraph = new Paragraph();
            while (true)
            {
                if (string.IsNullOrEmpty(text))
                    return paragraph;

                var indexOf = text.IndexOf("http", StringComparison.Ordinal);
                if (indexOf == -1)
                {
                    AddText(text, paragraph);
                    return paragraph;
                }

                var str = text.Substring(indexOf);
                var urlEndIndex1 = str.IndexOf(" ", StringComparison.Ordinal);
                var urlEndIndex2 = str.IndexOf("\r\n", StringComparison.Ordinal);
                if (urlEndIndex2 == -1)
                    urlEndIndex2 = str.IndexOf("\n", StringComparison.Ordinal);
                if (urlEndIndex2 == -1)
                    urlEndIndex2 = str.IndexOf("\r", StringComparison.Ordinal);

                var endIndex = 0;
                if (urlEndIndex1 == -1)
                    endIndex = urlEndIndex2;
                if (urlEndIndex2 == -1)
                    endIndex = urlEndIndex1;
                if (urlEndIndex1 != -1 && urlEndIndex2 != -1)
                    endIndex = Math.Min(urlEndIndex1, urlEndIndex2);
                if (urlEndIndex1 == -1 && urlEndIndex2 == -1)
                    endIndex = text.Length - indexOf;

                var url = str.Substring(0, endIndex);
                var runText = text.Substring(0, indexOf);
                AddHyperlinkSafe(runText, url, paragraph);
                text = str.Substring(endIndex, str.Length - endIndex);
            }
        }

        private static void AddHyperlinkSafe(string textBefore, string url, Paragraph paragraph)
        {
            try
            {
                var run1 = new Run { Text = textBefore };
                paragraph.Inlines.Add(run1);

                if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                {
                    AddText(url, paragraph);
                    return;
                }

                var run2 = new Run { Text = url };
                var hyperlink = new Hyperlink
                {
                    Inlines = { run2 }, 
                    Command = new RelayCommand<Hyperlink>(HyperlinkClick),
                    TextDecorations = null,
                    Foreground = ThemeManager.AccentDarkSolidColorBrush,
                    MouseOverForeground = ThemeManager.AccentSolidColorBrush,
                };
                hyperlink.CommandParameter = hyperlink;
                paragraph.Inlines.Add(hyperlink);
            }
            catch (Exception)
            {
                ;
            }
        }

        private static void HyperlinkClick(Hyperlink hyperlink)
        {
            if (hyperlink == null)
                return;

            var inline = hyperlink.Inlines.FirstOrDefault();
            if (inline == null)
                return;

            var run = inline as Run;
            if (run == null)
                return;

            var videoId = GetVideoId(run.Text);
            var channelId = GetChannelId(run.Text);
            var playlistId = GetPlaylistId(run.Text);
            //var username = GetChannelName(run.Text);

            if (!string.IsNullOrEmpty(videoId))
            {
#if SILVERLIGHT
                NavigationHelper.GoToVideoPage(videoId);
#endif
                return;
            }

            if (!string.IsNullOrEmpty(channelId))
            {
#if SILVERLIGHT
                NavigationHelper.GoToChannelPage(channelId, null);
#endif
                return;
            }

//            if (!string.IsNullOrEmpty(username))
//            {
//#if SILVERLIGHT
//                NavigationHelper.GoToChannelPage(null, username);
//#endif
//                return;
//            }

            if (!string.IsNullOrEmpty(playlistId))
            {
#if SILVERLIGHT
                NavigationHelper.GoToPLaylistPage(playlistId);
#endif
                return;
            }

            var task = new WebBrowserTask
            {
                Uri = new Uri(run.Text)
            };
            task.Show();
        }

        private static void AddText(string text, Paragraph paragraph)
        {
            paragraph.Inlines.Add(new Run { Text = text});
        }

        private static string GetVideoId(string url)
        {
            var videoId = string.Empty;
            var regex = new Regex(@"watch\?v=(.*)");
            var colm = regex.Matches(url);
            foreach (Match match in colm)
            {
                videoId = match.Value.Substring(8, match.Value.Length - 8);
                return videoId;
            }
            return videoId;
        }

        private static string GetChannelId(string url)
        {
            var channelId = string.Empty;
            var regex = new Regex(@"channel/(.*)");
            var colm = regex.Matches(url);
            foreach (Match match in colm)
            {
                channelId = match.Value.Substring(8, match.Value.Length - 8);
                return channelId;
            }

            return channelId;
        }

        private static string GetChannelName(string url)
        {
            var username = string.Empty;
            var regex = new Regex(@"user/(.*)");
            var colm = regex.Matches(url);
            foreach (Match match in colm)
            {
                username = match.Value.Substring(5, match.Value.Length - 5);
                return username;
            }

            return username;
        }

        private static string GetPlaylistId(string url)
        {
            var listIndex = url.LastIndexOf("list=");
            if (listIndex == -1)
                return string.Empty;

            return url.Substring(listIndex + 5);
        }
    }
}
