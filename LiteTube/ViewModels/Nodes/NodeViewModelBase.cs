
using LiteTube.Common;
using LiteTube.DataModel;
using LiteTube.ViewModels.Playlist;
using MyToolkit.Command;
using System.Windows.Input;

namespace LiteTube.ViewModels.Nodes
{
    public abstract class NodeViewModelBase : PropertyChangedBase
    {
        private readonly RelayCommand<object> _addToPlayListCommand;
        protected readonly IDataSource _dataSource;
        private readonly IContextMenuStrategy _menu;
        private readonly IPlaylistsSevice _playlistService;

        public delegate void ShowPlaylistContainer(bool show);

        public NodeViewModelBase(IDataSource dataSource, IContextMenuStrategy menu, IPlaylistsSevice playlistService = null)
        {
            if (menu == null)
                _menu = new NoContextMenuStrategy();

            _dataSource = dataSource;
            _menu = menu;
            _playlistService = playlistService;
            _addToPlayListCommand = new RelayCommand<object>(AddToPlayList);
        }

        public abstract string Id { get; }
        public abstract string VideoId { get; }

        public ICommand AddToPlaylistCommand
        {
            get { return _addToPlayListCommand; }
        }

        public IContextMenuStrategy MenuProvider
        {
            get { return _menu; }
        }

        protected virtual async void AddToPlayList(object obj)
        {
            if (obj.ToString() == "WatchLater")
            {
                await _dataSource.AddItemToPlaylist(VideoId, _dataSource.WatchLaterPlaylistId);
            }

            if (obj.ToString() == "Favorites")
            {
                await _dataSource.AddItemToPlaylist(VideoId, _dataSource.FavoritesPlaylistId);
            }

            if (obj.ToString() == "Playlist")
            {
                if (_playlistService != null)
                    _playlistService.ShowContainer(true, VideoId);
            }
        }
    }
}
