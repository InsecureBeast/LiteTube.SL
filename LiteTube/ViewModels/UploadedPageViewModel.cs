using System;
using System.Threading.Tasks;
using LiteTube.DataModel;
using LiteTube.DataClasses;

namespace LiteTube.ViewModels
{
    class UploadedPageViewModel : SectionBaseViewModel
    {
        public UploadedPageViewModel(Func<IDataSource> getGeDataSource,    IConnectionListener connectionListener) 
            : base(getGeDataSource, connectionListener, null)
        {
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            var uploadedPlaylistId = _getDataSource().UploadedPlaylistId;
            return await _getDataSource().GetVideoPlaylist(uploadedPlaylistId, nextPageToken);
        }
    }
}
