using System;
using System.Threading.Tasks;
using LiteTube.DataClasses;
using LiteTube.DataModel;

namespace LiteTube.ViewModels
{
    class RecommendedSectionViewModel : SectionBaseViewModel
    {
        public RecommendedSectionViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
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
