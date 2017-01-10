using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteTube.DataModel;

namespace LiteTube.ViewModels.Playlist
{
    class PlaylistsManagePageViewModel : PropertyChangedBase
    {
        private readonly Func<IDataSource> _getGeDataSource;
        private readonly NavigationPanelViewModel _navigatioPanelViewModel;

        public PlaylistsManagePageViewModel(Func<IDataSource> getGeDataSource, IConnectionListener connectionListener)
        {
            _getGeDataSource = getGeDataSource;
            _navigatioPanelViewModel = new NavigationPanelViewModel(getGeDataSource, connectionListener);
        }

        public NavigationPanelViewModel NavigationPanelViewModel
        {
            get { return _navigatioPanelViewModel; }
        }

        public PlaylistListViewModel PlaylistListViewModel
        {
            get { return App.ViewModel.PlaylistListViewModel; }
        }

        public void CreatePlaylist()
        {
            //_getGeDataSource().AddNewPlaylist();
        }
    }
}
