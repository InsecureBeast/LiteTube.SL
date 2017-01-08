using System;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;
using LiteTube.ViewModels.Playlist;

namespace LiteTube.ViewModels
{
    public class RelatedVideosViewModel : SectionBaseViewModel
    {
        private readonly string _videoId;

        public RelatedVideosViewModel(string videoId, Func<IDataSource> geDataSource, IConnectionListener connectionListener, IPlaylistsSevice playlistService)
            : base(geDataSource, connectionListener, playlistService)
        {
            if (videoId == null)
                return;
            
            _videoId = videoId;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getDataSource().GetRelatedVideoList(_videoId, nextPageToken);
        }
    }
}
