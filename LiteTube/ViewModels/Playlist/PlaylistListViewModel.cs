using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteTube.DataModel;
using LiteTube.DataClasses;
using LiteTube.ViewModels.Nodes;
using LiteTube.Common;
using LiteTube.Common.Helpers;

namespace LiteTube.ViewModels
{
    class PlaylistListViewModel : SectionBaseViewModel
    {
        private readonly string _channelId;

        public PlaylistListViewModel(string channelId, Func<IDataSource> getGeDataSource, 
            IConnectionListener connectionListener, Action<bool> changeProgressIndicator = null) 
            : base(getGeDataSource, connectionListener, changeProgressIndicator)
        {
            _channelId = channelId;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            if (string.IsNullOrEmpty(_channelId))
                return MResponceList.Empty;

            return await _getGeDataSource().GetChannelPlaylistList(_channelId, nextPageToken);
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
            var view = string.Format("/PlaylistVideoPage.xaml", id);
#if SILVERLIGHT
            NavigationHelper.Navigate(view, new PlaylistVideoPageViewModel(id, _getGeDataSource, _connectionListener));
#endif
        }

        private void AddPlaylistItems(IEnumerable<IPlaylist> items)
        {
            var menuProvider = new ContextMenuProvider()
            {
                CanAddToPlayList = false,
                CanDelete = false
            };

            var itemsList = Items.ToList();
            var itemsArray = items.ToArray();
            for (int i = 0; i < itemsArray.Length; i++)
            {
                var item = itemsArray[i];
                if (item.ContentDetails == null)
                    continue;

                if (itemsList.Exists(c => c.Id == item.Id))
                    continue;

                AdvHelper.AddAdv(Items, ShowAdv);
                Items.Add(new PlaylistNodeViewModel(item, _getGeDataSource(), menuProvider));
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
    }
}
