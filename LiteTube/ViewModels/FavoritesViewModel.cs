using LiteTube.Common;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.ViewModels.Nodes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiteTube.ViewModels
{
    class FavoritesViewModel : SectionBaseViewModel
    {
        public FavoritesViewModel(IDataSource datasource, IConnectionListener connectionListener)
            : base(datasource, connectionListener)
        {
            //var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            //var arstring = resourceLoader.GetString("FavoritesSectionHeader");
            //_title = arstring;
            _title = "favorites"; //TODO localization
        }

        public override string ToString()
        {
            return Title;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            var res = await _dataSource.GetFavorites(nextPageToken);
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
                await _dataSource.RemoveFromFavorites(item.Id);
                Items.Remove(item);
            }
        }
    }
}
