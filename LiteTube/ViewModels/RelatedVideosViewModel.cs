using System;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;

namespace LiteTube.ViewModels
{
    public class RelatedVideosViewModel : SectionBaseViewModel
    {
        private readonly string _videoId;

        public RelatedVideosViewModel(IVideoItem videoItem, Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
        {
            if (videoItem == null)
                return;
            
            _videoId = videoItem.Details.Video.Id;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getGeDataSource().GetRelatedVideoList(_videoId, nextPageToken);
        }
    }
}
