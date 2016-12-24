
using LiteTube.Common;
using LiteTube.DataModel;
using MyToolkit.Command;
using System.Windows.Input;

namespace LiteTube.ViewModels.Nodes
{
    public abstract class NodeViewModelBase : PropertyChangedBase
    {
        private readonly RelayCommand<object> _addToPlayListCommand;
        protected readonly IDataSource _dataSource;
        private readonly IContextMenuProvider _menuProvider;

        public NodeViewModelBase(IDataSource dataSource, IContextMenuProvider menuProvider)
        {
            _dataSource = dataSource;

            _menuProvider = menuProvider;
            if (menuProvider == null)
            {
                _menuProvider = new ContextMenuProvider()
                {
                    CanAddToPlayList = false,
                    CanDelete = false
                };
            }
           
            _addToPlayListCommand = new RelayCommand<object>(AddToPlayList);
        }

        public abstract string Id { get; }
        public abstract string VideoId { get; }

        public ICommand AddToPlaylistCommand
        {
            get { return _addToPlayListCommand; }
        }

        public IContextMenuProvider MenuProvider
        {
            get { return _menuProvider; }
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
        }
    }
}
