using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.Common.Helpers;
using LiteTube.Common;
using LiteTube.ViewModels.Nodes;

namespace LiteTube.ViewModels.Search
{
    class SearchPlaylistsViewModel : SearchBaseViewModel
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

                AdvHelper.AddAdv(Items, ShowAdv);
                Items.Add(new PlaylistNodeViewModel(item));
            }
        }
    }
}
