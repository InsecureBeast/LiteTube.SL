using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LiteTube.Common;

namespace LiteTube.Controls
{
    public partial class NavigationPanel : UserControl
    {
        public NavigationPanel()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var page = VisualHelper.FindParent<Page>(this);
            BackgroundGridPopup.Height = page.ActualHeight;
        }

        private void Popup_Closed(object sender, object e)
        {
            MainMenuButton.IsChecked = false;
            LoginMenuButton.IsChecked = false;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuButton.IsChecked = false;
            LoginMenuButton.IsChecked = false;
        }
    }
}
