using LiteTube.Common;
using LiteTube.DataModel;

namespace LiteTube
{
    class ContainerBootstrapper
    {
        private IDialogService _dialogService;
        private IDeviceHistory _deviceHistory;
        private Context _context;

        public void Build()
        {
            _deviceHistory = new DeviceHistory();
            _context = new Context(_deviceHistory);
            _context.BuidContext();
            _dialogService = new DialogService(_deviceHistory);
        }

        internal IDataSource GetDataSource()
        {
            return _context.DataSource;
        }

        internal IDialogService DialogService
        {
            get { return _dialogService; }
        }

        internal IConnectionListener ConnectionListener
        {
            get { return _context.ConnectionListener; }
        }
    }
}
