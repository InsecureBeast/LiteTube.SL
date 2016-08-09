using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.Core.DataClasses;
using LiteTube.Core.DataModel;
using LiteTube.Core.ViewModels.Nodes;

namespace LiteTube.Core.ViewModels
{
    class FavoritesViewModel : SectionBaseViewModel
    {
        public FavoritesViewModel(Func<IDataSource> datasource, IConnectionListener connectionListener)
            : base(datasource, connectionListener)
        {
        }

        public override string ToString()
        {
            return Title;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            var res = await _getGeDataSource().GetFavorites(nextPageToken);
            return res;
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
                Items.Add(new PlayListItemNodeViewModel(item, Delete));
            }

            IsLoading = false;
            if (!Items.Any())
                IsEmpty = true;
        }

        private async Task Delete()
        {
            var items = Items.Where(i => ((PlayListItemNodeViewModel)i).IsSelected).ToList();
            foreach (var item in items)
            {
                await _getGeDataSource().RemoveFromFavorites(item.Id);
                Items.Remove(item);
            }
        }
    }
}
