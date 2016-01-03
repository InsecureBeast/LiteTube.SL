using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LiteTube.ViewModels;
using LiteTube.Common.Helpers;

namespace LiteTube
{
    public partial class MenuPage : PhoneApplicationPage
    {
        private int _selectedIndex = 0;

        public MenuPage()
        {
            InitializeComponent();
            Loaded += MenuPage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(this);

            string index = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("item", out index))
            {
                _selectedIndex = int.Parse(index);
            }
        }

        private void MenuPage_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MenuPageViewModel;
            if (viewModel == null)
                return;

            if (!viewModel.IsAuthorized)
            {
                Pivot.Items.Remove(RecommendedItem);
                Pivot.Items.Remove(SubscribtionsItem);
                Pivot.Items.Remove(HistoryItem);
                Pivot.Items.Remove(FavoritesItem);
            }

            Pivot.SelectedIndex = _selectedIndex;
        }
    }
}