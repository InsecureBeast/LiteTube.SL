using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteTube.DataModel;
using LiteTube.Common;
using LiteTube.DataClasses;
using System.Windows.Input;
using LiteTube.Common.Helpers;
using LiteTube.ViewModels.Nodes;

namespace LiteTube.ViewModels.Playlist
{
    public class PlaylistsContainerViewModel : PlaylistListViewModel, IPlaylistsChangeHandler
    {
        private bool _isContainerShown;
        private readonly RelayCommand _manageCommand;
        private readonly RelayCommand _closeCommand;
        private string _videoId;

        public PlaylistsContainerViewModel(Func<IDataSource> getGeDataSource, IConnectionListener connectionListener)
            : base(null, getGeDataSource, connectionListener, null)
        {
            _manageCommand = new RelayCommand(Manage);
            _closeCommand = new RelayCommand(Close);
        }

        public bool IsContainerShown
        {
            get { return _isContainerShown; }
            set
            {
                _isContainerShown = value;
                NotifyOfPropertyChanged(() => IsContainerShown);
            }
        }

        public ICommand ManageCommand
        {
            get { return _manageCommand; }
        }

        public ICommand CloseCommand
        {
            get { return _closeCommand; }
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getDataSource().GetMyPlaylistList(nextPageToken);
        }

        internal override void NavigateTo(NavigationObject navObject)
        {
            IsContainerShown = false;

            if (navObject == null)
                return;

            var nodePlaylist = navObject.ViewModel as PlaylistNodeViewModel;
            if (nodePlaylist == null)
                return;

            _getDataSource().AddItemToPlaylist(_videoId, nodePlaylist.Id);
        }

        private void Close()
        {
            IsContainerShown = false;
        }

        private void Manage()
        {
            IsContainerShown = false;
            NavigationHelper.GoToPLaylistMangePage(this);
        }

        internal void SetVideoId(string videoId)
        {
            _videoId = videoId;
        }

        public void UpdatePlaylists()
        {
            Items.Clear();
        }
    }
}
