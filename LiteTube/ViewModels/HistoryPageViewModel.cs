using LiteTube.Common;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;

namespace LiteTube.ViewModels
{
    class HistoryPageViewModel : SectionBaseViewModel
    {
        public HistoryPageViewModel(IDataSource datasource, ConnectionListener connectionListener)
            : base(datasource, connectionListener)
        {
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var arstring = resourceLoader.GetString("HistorySectionHeader");
            //_title = arstring;
            _title = "watch history"; //TODO localization
        }

        public override string ToString()
        {
            return Title;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _dataSource.GetHistory(nextPageToken);
        }
    }
}
