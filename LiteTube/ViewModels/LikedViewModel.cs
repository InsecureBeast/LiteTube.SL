using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.ViewModels.Nodes;

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
            var itemsList = Items.ToList();
            foreach (var item in items)
            {
                if (itemsList.Exists(i => i.Id == item.ContentDetails.VideoId))
                    continue;
                Items.Add(new PlayListItemNodeViewModel(item));
            }

            IsLoading = false;
            if (!Items.Any())
                IsEmpty = true;
        }
    }
}
