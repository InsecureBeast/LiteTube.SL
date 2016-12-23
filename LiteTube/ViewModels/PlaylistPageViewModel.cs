using System;
using System.Threading.Tasks;
using LiteTube.Common.Helpers;
using LiteTube.DataClasses;
using LiteTube.DataModel;

namespace LiteTube.ViewModels
{
    class PlaylistPageViewModel: SectionBaseViewModel
    {
        private readonly string _playlistId;

        public PlaylistPageViewModel(string playlistId, string title, Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
        {
            _playlistId = playlistId;
            Title = title;
            ShowAdv = SettingsHelper.IsAdvVisible;
            LayoutHelper.InvokeFromUiThread(async() => await FirstLoad());
        }

        public override string ToString()
        {
            return Title;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            if (_playlistId == _getDataSource().WatchLaterPlaylistId)
                return await _getDataSource().GetWatchLater(nextPageToken);

            return await _getDataSource().GetCategoryVideoList(_playlistId, nextPageToken);
        }

        public override void Notify(ConnectionEventArgs e)
        {
            base.Notify(e);
            if (e.IsConnected)
            {
                LayoutHelper.InvokeFromUiThread(async () =>
                {
                    await FirstLoad();
                });
            }
        }

        public void PlayAll()
        {
            var id = _getDataSource().WatchLaterPlaylistId;
            var view = string.Format("/PlaylistVideoPage.xaml", id);
            NavigationHelper.Navigate(view, new PlaylistVideoPageViewModel(id, _getDataSource, _connectionListener));
        }
    }
}
