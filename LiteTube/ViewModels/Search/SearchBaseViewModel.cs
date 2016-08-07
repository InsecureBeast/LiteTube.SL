using LiteTube.Common.Helpers;
using LiteTube.Common.Tools;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System;
using System.Threading.Tasks;

namespace LiteTube.ViewModels.Search
{
    class SearchBaseViewModel : SectionBaseViewModel
    {
        private string _searchString;
        private readonly SearchType _searchType;

        public SearchBaseViewModel(SearchType searchType, Func<IDataSource> geDataSource, IConnectionListener connectionListener, Action<bool> changeProgressIndicator)
            : base(geDataSource, connectionListener, changeProgressIndicator)
        {
            _searchType = searchType;
            IsLoading = false;
            IsEmpty = false;
            _showAdv = SettingsHelper.IsAdvVisible;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            if (string.IsNullOrEmpty(_searchString))
                return null;

            return await _getGeDataSource().Search(_searchString, nextPageToken, _searchType);
        }

        internal async Task Search(string searchString)
        {
            LastRequest.SearchString = _searchString;
            _searchString = searchString;
            Items.Clear();
            await FirstLoad();
        }
    }
}
