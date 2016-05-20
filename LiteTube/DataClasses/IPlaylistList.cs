using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.YouTube.v3.Data;

namespace LiteTube.DataClasses
{
    public interface IPlaylistList : IResponceList
    {
        IList<IPlaylist> Items { get; }
    }

    class MPlaylistList : IPlaylistList
    {
        public MPlaylistList()
        {
        }

        public MPlaylistList(PlaylistListResponse response)
        {
            ETag = response.ETag;
            EventId = response.EventId;
            NextPageToken = response.NextPageToken;
            PrevPageToken = response.PrevPageToken;
            PageInfo = new MPageInfo(response.PageInfo);
            TokenPagination = new MTokenPagination(response.TokenPagination);
            VisitorId = response.VisitorId;
            var items = response.Items;//.Where(i => i.Snippet.Title != "Deleted video");
            Items = items.Select(i => new MPlaylist(i)).ToList<IPlaylist>();
        }

        public static IPlaylistList Empty
        {
            get { return new MPlaylistList(); }
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

        public IList<IPlaylist> Items
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

