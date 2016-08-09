using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using LiteTube.Core.Common.Helpers;
using Microsoft.Phone.Tasks;

namespace LiteTube.Core.Common.Tools
{
    public static class HyperlinkHighlighter
    {
        public static void HighlightUrls(string text, RichTextBox textBlock)
        {
            var paragraph = GetParagraph(text);
            textBlock.Blocks.Clear();
            textBlock.Blocks.Add(paragraph);
            textBlock.Tap += TextBlockOnTap;
        }

        private static void TextBlockOnTap(object sender, GestureEventArgs e)
        {
            var richTb = sender as RichTextBox;
            if (richTb == null)
                return;

            var textPointer = richTb.GetPositionFromPoint(e.GetPosition(richTb));

            var element = textPointer.Parent as TextElement;
            while (element != null && !(element is Run))
            {
                if (element.ContentStart != null
                    && element != element.ElementStart.Parent)
                {
                    element = element.ElementStart.Parent as TextElement;
                }
                else
                {
                    element = null;
                }
            }

            if (element == null) 
                return;

            var underline = element as Run;
            if (underline == null)
                return;

            underline.Foreground = ThemeManager.AccentDarkSolidColorBrush;
            HyperlinkClick(underline.Text);
        }

        private static Paragraph GetParagraph(string text)
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

                var run2 = new Run
                {
                    Text = url,
                    Foreground = ThemeManager.AccentSolidColorBrush
                };
                paragraph.Inlines.Add(run2);
            }
            catch (Exception)
            {
                ;
            }
        }

        private static void HyperlinkClick(string hyperlink)
        {
            if (string.IsNullOrEmpty(hyperlink))
                return;

            if (!Uri.IsWellFormedUriString(hyperlink, UriKind.Absolute))
                return;

            var videoId = GetVideoId(hyperlink);
            var channelId = GetChannelId(hyperlink);
            var playlistId = GetPlaylistId(hyperlink);
            //var username = GetChannelName(hyperlink);

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
                Uri = new Uri(hyperlink)
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
                var ampIndex = videoId.IndexOf("&");
                if (ampIndex == -1)
                    return videoId;

                var v = videoId.Substring(0, ampIndex);
                return v;
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
            var listIndex = url.LastIndexOf("list=", StringComparison.Ordinal);
            return listIndex == -1 ? string.Empty : url.Substring(listIndex + 5);
        }
    }
}
