using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LiteTube.Common.Helpers;
using LiteTube.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LiteTube.Common.Tools;
using LiteTube.Resources;

namespace LiteTube
{
    public partial class PlaylistPage : PhoneApplicationPage
    {
        public PlaylistPage()
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

        private void Play_Click(object sender, EventArgs e)
        {
            var model = DataContext as PlaylistPageViewModel;
            if (model == null)
                return;

            model.PlayAll();
        }

        private void BuildLocalizedApplicationBar()
        {
            var appBar = new ApplicationBar();
            var homeButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Home.png", AppResources.Home, Home_Click);
            appBar.Buttons.Add(homeButton);
            var playAllButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Play.png", AppResources.PlayAll, Play_Click);
            appBar.Buttons.Add(playAllButton);
            ApplicationBar = appBar;
        }
    }
}