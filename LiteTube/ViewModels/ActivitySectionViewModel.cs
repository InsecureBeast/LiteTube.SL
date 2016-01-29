using LiteTube.Common;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System;
using System.Threading.Tasks;

namespace LiteTube.ViewModels
{
    public class ActivitySectionViewModel : SectionBaseViewModel
    {
        public ActivitySectionViewModel(IVideoList activity, Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
        {
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var arstring = resourceLoader.GetString("RecommendedSectionHeader");
            //Title = arstring;
            Title = "Activity"; //TODO Localize
        }

        public ActivitySectionViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
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
            return await _getGeDataSource().GetActivity(nextPageToken);
        }
    }
}

