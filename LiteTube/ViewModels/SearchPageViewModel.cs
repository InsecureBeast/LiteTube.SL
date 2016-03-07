using LiteTube.DataClasses;
using LiteTube.DataModel;
using System;
using System.Threading.Tasks;

namespace LiteTube.ViewModels
{
    class SearchPageViewModel : SectionBaseViewModel
    {
        private string _searchString;

        public SearchPageViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
        {
            _uniqueId = Guid.NewGuid().ToString();
            IsLoading = false;
            IsEmpty = false;
        }

        public override string ToString()
        {
            return _searchString;
        }

        public string SearchString
        {
            get { return _searchString; }
            set { _searchString = value; }
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            if (string.IsNullOrEmpty(_searchString))
                return null;

            return await _getGeDataSource().Search(_searchString, nextPageToken, SearchType.Video);
        }

        internal async Task Search()
        {
            Items.Clear();
            IsLoading = true;
            ShowProgressIndicator();
            var responseList = await GetItems(string.Empty);
            if (responseList == null)
            {
                IsEmpty = true;
                IsLoading = false;
                return;
            }
            _pageToken = responseList.NextPageToken;
            LoadItems(responseList);

            IsLoading = false;
            HideProgressIndicator();
        }
    }
}
