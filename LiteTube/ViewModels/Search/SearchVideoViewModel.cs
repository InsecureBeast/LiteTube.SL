using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.ViewModels.Playlist;
using System;

namespace LiteTube.ViewModels.Search
{
    class SearchVideoViewModel : SearchBaseViewModel
    {
        public SearchVideoViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener, 
            Action<bool> changeProgressIndicator, IPlaylistsSevice playlistService)
            : base(SearchType.Video, geDataSource, connectionListener, changeProgressIndicator)
        {
            _playlistService = playlistService;
        }
    }
}
