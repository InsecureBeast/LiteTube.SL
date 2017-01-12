using System;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.Common;
using LiteTube.DataClasses;
using LiteTube.DataModel;

namespace LiteTube.ViewModels.Playlist
{
    public class MyPlaylistListViewModel : PlaylistListViewModel
    {
        private readonly IPlaylistsChangeHandler _playlistsChangeHandler;

        public MyPlaylistListViewModel(Func<IDataSource> getGeDataSource, IConnectionListener connectionListener, 
            IContextMenuStrategy contextMenuStrategy, IPlaylistsChangeHandler playlistsChangeHandler, Action<bool> changeProgressIndicator = null)
            : base(null, getGeDataSource, connectionListener, contextMenuStrategy, changeProgressIndicator)
        {
            _playlistsChangeHandler = playlistsChangeHandler;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getDataSource().GetMyPlaylistList(nextPageToken);
        }

        protected internal override async Task Delete(string playlistId)
        {
            await _getDataSource().RemovePlaylist(playlistId);
            var item = Items.FirstOrDefault(i => i.Id == playlistId);
            if (item == null)
                return;

            Items.Remove(item);
            App.ViewModel.PlaylistListViewModel.Items.Clear();
            _playlistsChangeHandler.UpdatePlaylists();
        }
    }
}
