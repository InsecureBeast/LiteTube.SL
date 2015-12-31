using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;

namespace LiteTube.ViewModels
{
    public class RelatedVideosViewModel : SectionBaseViewModel
    {
        private readonly string _videoId;

        public RelatedVideosViewModel(IVideoItem videoItem, IDataSource dataSource) : base(dataSource)
        {
            _videoId = videoItem.Details.Video.Id;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _dataSource.GetRelatedVideoList(_videoId, nextPageToken);
        }
    }
}
