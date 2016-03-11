using System;
using System.Threading;
using LiteTube.Common;
using LiteTube.Common.Helpers;

namespace LiteTube.DataModel
{
    class Context : IListener<ConnectionEventArgs>, IDisposable
    {
        private readonly IDeviceHistory _deviceHistory;
        private RemoteDataSource _remoteDataSource;
        private readonly BaseConnectionListener _baseConnectionListener;
        private IDataSource _dataSource;

        public Context(IDeviceHistory deviceHistory)
        {
            _deviceHistory = deviceHistory;
            var connectionListener = new ConnectionListener();
            connectionListener.Subscribe(this);
            _baseConnectionListener = new BaseConnectionListener();
        }

        public void BuidContext()
        {
            var youTubeService = new YouTubeServiceControl();
            var region = SettingsHelper.GetRegion();
            var quality = SettingsHelper.GetQuality();
            const int maxPageResult = 30;
            if (!_baseConnectionListener.CheckNetworkAvailability())
            {
                _dataSource = new NullableDataSource();
                return;
            }

            _remoteDataSource = new RemoteDataSource(youTubeService);
            var remoteExceptionWrapper = new DataSourceExceptionWrapper(_remoteDataSource);
            _dataSource = new DataSource(remoteExceptionWrapper, region, maxPageResult, _deviceHistory, quality);
        }

        internal IDataSource DataSource
        {
            get { return _dataSource; }
        }

        internal IConnectionListener ConnectionListener
        {
            get { return _baseConnectionListener; }
        }

        public async void Notify(ConnectionEventArgs e)
        {
            if (!e.IsConnected)
            {
                Dispose();
                _baseConnectionListener.Notify(e);
                return;
            }

            if (_remoteDataSource != null)
                return;

            BuidContext();

            if (SettingsHelper.IsContainsAuthorizationData())
                await _dataSource.LoginSilently(string.Empty);
            
            _baseConnectionListener.Notify(e);
        }

        public void Dispose()
        {
            _remoteDataSource = null;
            _dataSource = new NullableDataSource();
        }
    }
}
