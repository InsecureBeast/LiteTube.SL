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

        public void Build()
        {
            _deviceHistory = new DeviceHistory();
            _youTubeService = new YouTubeServiceControl();
            _remoteDataSource = new RemoteDataSource(_youTubeService);
            var region = SettingsHelper.GetRegion();
            var quality = SettingsHelper.GetQuality();
            const int maxPageResult = 30;
            var remoteExceptionWrapper = new DataSourceExceptionWrapper(_remoteDataSource);
            _dataSource = new DataSource(remoteExceptionWrapper, region, maxPageResult, _deviceHistory, quality, (IConnectionListener) _remoteDataSource);
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

        internal IConnectionListener ConnectionListener
        {
            get { return (IConnectionListener)_remoteDataSource; }
        }
    }
}
