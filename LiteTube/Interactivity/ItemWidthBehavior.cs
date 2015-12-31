using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using LiteTube.Common;
using System;

namespace LiteTube.Interactivity
{
    class ItemWidthBehavior : DependencyObject, IAttachedObject
    {
        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ItemWidthBehavior), new PropertyMetadata(false));

        public static DependencyProperty AdditionalWidthProperty =
            DependencyProperty.Register("AdditionalWidth", typeof(bool), typeof(ItemWidthBehavior), new PropertyMetadata(false));


        protected FrameworkElement _frameworkElement;

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public bool AdditionalWidth
        {
            get { return (bool)GetValue(AdditionalWidthProperty); }
            set { SetValue(AdditionalWidthProperty, value); }
        }

        public DependencyObject AssociatedObject
        {
            get; private set;
        }

        public void Attach(DependencyObject associatedObject)
        {
            if ((associatedObject != AssociatedObject))
            {
                if (AssociatedObject != null)
                    throw new InvalidOperationException("Cannot attach behavior multiple times.");

                AssociatedObject = associatedObject;
                _frameworkElement = AssociatedObject as FrameworkElement;
                if (_frameworkElement == null)
                    return;

                _frameworkElement.Loaded += OnLoaded;
            }
        }

        public void Detach()
        {
            FrameworkElement fe = AssociatedObject as FrameworkElement;
            if (fe == null)
                return;

            var page = VisualHelper.FindParent<Page>(fe);
            if (page == null)
                return;

            page.SizeChanged -= OnPageSizeChanged;
            //fe.RemoveHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPointerPressed));
            fe.Loaded -= OnLoaded;
            AssociatedObject = null;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var item = sender as FrameworkElement;
            if (item == null)
                return;

            var page = VisualHelper.FindParent<Page>(item);
            if (page == null)
                return;

            page.SizeChanged += OnPageSizeChanged;
            SetItemSize();
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetItemSize();
        }

        protected virtual void SetItemSize()
        {
            _frameworkElement.Width = GetItemWidth();
        }

        private double GetItemWidth()
        {
            var width = App.RootFrame.ActualWidth;
            var actuaWidth = width - 100; //margin
            var itemWidth = 300.0;
            var itemsCount = actuaWidth / itemWidth;
            var rem = itemsCount - (int)itemsCount;
            if (rem <= 0.25)
                itemWidth = actuaWidth / ((int)itemsCount);
            else
                itemWidth = actuaWidth / ((int)itemsCount + 1);

            return itemWidth - 17;
        }
    }
}
