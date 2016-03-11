using System.Windows;
using System.Windows.Input;
using LiteTube.Common.Helpers;

namespace LiteTube.Common
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
            
            element.Loaded += element_Loaded;
        }

        private static void element_Loaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            if (element == null)
                return;

            element.Loaded -= element_Loaded;

            var viewer = VisualHelper.GetScrollViewer(element);
            if (viewer == null)
                return;

            viewer.MouseMove += (o, args) =>
            {
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
