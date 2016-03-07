using LiteTube.DataModel;
using LiteTube.ViewModels.Search;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LiteTube.ViewModels
{
    class SearchPageViewModel : ProgressIndicatorViewModel
    {
        private readonly SearchVideoViewModel _searchVideoViewModel;
        private readonly SearchChannelsViewModel _searchChannelsViewModel;
        private string _searchString;
        private int _selectedIndex;

        public SearchPageViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(null)
        {
            _searchVideoViewModel = new SearchVideoViewModel(geDataSource, connectionListener, ChangeProgressIndicator);
            _searchChannelsViewModel = new SearchChannelsViewModel(geDataSource, connectionListener, ChangeProgressIndicator);
        }

        public SearchVideoViewModel SearchVideoViewModel
        {
            get { return _searchVideoViewModel; }
        }

        public SearchChannelsViewModel SearchChannelsViewModel
        {
            get { return _searchChannelsViewModel; }
        }

        public string SearchString
        {
            get { return _searchString; }
            set { _searchString = value; }
        }

        internal async Task Search(int selectedIndex)
        {
            if (string.IsNullOrEmpty(_searchString))
                return;

            if (_selectedIndex == selectedIndex)
                Clear();

            _selectedIndex = selectedIndex;
            switch (selectedIndex)
            {
                case 0:
                    if (!_searchVideoViewModel.Items.Any())
                        await _searchVideoViewModel.Search(_searchString);
                    break;
                case 1:
                    if (!_searchChannelsViewModel.Items.Any())
                        await _searchChannelsViewModel.Search(_searchString);
                    break;
                default:
                    break;
            }
        }

        private void Clear()
        {
            _searchVideoViewModel.Items.Clear();
            _searchChannelsViewModel.Items.Clear();
        }

        private void ChangeProgressIndicator(bool isVisible)
        {
            if (isVisible)
            {
                ShowProgressIndicator();
                return;
            }
            HideProgressIndicator();
        }
    }
}
