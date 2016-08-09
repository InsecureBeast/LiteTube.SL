using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.YouTube.v3.Data;

namespace LiteTube.Core.DataClasses
{
    class MActivityList : IVideoList
    {
        public MActivityList()
        {
            Items = new List<IVideoItem>();
        }

        public MActivityList(ActivityListResponse response, VideoListResponse videoList)
        {
            Kind = response.Kind;
            NextPageToken = response.NextPageToken;
            PageInfo = new MPageInfo(response.PageInfo);
            PrevPageToken = response.PrevPageToken;
            VisitorId = response.VisitorId;
            Items = videoList.Items.Select(i => new MVideoItem(i)).ToList<IVideoItem>();
            Id = Guid.NewGuid().ToString();
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

        public string VisitorId
        {
            get;
            private set;
        }

        public List<IVideoItem> Items
        {
            get; 
            private set;
        }

        public string Id
        {
            get;
            private set;
        }

        public static IVideoList Empty
        {
            get { return new MActivityList(); }
        }
    }

    class MPageInfo : IPageInfo
    {
        public MPageInfo(PageInfo pageInfo)
        {
            if (pageInfo == null)
                return;

            ETag = pageInfo.ETag;
            ResultsPerPage = pageInfo.ResultsPerPage;
            TotalResults = pageInfo.TotalResults;
        }

        public MPageInfo()
        {
            ETag = string.Empty;
        }

        public static IPageInfo Empty
        {
            get { return new MPageInfo(); }
        }

        public string ETag
        {
            get;
            private set;
        }

        public int? ResultsPerPage
        {
            get;
            private set;
        }

        public int? TotalResults
        {
            get;
            private set;
        }
    }
}
