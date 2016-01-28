using LiteTube.Common;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;

namespace LiteTube.ViewModels
{
    public class MostPopularViewModel : SectionBaseViewModel
    {
        private readonly IVideoList _videoList;

        public MostPopularViewModel(IVideoList videoList, IDataSource dataSource, IConnectionListener connectionListener)
            : base(dataSource, connectionListener)
        {
            _videoList = videoList;
            _uniqueId = videoList.GetHashCode().ToString();
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var arstring = resourceLoader.GetString("PopularSectionHeader");
            //Title = arstring;
        }

        public MostPopularViewModel(IDataSource dataSource, IConnectionListener connectionListener)
            : base(dataSource, connectionListener)
        {
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var arstring = resourceLoader.GetString("PopularSectionHeader");
            //Title = arstring;
        }

        public override string ToString()
        {
            return Title;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _dataSource.GetMostPopular(nextPageToken);
        }
    }
}
