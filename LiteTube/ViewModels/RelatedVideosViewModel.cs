using System;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;

namespace LiteTube.ViewModels
{
    public class RelatedVideosViewModel : SectionBaseViewModel
    {
        private readonly string _videoId;

        public RelatedVideosViewModel(string videoId, Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
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
