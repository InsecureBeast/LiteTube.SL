using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteTube.Common;
using LiteTube.DataModel;
using LiteTube.DataClasses;

namespace LiteTube.ViewModels.Playlist
{
    class PlaylistWatchLaterViewModel : PlaylistVideoPageViewModel
    {
        public PlaylistWatchLaterViewModel(Func<IDataSource> getDataSource, IConnectionListener connectionListener) 
            : base(string.Empty, getDataSource, connectionListener)
        {
            _playlistVideosViewModel = new VideosWatchLaterViewModel(getDataSource, connectionListener, (s) => { PlaylistVideoItemClick(s); });

        }
    }

    class VideosWatchLaterViewModel : PlaylistVideosViewModel
    {
        public VideosWatchLaterViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener, Action<NavigationObject> itemTap) 
            : base(string.Empty, geDataSource, connectionListener, itemTap)
        {
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getDataSource().GetWatchLater(nextPageToken);
        }
    }
}
