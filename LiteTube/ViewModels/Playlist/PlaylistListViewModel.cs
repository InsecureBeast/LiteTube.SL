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

        public PlaylistListViewModel(string channelId, Func<IDataSource> getGeDataSource, IConnectionListener connectionListener, Action<bool> changeProgressIndicator = null) 
            : base(getGeDataSource, connectionListener, changeProgressIndicator)
        {
            _channelId = channelId;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
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

            var id = navObject.ViewModel.Id;
            var view = string.Format("/PlaylistVideoPage.xaml", id);
#if SILVERLIGHT
            NavigationHelper.Navigate(view, new PlaylistVideoPageViewModel(id, _getGeDataSource, _connectionListener));
#endif
        }

        private void AddPlaylistItems(IEnumerable<IPlaylist> items)
        {
            var itemsList = Items.ToList();
            foreach (var item in items)
            {
                if (item.ContentDetails == null)
                    continue;

                if (itemsList.Exists(i => i.Id == item.Id))
                    continue;

                Items.Add(new PlaylistNodeViewModel(item));
            }
        }
    }
}
