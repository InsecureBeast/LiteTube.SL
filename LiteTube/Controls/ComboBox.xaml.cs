using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Input;
using LiteTube.Common.Helpers;

namespace LiteTube.Controls
{
    public partial class ComboBox : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ComboBox), null);
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItemSource", typeof(object), typeof(ComboBox), null);

        public ComboBox()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public IEnumerable ItemsSource
        {
            get { return GetValue(ItemsSourceProperty) as IEnumerable; }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty) as object; }
            set { SetValue(SelectedItemProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            DropDownList.MinWidth = availableSize.Width;
            return base.MeasureOverride(availableSize);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var page = VisualHelper.FindParent<Page>(this);
            if (page == null)
                return;

            page.MouseLeftButtonDown += Page_MouseLeftButtonDown;
            DropDownList.ItemsSource = ItemsSource;

            if (DropDownList.Items.Count == 0)
                return;

            //var firstItem = DropDownList.Items.FirstOrDefault();
            //if (firstItem == null)
            //    return;

            //if (DropDownList.SelectedItem == null)
            //{
            //    DropDownList.SelectedItem = firstItem.ToString();
            //    SelectedValueTextBlock.Text = firstItem.ToString();
            //    return;
            //}
            if (SelectedItem == null)
                return;

            SelectedValueTextBlock.Text = SelectedItem.ToString();
            DropDownList.SelectedItem = SelectedItem;
        }

        private void Page_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void DropDownList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // при выборе элемента из списка - устанавливаем выбранный элемент и сворачиваем список
            SelectedItem = DropDownList.SelectedItem;
            SelectedValueTextBlock.Text = SelectedItem.ToString();
            Popup.IsOpen = false;
        }

        private void SelectedValueTextBlock_Tap(object sender, GestureEventArgs e)
        {
            Popup.IsOpen = !Popup.IsOpen;
        }
    }
}
