using System;
using System.Collections.Generic;
using System.Linq;
using LiteTube.Core.Common;
using LiteTube.Core.Common.Helpers;
using LiteTube.Core.DataClasses;
using LiteTube.Core.DataModel;
using LiteTube.Core.ViewModels.Nodes;
using LiteTube.Core.ViewModels.Playlist;

namespace LiteTube.Core.ViewModels.Search
{
    public class SearchPlaylistsViewModel : SearchBaseViewModel
    {
        public SearchPlaylistsViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener, Action<bool> changeProgressIndicator)
            : base(SearchType.Playlist, geDataSource, connectionListener, changeProgressIndicator)
        {
        }

        internal override void LoadItems(IResponceList responceList)
        {
            var list = responceList as IPlaylistList;
            if (list == null)
                return;

            AddItems(list.Items);
        }

        internal override void NavigateTo(NavigationObject navObject)
        {
            if (navObject == null)
                return;

            if (navObject.ViewModel == null)
                return;

            var viewModel = navObject.ViewModel as PlaylistNodeViewModel;
            if (viewModel == null)
                return;

            var playlistId = viewModel.Id;
            if (playlistId == null)
                return;

#if SILVERLIGHT
            NavigationHelper.Navigate("/PlaylistVideoPage.xaml", new PlaylistVideoPageViewModel(playlistId, _getGeDataSource, _connectionListener));
#endif
        }

        private void AddItems(IEnumerable<IPlaylist> items)
        {
            //var itemsList = Items.ToList();
            //foreach (var item in items)
            //{
            //    if (itemsList.Exists(i => i.Id == item.Id))
            //        continue;
            //    Items.Add(new PlaylistNodeViewModel(item));
            //}

            var itemsList = Items.ToList();
            var itemsArray = items.ToArray();
            for (int i = 0; i < itemsArray.Length; i++)
            {
                var item = itemsArray[i];

                if (itemsList.Exists(c => c.Id == item.Id))
                   continue;

                if (i % SettingsHelper.AdvCount == 0 && i != 0 && ShowAdv)
                {
                    Items.Add(new AddNodeViewModel());
                }

                Items.Add(new PlaylistNodeViewModel(item));
            }
        }
    }
}
