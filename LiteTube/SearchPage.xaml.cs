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
    public partial class SearchPage : PhoneApplicationPage
    {
        private bool _firstLoad = true;

        public SearchPage()
        {
            InitializeComponent();
            Loaded += SearchPage_Loaded;
        }

        private void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_firstLoad)
                SearchBox.Focus();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var model = PhoneApplicationService.Current.State["searchModel"] as SearchPageViewModel;
            DataContext = model;
            object obj;
            if (PhoneApplicationService.Current.State.TryGetValue("search", out obj))
            {
                var view = obj as SearchPage;
                SearchBox.Text = view.SearchBox.Text;
                SearchList = view.SearchList;
                if (model.Items.Count > 0)
                    _firstLoad = false;
            }
            
            PhoneApplicationService.Current.State["searchModel"] = null;
            PhoneApplicationService.Current.State["search"] = null;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            PhoneApplicationService.Current.State["search"] = this;
            PhoneApplicationService.Current.State["searchModel"] = DataContext;

        }

        private async void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var viewModel = DataContext as SearchPageViewModel;
                if (viewModel == null)
                    return;

                Focus();
                SearchBox.IsReadOnly = true;

                viewModel.SearchString = SearchBox.Text;
                await viewModel.Search();

                SearchBox.IsReadOnly = false;
            }
        }
    }
}