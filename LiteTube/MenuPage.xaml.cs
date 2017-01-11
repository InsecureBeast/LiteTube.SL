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
        private readonly ApplicationBarIconButton _playlistsManagerButton;

        public MenuPage()
        {
            InitializeComponent();
            _playlistsManagerButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Manage.png", AppResources.Manage, PlaylistManage_Click);
            Pivot.SelectionChanged += Pivot_SelectionChanged;
            ApplicationBar.Mode = ApplicationBarMode.Default;
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
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedIndex = Pivot.SelectedIndex;
            if (_selectedIndex == 3)
            {
                if (!ApplicationBar.Buttons.Contains(_playlistsManagerButton))
                    ApplicationBar.Buttons.Add(_playlistsManagerButton);
            }
            else
            {
                if (ApplicationBar.Buttons.Contains(_playlistsManagerButton))
                    ApplicationBar.Buttons.Remove(_playlistsManagerButton);
            }
        }

        private void PlaylistManage_Click(object sender, EventArgs e)
        {
            NavigationHelper.GoToPLaylistMangePage();
        }
    }
}