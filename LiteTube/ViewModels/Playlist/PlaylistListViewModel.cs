using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.Common;
using LiteTube.Common.Helpers;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.ViewModels.Nodes;

namespace LiteTube.ViewModels.Playlist
{
    public class PlaylistListViewModel : SectionBaseViewModel
    {
        private readonly string _channelId;
        private readonly IContextMenuStrategy _contextMenuStrategy;

        public PlaylistListViewModel(string channelId, Func<IDataSource> getGeDataSource, 
            IConnectionListener connectionListener, IContextMenuStrategy contextMenuStrategy, Action<bool> changeProgressIndicator = null) 
            : base(getGeDataSource, connectionListener, null, changeProgressIndicator)
        {
            _channelId = channelId;
            _contextMenuStrategy = contextMenuStrategy;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            if (string.IsNullOrEmpty(_channelId))
                return MResponceList.Empty;

            return await _getDataSource().GetChannelPlaylistList(_channelId, nextPageToken);
        }

        internal override void LoadItems(IResponceList videoList)
        {
            var snippetList = videoList as IPlaylistList;
            if (snippetList == null)
                return;

            AddPlaylistItems(snippetList.Items);
        }

        internal override void NavigateTo(NavigationObject navObject)
        {
            if (navObject == null)
                return;

            if (navObject.ViewModel == null)
                return;

            var playlistViewModel = navObject.ViewModel as PlaylistNodeViewModel;
            if (playlistViewModel == null)
                return;

            if (playlistViewModel.ItemsCount == null)
                return;

            if (playlistViewModel.ItemsCount == 0)
                return;

            var id = playlistViewModel.Id;
            var view = string.Format("/PlaylistPage.xaml", id);
            if (string.IsNullOrEmpty(_channelId))
                NavigationHelper.Navigate(view, new PlaylistPageViewModel(id, playlistViewModel.Title, _getDataSource, _connectionListener));
            else
                NavigationHelper.Navigate(view, new ChannelPlaylistPageViewModel(id, playlistViewModel.Title, _getDataSource, _connectionListener));
        }

        private void AddPlaylistItems(IEnumerable<IPlaylist> items)
        {
            var itemsList = Items.ToList();
            var itemsArray = items.ToArray();
            for (int i = 0; i < itemsArray.Length; i++)
            {
                var item = itemsArray[i];
                if (item.ContentDetails == null)
                    continue;

                if (itemsList.Exists(c => c.Id == item.Id))
                    continue;

                Items.Add(new PlaylistNodeViewModel(item, _getDataSource(), _contextMenuStrategy, Delete));
            }

            //var itemsList = Items.ToList();
            //foreach (var item in items)
            //{
            //    if (item.ContentDetails == null)
            //        continue;

            //    if (itemsList.Exists(i => i.Id == item.Id))
            //        continue;

            //    Items.Add(new PlaylistNodeViewModel(item));
            //}
        }

        protected internal virtual async Task Delete(string playlistId)
        {
        }
    }
}
