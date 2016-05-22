using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteTube.DataModel;
using LiteTube.DataClasses;
using LiteTube.Common.Helpers;
using LiteTube.Common;
using LiteTube.ViewModels.Nodes;

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

        private void PlaylistVideoItemClick(NavigationObject s)
        {
            _playlistVideosViewModel.SetPlayingVideo(s.ViewModel);
            VideoViewModel = new VideoPageViewModel(s.ViewModel.VideoId, _getDataSource, _connectionListener);
        }
    }
}
