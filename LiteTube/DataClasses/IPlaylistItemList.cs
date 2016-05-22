using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteTube.DataClasses
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

            ETag = response.ETag;
            EventId = response.EventId;
            Kind = response.Kind;
            NextPageToken = response.NextPageToken;
            PageInfo = new MPageInfo(response.PageInfo);
            PrevPageToken = response.PrevPageToken;
            TokenPagination = new MTokenPagination(response.TokenPagination);
            VisitorId = response.VisitorId;
            var items = response.Items.Where(i => i.Snippet.Title != "Deleted video");
            Items = items.Select(i => new MPlayListItem(i)).ToList<IPlayListItem>();
        }

        public static IPlaylistItemList Empty
        {
            get { return new MPlaylistItemList(null); }
        }

        public string ETag
        {
            get;
            private set;
        }

        public string EventId
        {
            get;
            private set;
        }

        public IList<IPlayListItem> Items
        {
            get;
            private set;
        }

        public string Kind
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

        public ITokenPagination TokenPagination
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
