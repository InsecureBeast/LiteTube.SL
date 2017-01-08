using System;
using System.Linq;
using LiteTube.DataModel;
using LiteTube.Common.Helpers;
using LiteTube.Common;
using LiteTube.ViewModels.Nodes;

namespace LiteTube.ViewModels
{
    class PlaylistVideoPageViewModel : PropertyChangedBase
    {
        public event EventHandler PlaylistItemChanged;

        private VideoPageViewModel _videoViewModel;
        private readonly IConnectionListener _connectionListener;
        private readonly Func<IDataSource> _getDataSource;
        protected PlaylistVideosViewModel _playlistVideosViewModel;
        private readonly NavigationPanelViewModel _navigatioPanelViewModel;

        public PlaylistVideoPageViewModel(string playlistId, Func<IDataSource> getDataSource, IConnectionListener connectionListener) 
        {
            _connectionListener = connectionListener;
            _getDataSource = getDataSource;

            _navigatioPanelViewModel = new NavigationPanelViewModel(_getDataSource, _connectionListener);
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
                PlaylistItemChanged?.Invoke(this, new EventArgs());
            }
        }

        public PlaylistVideosViewModel PlaylistVideosViewModel
        {
            get { return _playlistVideosViewModel; }
        }

        public NavigationPanelViewModel NavigationPanelViewModel
        {
            get { return _navigatioPanelViewModel; }
        }

        public void SkipNext()
        {
            var next = _playlistVideosViewModel.GetNextVideo();
            if (next == null)
                return;

            _playlistVideosViewModel.SetPlayingVideo(next);
            VideoViewModel = new VideoPageViewModel(next.VideoId, _getDataSource, _connectionListener);
        }

        public void SkipPrevious()
        {
            var previous = _playlistVideosViewModel.GetPreviousVideo();
            if (previous == null)
                return;

            _playlistVideosViewModel.SetPlayingVideo(previous);
            VideoViewModel = new VideoPageViewModel(previous.VideoId, _getDataSource, _connectionListener);
        }

        private void LoadPlaylistVideos(string playlistId)
        {
            LayoutHelper.InvokeFromUiThread(async () =>
            {
                await _playlistVideosViewModel.FirstLoad();
                var firstVideo = _playlistVideosViewModel.Items.FirstOrDefault();
                if (firstVideo == null)
                    return;

                VideoViewModel = new VideoPageViewModel(firstVideo.VideoId, _getDataSource, _connectionListener);
                var videoNode = firstVideo as VideoItemViewModel;
                if (videoNode == null)
                    return;

                videoNode.IsNowPlaying = true;
            });
        }

        protected void PlaylistVideoItemClick(NavigationObject navObject)
        {
            if (navObject == null)
                return;

            _playlistVideosViewModel.SetPlayingVideo(navObject.ViewModel);
            VideoViewModel = new VideoPageViewModel(navObject.ViewModel.VideoId, _getDataSource, _connectionListener);
        }
    }
}
