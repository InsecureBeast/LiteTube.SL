using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LiteTube.Common.Helpers;
using Microsoft.Phone.Tasks;
using LiteTube.Resources;

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
            if (page == null)
            {
                BackgroundGridPopup.Height = 1920;
                return;
            }
            
            BackgroundGridPopup.Height = page.ActualHeight;
        }

        private void Popup_Closed(object sender, EventArgs eventArgs)
        {
            MainMenuButton.IsChecked = false;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            NavigationHelper.Navigate("/AboutPage.xaml", null);
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            var emailComposeTask = new EmailComposeTask
            {
                Subject = string.Format("{0} feedback", AppResources.ApplicationTitle),
                Body = string.Format("<{0}>", AppResources.FeedbackMessage),
                To = "pe.dmitriev@gmail.com"
            };
            emailComposeTask.Show();
        }

        private void BackgroundPopup_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MainMenuButton.IsChecked = false;
        }
    }
}
