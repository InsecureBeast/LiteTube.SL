using System;
using System.Threading.Tasks;
using LiteTube.Common;
using LiteTube.DataClasses;
using LiteTube.DataModel;

namespace LiteTube.ViewModels
{
    class RecommendedSectionViewModel : SectionBaseViewModel
    {
        //private bool _canLoad = false;

        public RecommendedSectionViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
        {
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var arstring = resourceLoader.GetString("RecommendedSectionHeader");
            //Title = arstring;
            Title = "Recommended for you"; //TODO Localization
        }

        public override string ToString()
        {
            return Title;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            //if (!_canLoad)
            //{
            //    _canLoad = true;
            //    return null;
            //}
            return await _getGeDataSource().GetRecommended(nextPageToken);
        }
    }
}
