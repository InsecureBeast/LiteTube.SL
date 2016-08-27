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
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItemSource", typeof(string), typeof(ComboBox), null);

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

        public string SelectedItem
        {
            get { return GetValue(SelectedItemProperty) as string; }
            set { SetValue(SelectedItemProperty, value); }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var page = VisualHelper.FindParent<Page>(this);
            if (page == null)
                return;

            page.MouseLeftButtonDown += Page_MouseLeftButtonDown;
            DropDownList.ItemsSource = ItemsSource;
            var firstItem = DropDownList.Items.First().ToString();
            DropDownList.SelectedItem = firstItem;
            SelectedValueTextBlock.Text = firstItem;
        }

        private void Page_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void DropDownList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // при выборе элемента из списка - устанавливаем выбранный элемент и сворачиваем список
            SelectedItem = DropDownList.SelectedItem.ToString();
            SelectedValueTextBlock.Text = SelectedItem;
            Popup.IsOpen = false;
        }

        private void SelectedValueTextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Popup.IsOpen = !Popup.IsOpen;
        }
    }
}
