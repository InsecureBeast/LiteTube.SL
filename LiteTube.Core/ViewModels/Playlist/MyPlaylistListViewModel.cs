using System;
using System.Threading.Tasks;
using LiteTube.Core.DataClasses;
using LiteTube.Core.DataModel;

namespace LiteTube.Core.ViewModels.Playlist
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
            return await _getGeDataSource().GetMyPlaylistList(nextPageToken);
        }
    }
}
