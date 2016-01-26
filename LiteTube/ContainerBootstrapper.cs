using LiteTube.Common;
using LiteTube.DataModel;

namespace LiteTube
{
    class ContainerBootstrapper
    {
        private IDataSource _dataSource;
        private IRemoteDataSource _remoteDataSource;
        private IYouTubeService _youTubeService;
        private IDialogService _dialogService;
        private IDeviceHistory _deviceHistory;
        private ConnectionListener _connectionListener;

        public void Build()
        {
            _connectionListener = new ConnectionListener();
            _deviceHistory = new DeviceHistory();
            _youTubeService = new YouTubeServiceControl();
            _remoteDataSource = new RemoteDataSource(_youTubeService, _connectionListener);
            var region = SettingsHelper.GetRegion();
            var quality = SettingsHelper.GetQuality();
            const int maxPageResult = 30;
            var remoteExceptionWrapper = new DataSourceExceptionWrapper(_remoteDataSource);
            _dataSource = new DataSource(remoteExceptionWrapper, region, maxPageResult, _deviceHistory, quality, _connectionListener);
            _dialogService = new DialogService(_deviceHistory);
        }

        internal IDataSource DataSource
        {
            get { return _dataSource; }
        }

        internal IDialogService DialogService
        {
            get { return _dialogService; }
        }

        internal ConnectionListener ConnectionListener
        {
            get { return _connectionListener; }
        }
    }
}
