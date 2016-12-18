using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.ViewModels.Nodes;
using LiteTube.Common;

namespace LiteTube.ViewModels
{
    class LikedViewModel : SectionBaseViewModel
    {
        public LikedViewModel(Func<IDataSource> getGeDataSource, IConnectionListener connectionListener) 
            : base(getGeDataSource, connectionListener)
        {
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getGeDataSource().GetLiked(nextPageToken);
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
            var menuProvider = new ContextMenuProvider()
            {
                CanAddToPlayList = false,
                CanDelete = false
            };
            
            var itemsList = Items.ToList();
            foreach (var item in items)
            {
                if (item == null)
                    continue;

                if (itemsList.Exists(i => i.Id == item.ContentDetails.VideoId))
                    continue;

                Items.Add(new PlayListItemNodeViewModel(item, _getGeDataSource(), Delete, menuProvider));
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
                //await _getGeDataSource().RemoveFromFavorites(item.Id);
                //Items.Remove(item);
            }
        }
    }
}
