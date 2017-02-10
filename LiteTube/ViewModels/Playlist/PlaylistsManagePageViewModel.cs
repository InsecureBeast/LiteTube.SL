using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LiteTube.Common;
using LiteTube.DataModel;
using LiteTube.Resources;
using LiteTube.ViewModels.Nodes;
using RelayCommand = MyToolkit.Command.RelayCommand;

namespace LiteTube.ViewModels.Playlist
{
    public interface IPlaylistsChangeHandler
    {
        void UpdatePlaylists();
    }

    class PlaylistsManagePageViewModel : PropertyChangedBase
    {
        private readonly Func<IDataSource> _getGeDataSource;
        private readonly IPlaylistsChangeHandler _playlistsChangeHandler;
        private readonly NavigationPanelViewModel _navigatioPanelViewModel;
        private readonly MyPlaylistListViewModel _playlistListViewModel;
        private readonly RelayCommand _createCommand;
        private string _playlistTitle;
        private string _playlistDescription;
        private readonly List<AccessItem> _accessItems;
        private AccessItem _selectedAccess;

        public PlaylistsManagePageViewModel(Func<IDataSource> getGeDataSource, IConnectionListener connectionListener, IPlaylistsChangeHandler playlistsChangeHandler)
        {
            _getGeDataSource = getGeDataSource;
            _playlistsChangeHandler = playlistsChangeHandler;
            _navigatioPanelViewModel = new NavigationPanelViewModel(getGeDataSource, connectionListener);
            _playlistListViewModel = new MyPlaylistListViewModel(_getGeDataSource, connectionListener, new DeleteContextMenuStrategy(), playlistsChangeHandler);
            _playlistListViewModel.FirstLoad();
            _createCommand = new RelayCommand(CreatePlaylist, CanCreateNewPlaylist);
            _accessItems = new List<AccessItem>() { new AccessItem(PrivacyStatus.Public), new AccessItem(PrivacyStatus.Private) };
            _selectedAccess = _accessItems.First();
        }

        private bool CanCreateNewPlaylist()
        {
            return !string.IsNullOrEmpty(PlaylistTitle);
        }

        public NavigationPanelViewModel NavigationPanelViewModel
        {
            get { return _navigatioPanelViewModel; }
        }

        public MyPlaylistListViewModel PlaylistListViewModel
        {
            get { return _playlistListViewModel; }
        }

        public ICommand CreateCommand
        {
            get { return _createCommand; }
        }

        public string PlaylistTitle
        {
            get { return _playlistTitle; }
            set
            {
                _playlistTitle = value;
                NotifyOfPropertyChanged(() => PlaylistTitle);
                _createCommand.RaiseCanExecuteChanged();
            }
        }

        public string PlaylistDescription
        {
            get { return _playlistDescription; }
            set
            {
                _playlistDescription = value;
                NotifyOfPropertyChanged(() => PlaylistDescription);
            }
        }

        public List<AccessItem> AccessItems
        {
            get { return _accessItems; }
        }

        public AccessItem SelectedAccess
        {
            get { return _selectedAccess; }
            set
            {
                _selectedAccess = value;
                NotifyOfPropertyChanged(() => SelectedAccess);
            }
        }

        private async void CreatePlaylist()
        {
            var newPlaylist = await _getGeDataSource().AddNewPlaylist(PlaylistTitle, PlaylistDescription, _selectedAccess.Status);
            if (newPlaylist != null)
            {
                PlaylistListViewModel.Items.Insert(0, new PlaylistNodeViewModel(newPlaylist, _getGeDataSource(), new DeleteContextMenuStrategy(), PlaylistListViewModel.Delete));
                _playlistsChangeHandler.UpdatePlaylists();
            }
            //Clear data
            PlaylistTitle = string.Empty;
            PlaylistDescription = string.Empty;
            SelectedAccess = AccessItems.First();
        }
    }

    public class AccessItem
    {
        public AccessItem(PrivacyStatus status)
        {
            Status = status;
            if (status == PrivacyStatus.Private)
            {
                Access = AppResources.StatusPrivate;
                return;
            }

            Access = AppResources.StatusPublic;
        }

        public PrivacyStatus Status { get; private set; }
        public string Access { get; private set; }

        public override string ToString()
        {
            return Access;
        }
    }
}
