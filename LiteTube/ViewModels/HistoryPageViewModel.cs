using System;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;

namespace LiteTube.ViewModels
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
            return await _getDataSource().GetHistory(nextPageToken);
        }
    }
}
