using System.Windows;
using System.Windows.Input;
using LiteTube.Core.Common.Helpers;

namespace LiteTube.Core.Common
{
    public static class ScrollViewerMonitor
    {
        public static readonly DependencyProperty LoadMoreCommandProperty =
            DependencyProperty.RegisterAttached("LoadMoreCommand", typeof(ICommand), typeof(ScrollViewerMonitor), new PropertyMetadata(null, OnCommandPropertyChanged));

        public static void SetLoadMoreCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(LoadMoreCommandProperty, value);
        }

        public static ICommand GetLoadMoreCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(LoadMoreCommandProperty);
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)d;
            if (element == null) 
                return;
            
            element.MouseMove += (o, args) => 
            {
                var viewer = VisualHelper.GetScrollViewer(element);
                if (viewer == null)
                    return;

                var command = GetLoadMoreCommand(element);
                if (command == null)
                    return;

                var progress = viewer.VerticalOffset / viewer.ScrollableHeight;
                if (progress >= 0.6)
                    command.Execute(null);
            };
        }
    }
}
