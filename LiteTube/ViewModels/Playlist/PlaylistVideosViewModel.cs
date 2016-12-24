using LiteTube.DataClasses;
using LiteTube.DataModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.Common;
using LiteTube.ViewModels.Nodes;

namespace LiteTube.ViewModels
{
    public class PlaylistVideosViewModel : SectionBaseViewModel
    {
        private readonly string _playlistId;
        private Action<NavigationObject> _itemTap;

        public PlaylistVideosViewModel(string playlistId, Func<IDataSource> geDataSource, IConnectionListener connectionListener, Action<NavigationObject> itemTap)
            : base(geDataSource, connectionListener)
        {
            if (playlistId == null)
                return;

            _playlistId = playlistId;
            _itemTap = itemTap;
        }

        public void SetPlayingVideo(NodeViewModelBase video)
        {
            var node = Items.FirstOrDefault(n => n.Id == video.Id);
            var videoItem = node as VideoItemViewModel;
            if (videoItem == null)
                return;

            var nowPlaying = GetNowPlayingVideo();
            if (nowPlaying == null)
                return;

            nowPlaying.IsNowPlaying = false;
            videoItem.IsNowPlaying = true;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getDataSource().GetVideoPlaylist(_playlistId, nextPageToken);
        }

        internal override void NavigateTo(NavigationObject navObject)
        {
            _itemTap(navObject);
        }

        public VideoItemViewModel GetNextVideo()
        {
            var isSet = false;
            foreach (var item in Items.Cast<VideoItemViewModel>())
            {
                if (isSet)
                    return item;

                if (item.IsNowPlaying)
                    isSet = true;
            }

            return null;
        }

        public VideoItemViewModel GetPreviousVideo()
        {
            VideoItemViewModel previous = null;
            foreach (var item in Items.Cast<VideoItemViewModel>())
            {
                if (item.IsNowPlaying)
                    return previous;

                previous = item;
            }

            return null;
        }

        public VideoItemViewModel GetNowPlayingVideo()
        {
            var node = Items.FirstOrDefault(n => ((VideoItemViewModel)n).IsNowPlaying);
            return node as VideoItemViewModel;
        }
    }
}
