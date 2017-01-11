using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using LiteTube.ViewModels;
using LiteTube.Common.Helpers;
using LiteTube.Common.Tools;
using LiteTube.Resources;
using Microsoft.Phone.Shell;

namespace LiteTube
{
    public partial class MenuPage : PhoneApplicationPage
    {
        private int _selectedIndex = 0;

        public MenuPage()
        {
            InitializeComponent();
            Pivot.SelectionChanged += Pivot_SelectionChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(this);

            if (e.NavigationMode == NavigationMode.Back)
                return;

            var index = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("item", out index))
            {
                _selectedIndex = int.Parse(index);
            }
            Load();
        }

        private void Load()
        {
            var viewModel = DataContext as MenuPageViewModel;
            if (viewModel == null)
                return;

            if (!viewModel.IsAuthorized)
            {
                Pivot.Items.Remove(RecommendedItem);
                Pivot.Items.Remove(SubscribtionsItem);
                Pivot.Items.Remove(HistoryItem);
                Pivot.Items.Remove(LikedItem);
                Pivot.Items.Remove(PlaylistsItem);
                Pivot.Items.Remove(UploadedItem);
            }

            Pivot.SelectedIndex = _selectedIndex;
            BuildApplicationBar(_selectedIndex);
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedIndex = Pivot.SelectedIndex;
            BuildApplicationBar(_selectedIndex);
        }

        private void PlaylistManage_Click(object sender, EventArgs e)
        {
            NavigationHelper.GoToPLaylistMangePage();
        }

        private void Home_Click(object sender, EventArgs e)
        {
            NavigationHelper.GoHome();
        }

        private void BuildApplicationBar(int selectedIndex)
        {
            var appBar = new ApplicationBar
            {
                Mode = ApplicationBarMode.Default
            };

            var homeButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Home.png", AppResources.Home, Home_Click);
            var managePlaylistsButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Manage.png", AppResources.Manage, PlaylistManage_Click);

            appBar.Buttons.Add(homeButton);

            if (selectedIndex == 3)
            {
                appBar.Buttons.Add(managePlaylistsButton);
            }

            ApplicationBar = appBar;
        }
    }
}