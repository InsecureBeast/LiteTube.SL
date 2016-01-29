using System;
using LiteTube.Common;

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
            _remoteDataSource = new RemoteDataSource(youTubeService);
            var region = SettingsHelper.GetRegion();
            var quality = SettingsHelper.GetQuality();
            const int maxPageResult = 30;
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

        public void Notify(ConnectionEventArgs e)
        {
            lock (this)
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
                _baseConnectionListener.Notify(e); 
            }
        }

        public void Dispose()
        {
            _remoteDataSource = null;
            _dataSource = new NullableDataSource();
        }
    }
}
