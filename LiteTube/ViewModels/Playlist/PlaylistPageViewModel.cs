using System;
using System.Threading.Tasks;
using LiteTube.Common;
using LiteTube.Common.Helpers;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Linq;
using LiteTube.ViewModels.Nodes;
using System.Collections.Generic;
using LiteTube.Resources;

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
            var view = string.Format("/PlaylistVideoPage.xaml", _playlistId);
            NavigationHelper.Navigate(view, new PlaylistVideoPageViewModel(_playlistId, _getDataSource, _connectionListener));
        }

        internal override void LoadItems(IResponceList videoList)
        {
            var list = videoList as IPlaylistItemList;
            if (list == null)
                return;

            AddItems(list.Items);
        }

        internal void AddItems(IEnumerable<IPlayListItem> items)
        {
            var itemsList = Items.ToList();
            foreach (var item in items)
            {
                if (itemsList.Exists(i => i.Id == item.ContentDetails.VideoId))
                    continue;
                Items.Add(new PlayListItemNodeViewModel(item, _getDataSource(), Delete, GetContextMenuProvider()));
            }

            IsLoading = false;
            if (!Items.Any())
                IsEmpty = true;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getDataSource().GetPlaylistItems(_playlistId, nextPageToken);
        }

        protected override IContextMenuProvider GetContextMenuProvider()
        {
            return new ContextMenuProvider()
            {
                CanAddToPlayList = false,
                CanDelete = true
            };
        }

        private async Task Delete()
        {
            var items = Items.Where(i => ((PlayListItemNodeViewModel)i).IsSelected).ToList();
            foreach (var item in items)
            {
                await _getDataSource().RemoveItemFromPlaylist(item.Id);
                Items.Remove(item);
            }
        }
    }
}
