using System;
using System.Threading.Tasks;
using LiteTube.DataClasses;
using LiteTube.DataModel;

namespace LiteTube.ViewModels
{
    class MyPlaylistListViewModel : PlaylistListViewModel
    {
        public MyPlaylistListViewModel(Func<IDataSource> getGeDataSource, 
            IConnectionListener connectionListener, Action<bool> changeProgressIndicator = null)
            : base(null, getGeDataSource, connectionListener, changeProgressIndicator)
        {
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getDataSource().GetMyPlaylistList(nextPageToken);
        }
    }
}
