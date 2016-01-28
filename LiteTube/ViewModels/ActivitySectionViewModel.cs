using LiteTube.Common;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System;
using System.Threading.Tasks;

namespace LiteTube.ViewModels
{
    public class ActivitySectionViewModel : SectionBaseViewModel
    {
        private bool _canLoad = false;

        public ActivitySectionViewModel(IVideoList activity, IDataSource dataSource, IConnectionListener connectionListener)
            : base(dataSource, connectionListener)
        {
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var arstring = resourceLoader.GetString("RecommendedSectionHeader");
            //Title = arstring;
            Title = "Activity"; //TODO Localize
        }

        public ActivitySectionViewModel(IDataSource dataSource, IConnectionListener connectionListener)
            : base(dataSource, connectionListener)
        {
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var arstring = resourceLoader.GetString("RecommendedSectionHeader");
            //Title = arstring;
            Title = "Activity"; //TODO Localize
        }

        public override string ToString()
        {
            return Title;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _dataSource.GetActivity(nextPageToken);
        }
    }
}

