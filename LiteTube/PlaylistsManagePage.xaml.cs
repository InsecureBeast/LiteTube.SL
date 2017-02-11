using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LiteTube.Common.Helpers;
using LiteTube.Common.Tools;
using LiteTube.Resources;
using LiteTube.ViewModels.Playlist;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LiteTube
{
    public partial class PlaylistsManagePage : PhoneApplicationPage
    {
        public PlaylistsManagePage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(this);
        }

        private void Home_Click(object sender, EventArgs e)
        {
            NavigationHelper.GoHome();
        }

        private void BuildLocalizedApplicationBar()
        {
            var appBar = new ApplicationBar();
            var homeButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Home.png", AppResources.Home, Home_Click);
            appBar.Buttons.Add(homeButton);
            var addButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Add.png", AppResources.Create, Create_Click);
            appBar.Buttons.Add(addButton);
            ApplicationBar = appBar;
        }

        private void Create_Click(object sender, EventArgs e)
        {
            if (NewPlaylistLayout.Visibility == Visibility.Collapsed)
                NewPlaylistLayout.Visibility = Visibility.Visible;
            else
                NewPlaylistLayout.Visibility = Visibility.Collapsed;
        }

        private void ButtonOnClick(object sender, RoutedEventArgs e)
        {
            NewPlaylistLayout.Visibility = Visibility.Collapsed;
        }
    }
}