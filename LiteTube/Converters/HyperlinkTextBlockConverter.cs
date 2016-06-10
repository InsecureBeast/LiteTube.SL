using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Microsoft.Phone.Tasks;

namespace LiteTube.Converters
{
    public class HyperlinkTextBlockConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private void AddUrlToText(string text, TextBlock outputTextBlock)
        {
            while (true)
            {
                if (string.IsNullOrEmpty(text))
                    return;

                var indexOf = text.IndexOf("http", StringComparison.Ordinal);
                if (indexOf == -1)
                {
                    AddText(text, outputTextBlock);
                    return;
                }

                var str = text.Substring(indexOf);
                var urlEndIndex1 = str.IndexOf(" ", StringComparison.Ordinal);
                var urlEndIndex2 = str.IndexOf("\r\n", StringComparison.Ordinal);

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
                AddHyperlinkSafe(runText, url, outputTextBlock);
                text = str.Substring(endIndex, str.Length - endIndex);
            }
        }

        private void AddHyperlinkSafe(string textBefore, string url, TextBlock textBlock)
        {
            try
            {
                var run1 = new Run { Text = textBefore };
                textBlock.Inlines.Add(run1);

                if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute)) 
                    return;
                
                var run2 = new Run { Text = url };
                var hyperlink = new Hyperlink
                {
                    Inlines = { run2 },
                    NavigateUri = new Uri(url)
                };

                hyperlink.Click += HyperlinkOnClick;
                textBlock.Inlines.Add(hyperlink);
            }
            catch (Exception)
            {
                ;
            }
        }

        private void AddText(string text, TextBlock textBlock)
        {
            textBlock.Inlines.Add(new Run { Text = text});
        }

        private void HyperlinkOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var hyperlink = sender as Hyperlink;
            if (hyperlink == null)
                return;

            var videoId = GetVideoId(hyperlink.NavigateUri.AbsoluteUri);
            var channelId = GetChannelId(hyperlink.NavigateUri.AbsoluteUri);

            if (!string.IsNullOrEmpty(videoId))
            {
                MessageBox.Show("Run video page - " + videoId);
                return;
            }

            if (!string.IsNullOrEmpty(channelId))
            {
                MessageBox.Show("Run channel page - " + channelId);
                return;
            }

            var task = new WebBrowserTask
            {
                Uri = hyperlink.NavigateUri
            };
            task.Show();
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
    }
}
