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
            if (DataContext != null)
                return;

            var model = PhoneApplicationService.Current.State["model"];
            DataContext = model;
            PhoneApplicationService.Current.State["model"] = null;

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

            //var scroll = VisualHelper.GetScrollViewer(VideoCategoriesList);
            //if (scroll != null)
            //    scroll.ChangeView(0, _categoriesScrollVerticalOffset, null);

            if (viewModel.IsAuthorized)
            {
                //scroll = VisualHelper.GetScrollViewer(SubscriptionsList);
                //if (scroll != null)
                //    scroll.ChangeView(0, _subscriptionsScrollVerticalOffset, null);

                //scroll = VisualHelper.GetScrollViewer(RecommendedList);
                //if (scroll != null)
                //    scroll.ChangeView(0, _recommendedScrollVerticalOffset, null);

                //scroll = VisualHelper.GetScrollViewer(HistoryList);
                //if (scroll != null)
                //    scroll.ChangeView(0, _historyScrollVerticalOffset, null);

                //scroll = VisualHelper.GetScrollViewer(FavoritesList);
                //if (scroll != null)
                //    scroll.ChangeView(0, _favoritesScrollVerticalOffset, null);

                return;
            }

            Pivot.Items.Remove(RecommendedItem);
            Pivot.Items.Remove(SubscribtionsItem);
            Pivot.Items.Remove(HistoryItem);
            Pivot.Items.Remove(FavoritesItem);

            Pivot.SelectedIndex = _selectedIndex;
        }
    }
}