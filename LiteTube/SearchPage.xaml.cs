using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LiteTube.Common;
using Microsoft.Phone.Controls;
using LiteTube.ViewModels;
using LiteTube.Common.Helpers;

namespace LiteTube
{
    public partial class SearchPage : PhoneApplicationPage
    {
        private bool _firstLoad = true;
        private readonly ObservableCollection<string> _autoCompleteItems = new ObservableCollection<string>(); 

        public SearchPage()
        {
            InitializeComponent();
            Loaded += SearchPage_Loaded;
            SearchBox.ItemsSource = _autoCompleteItems;
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

            NavigationHelper.OnNavigatedTo(this);

            var model = DataContext as SearchPageViewModel;
            if (model.Items.Count > 0)
                _firstLoad = false;
        }

        private async void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var viewModel = DataContext as SearchPageViewModel;
                if (viewModel == null)
                    return;

                Focus();

                viewModel.SearchString = SearchBox.Text;
                await viewModel.Search();
            }
        }

        private void Home_Click(object sender, EventArgs e)
        {
            NavigationHelper.GoHome();
        }

        private async void SearchBox_OnPopulating(object sender, PopulatingEventArgs e)
        {
            var result = await App.ViewModel.GetGeDataSource().GetAutoCompleteSearchItems(SearchBox.Text);
            
            _autoCompleteItems.Clear();
            foreach (var str in result)
            {
                _autoCompleteItems.Add(str);
            }

            SearchBox.PopulateComplete();
        }
    }
}