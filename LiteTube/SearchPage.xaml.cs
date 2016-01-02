using System;
using System.Windows;
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
            if (DataContext != null)
            {
                _firstLoad = false;
                return;
            }

            var model = PhoneApplicationService.Current.State["searchModel"] as SearchPageViewModel;
            DataContext = model;
            if (model.Items.Count > 0)
                _firstLoad = false;
            
            PhoneApplicationService.Current.State["searchModel"] = null;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
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

        private void Find_Click(object sender, EventArgs e)
        {
            SearchBox.Focus();
            SearchBox.SelectAll();
        }
    }
}