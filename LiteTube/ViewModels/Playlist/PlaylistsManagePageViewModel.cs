using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LiteTube.Common;
using LiteTube.DataModel;
using LiteTube.Resources;
using RelayCommand = MyToolkit.Command.RelayCommand;

namespace LiteTube.ViewModels.Playlist
{
    class PlaylistsManagePageViewModel : PropertyChangedBase
    {
        private readonly Func<IDataSource> _getGeDataSource;
        private readonly NavigationPanelViewModel _navigatioPanelViewModel;
        private readonly PlaylistListViewModel _playlistListViewModel;
        private readonly RelayCommand _createCommand;
        private string _playlistTitle;
        private string _playlistDescription;
        private readonly List<AccessItem> _accessItems;
        private AccessItem _selectedAccess;

        public PlaylistsManagePageViewModel(Func<IDataSource> getGeDataSource, IConnectionListener connectionListener)
        {
            _getGeDataSource = getGeDataSource;
            _navigatioPanelViewModel = new NavigationPanelViewModel(getGeDataSource, connectionListener);
            _playlistListViewModel = new MyPlaylistListViewModel(_getGeDataSource, connectionListener, new DeleteContextMenuStrategy());
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

        public PlaylistListViewModel PlaylistListViewModel
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
            //PlaylistListViewModel.Items.Add(new PlaylistNodeViewModel(newPlaylist,_getGeDataSource(), new ContextMenuStartegy()));
            App.ViewModel.PlaylistListViewModel.Items.Clear();
            PlaylistListViewModel.Items.Clear();
            await PlaylistListViewModel.FirstLoad();
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
