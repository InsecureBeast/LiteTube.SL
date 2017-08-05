using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using LiteTube.Common.Helpers;
using LiteTube.Common.Tools;
using LiteTube.Tools;
using Microsoft.Phone.Tasks;

namespace LiteTube.Interactivity
{
    public class HighlightUrlBehavior
    {
        protected RichTextBox _richTextBox;

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(HighlightUrlBehavior), new PropertyMetadata(false));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(HighlightUrlBehavior), new PropertyMetadata(string.Empty, TextChanged));

        public static void SetIsEnabled(DependencyObject d, bool value)
        {
            d.SetValue(IsEnabledProperty, value);
        }

        public static bool GetCommand(DependencyObject d)
        {
            return (bool)d.GetValue(IsEnabledProperty);
        }

        public static void SetText(DependencyObject d, string value)
        {
            d.SetValue(TextProperty, value);
        }

        public static string GetText(DependencyObject d)
        {
            return (string)d.GetValue(TextProperty);
        }

        private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextBox = d as RichTextBox;
            if (richTextBox == null)
                return;

            if (e.NewValue == null)
                return;

            HyperlinkHighlighter.HighlightUrls(e.NewValue.ToString(), richTextBox);
        }
    }
}
