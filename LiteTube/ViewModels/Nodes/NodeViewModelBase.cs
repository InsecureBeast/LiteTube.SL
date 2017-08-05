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
        private readonly IPlaylistsSevice _playlistService;

        public delegate void ShowPlaylistContainer(bool show);

        protected NodeViewModelBase(IDataSource dataSource, IContextMenuStrategy menu, bool isLargeItems, IPlaylistsSevice playlistService = null)
        {
            if (menu == null)
                MenuProvider = new NoContextMenuStrategy();

            _dataSource = dataSource;
            MenuProvider = menu;
            _playlistService = playlistService;
            _addToPlayListCommand = new RelayCommand<object>(AddToPlayList);

            IsLargeItems = isLargeItems;
        }

        public abstract string Id { get; }
        public abstract string VideoId { get; }
        public bool IsLargeItems { get; set; }
        public ICommand AddToPlaylistCommand => _addToPlayListCommand;
        public IContextMenuStrategy MenuProvider { get; }

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
                _playlistService?.ShowContainer(true, VideoId);
            }
        }
    }
}
