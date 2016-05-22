using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteTube.DataModel;
using LiteTube.DataClasses;
using LiteTube.Common.Helpers;
using LiteTube.Common;

namespace LiteTube.ViewModels
{
    public class PlaylistVideoPageViewModel : PropertyChangedBase
    {
        private VideoPageViewModel _videoViewModel;
        private readonly IConnectionListener _connectionListener;
        private readonly Func<IDataSource> _getDataSource;
        private readonly PlaylistVideosViewModel _playlistVideosViewModel;

        public PlaylistVideoPageViewModel(string playlistId, Func<IDataSource> getDataSource, IConnectionListener connectionListener) 
        {
            _connectionListener = connectionListener;
            _getDataSource = getDataSource;

            _videoViewModel = new VideoPageViewModel();
            _playlistVideosViewModel = new PlaylistVideosViewModel(playlistId, getDataSource, connectionListener, (s) => { PlaylistVideoItemClick(s); });
            LoadPlaylistVideos(playlistId);
        }

        public VideoPageViewModel VideoViewModel
        {
            get { return _videoViewModel; }
            private set
            {
                _videoViewModel = value;
                NotifyOfPropertyChanged(() => VideoViewModel);
            }
        }

        public PlaylistVideosViewModel PlaylistVideosViewModel
        {
            get { return _playlistVideosViewModel; }
        }

        private void LoadPlaylistVideos(string playlistId)
        {
            LayoutHelper.InvokeFromUiThread(async () =>
            {
                await _playlistVideosViewModel.FirstLoad();
                var firstVideoId = _playlistVideosViewModel.Items.FirstOrDefault();
                if (firstVideoId == null)
                    return;

                VideoViewModel = new VideoPageViewModel(firstVideoId.VideoId, _getDataSource, _connectionListener);
            });
        }

        private void PlaylistVideoItemClick(NavigationObject s)
        {
            VideoViewModel = new VideoPageViewModel(s.ViewModel.VideoId, _getDataSource, _connectionListener);
        }
    }
}
