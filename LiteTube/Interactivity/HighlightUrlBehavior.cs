using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using LiteTube.Common.Helpers;
using LiteTube.Common.Tools;
using Microsoft.Phone.Tasks;

namespace LiteTube.Interactivity
{
    public class HighlightUrlBehavior : Behavior<RichTextBox>, IAttachedObject
    {
        protected RichTextBox _richTextBox;

        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(HighlightUrlBehavior), new PropertyMetadata(false));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            _richTextBox = AssociatedObject as RichTextBox;
            if (_richTextBox == null)
                return;

            _richTextBox.Loaded += OnLoaded;
            _richTextBox.Tap += RichTextBoxOnTap;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            var fe = AssociatedObject as RichTextBox;
            if (fe == null)
                return;

            //fe.RemoveHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPointerPressed));
            fe.Loaded -= OnLoaded;
            fe.Tap -= RichTextBoxOnTap;
        }
        /*
        protected override Attach(DependencyObject dependencyObject)
        {
            if (dependencyObject != AssociatedObject)
            {
                if (AssociatedObject != null)
                    throw new InvalidOperationException("Cannot attach behavior multiple times.");

                AssociatedObject = dependencyObject;
                _richTextBox = AssociatedObject as RichTextBox;
                if (_richTextBox == null)
                    return;

                _richTextBox.Loaded += OnLoaded;
                _richTextBox.Tap += RichTextBoxOnTap;
            }
        }
        
        public void Detach()
        {
            var fe = AssociatedObject as RichTextBox;
            if (fe == null)
                return;

            //fe.RemoveHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPointerPressed));
            fe.Loaded -= OnLoaded;
            fe.Tap -= RichTextBoxOnTap;
            AssociatedObject = null;
        }
        */
        //public DependencyObject AssociatedObject { get; private set; }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var textBox = sender as RichTextBox;
            if (textBox == null)
                return;

            if (textBox.Tag == null)
                return;
            var paragraph = GetParagraph(textBox.Tag.ToString());
            textBox.Blocks.Clear();
            textBox.Blocks.Add(paragraph);
        }

        private void RichTextBoxOnTap(object sender, GestureEventArgs e)
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
            underline.Foreground = ThemeManager.AccentSolidColorBrush;
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
                    Foreground = ThemeManager.AccentDarkSolidColorBrush
                };
                /*
                var hyperlink = new Hyperlink
                {
                    Inlines = { run2 },
                    Command = new RelayCommand<string>(HyperlinkClick),
                    TextDecorations = null,
                    Foreground = ThemeManager.AccentDarkSolidColorBrush,
                    MouseOverForeground = ThemeManager.AccentSolidColorBrush,
                    CommandParameter = url
                };
                 */
                paragraph.Inlines.Add(run2);
            }
            catch (Exception)
            {
                ;
            }
        }

        private static void AddText(string text, Paragraph paragraph)
        {
            paragraph.Inlines.Add(new Run { Text = text });
        }

        private static void HyperlinkClick(string hyperlink)
        {
            if (hyperlink == null)
                return;

            //var inline = hyperlink.Inlines.FirstOrDefault();
            //if (inline == null)
            //    return;

            //var run = inline as Run;
            //if (run == null)
            //    return;

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
            var listIndex = url.LastIndexOf("list=", StringComparison.Ordinal);
            if (listIndex == -1)
                return string.Empty;

            return url.Substring(listIndex + 5);
        }
       
    }
}
