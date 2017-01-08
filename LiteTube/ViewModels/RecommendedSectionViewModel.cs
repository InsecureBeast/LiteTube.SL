using System;
using System.Threading.Tasks;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.ViewModels.Playlist;

namespace LiteTube.ViewModels
{
    class RecommendedSectionViewModel : SectionBaseViewModel
    {
        public RecommendedSectionViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener, IPlaylistsSevice playlistService)
            : base(geDataSource, connectionListener, playlistService)
        {
        }

        public override string ToString()
        {
            return Title;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getDataSource().GetRecommended(nextPageToken);
        }
    }
}
