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
using LiteTube.Common.Helpers;
using Microsoft.Phone.Tasks;

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
            BackgroundGridPopup1.Height = page.ActualHeight;
        }

        private void Popup_Closed(object sender, EventArgs eventArgs)
        {
            MainMenuButton.IsChecked = false;
            LoginMenuButton.IsChecked = false;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuButton.IsChecked = false;
            LoginMenuButton.IsChecked = false;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            NavigationHelper.Navigate("/AboutPage.xaml", null);
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.Subject = "LiteTube feedback";
            emailComposeTask.Body = "[Your feedback here]";
            emailComposeTask.To = "[LiteTube Team]dmitriev.pe@yandex.ru";
            emailComposeTask.Show();
        }

        private void BackgroundPopup_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MainMenuButton.IsChecked = false;
            LoginMenuButton.IsChecked = false;
        }
    }
}
