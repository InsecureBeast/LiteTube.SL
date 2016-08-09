using System;
using System.Threading.Tasks;
using LiteTube.Core.DataClasses;
using LiteTube.Core.DataModel;

namespace LiteTube.Core.ViewModels
{
    class HistoryPageViewModel : SectionBaseViewModel
    {
        public HistoryPageViewModel(Func<IDataSource> datasource, IConnectionListener connectionListener)
            : base(datasource, connectionListener)
        {
        }

        public override string ToString()
        {
            return Title;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getGeDataSource().GetHistory(nextPageToken);
        }
    }
}
