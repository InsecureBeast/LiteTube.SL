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
            }

            Pivot.SelectedIndex = _selectedIndex;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedIndex = Pivot.SelectedIndex;
        }
    }
}