using System;
using LiteTube.Common;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;

namespace LiteTube.ViewModels
{
    public class MostPopularViewModel : SectionBaseViewModel
    {
        private readonly IVideoList _videoList;

        public MostPopularViewModel(IVideoList videoList, Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
        {
            _videoList = videoList;
            _uniqueId = videoList.GetHashCode().ToString();
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var arstring = resourceLoader.GetString("PopularSectionHeader");
            //Title = arstring;
        }

        public MostPopularViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
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
            return await _getGeDataSource().GetMostPopular(nextPageToken);
        }
    }
}
