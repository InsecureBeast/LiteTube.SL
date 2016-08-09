using System.Collections.Generic;
using System.Linq;
using Google.Apis.YouTube.v3.Data;

namespace LiteTube.Core.DataClasses
{
    public interface IPlaylistItemList : IResponceList
    {
        IList<IPlayListItem> Items { get; }
    }

    class MPlaylistItemList : IPlaylistItemList
    {
        public MPlaylistItemList(PlaylistItemListResponse response)
        {
            if (response == null)
                return;

            NextPageToken = response.NextPageToken;
            PageInfo = new MPageInfo(response.PageInfo);
            PrevPageToken = response.PrevPageToken;
            VisitorId = response.VisitorId;
            var items = response.Items.Where(i => i.Snippet.Title != "Deleted video");
            Items = items.Select(i => new MPlayListItem(i)).ToList<IPlayListItem>();
        }

        public static IPlaylistItemList Empty
        {
            get { return new MPlaylistItemList(null); }
        }

        public IList<IPlayListItem> Items
        {
            get;
            private set;
        }

        public string NextPageToken
        {
            get;
            private set;
        }

        public IPageInfo PageInfo
        {
            get;
            private set;
        }

        public string PrevPageToken
        {
            get;
            private set;
        }

        public string VisitorId
        {
            get;
            private set;
        }
    }
}
