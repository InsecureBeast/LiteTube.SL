using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using LiteTube.Common;
using System;
using System.Windows.Input;

namespace LiteTube.Interactivity
{
    class LoadItemsByScrollBehavior : Behavior<Control>
    {

        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(LoadItemsByScrollBehavior), new PropertyMetadata(false));

        public static DependencyProperty LoadMoreCommandProperty =
            DependencyProperty.Register("LoadMoreCommand", typeof(ICommand), typeof(LoadItemsByScrollBehavior), new PropertyMetadata(null));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public ICommand LoadMoreCommand
        {
            get { return (ICommand)GetValue(LoadMoreCommandProperty); }
            set { SetValue(LoadMoreCommandProperty, value); }
        }

        protected override void OnAttached()
        {
            if (!IsEnabled)
                return;

            if (AssociatedObject != null)
                throw new InvalidOperationException("Cannot attach behavior multiple times.");

            var frameworkElement = AssociatedObject as FrameworkElement;
            if (frameworkElement == null)
                return;

            frameworkElement.Loaded += OnLoaded;
        }

        //void Attach(DependencyObject associatedObject)
        //{
        //    if (!IsEnabled)
        //        return;

        //    if ((associatedObject != AssociatedObject))
        //    {
        //        if (AssociatedObject != null)
        //            throw new InvalidOperationException("Cannot attach behavior multiple times.");

        //        //AssociatedObject = associatedObject;
        //        var frameworkElement = AssociatedObject as FrameworkElement;
        //        if (frameworkElement == null)
        //            return;

        //        frameworkElement.Loaded += OnLoaded;
        //    }
        //}

        //public void Detach()
        //{
        //    //AssociatedObject = null;
        //}

        //private void MainPageViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        //{
        //    var view = (ScrollViewer)sender;
        //    var progress = view.VerticalOffset / view.ScrollableHeight;
        //    if (progress > 0.7) // && !endoflist)
        //        LoadMoreCommand.Execute(null);
        //}

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var viewer = VisualHelper.GetScrollViewer(AssociatedObject);
            if (viewer == null)
                return;

            //viewer.ViewChanged += MainPageViewChanged;
            LoadMoreCommand.Execute(null);
        }
    }
}
