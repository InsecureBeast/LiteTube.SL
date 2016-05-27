using Google.Apis.YouTube.v3.Data;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LiteTube.DataClasses
{
    public interface IActivitySnippet
    {
        string ChannelId { get; }
        string ChannelTitle { get; }
        string Description { get; }
        string GroupId { get; }
        DateTime? PublishedAt { get; }
        string PublishedAtRaw { get; }
        IThumbnailDetails Thumbnails { get; }
        string Title { get; }
        string Type { get; }
    }

    public interface IActivity
    {
        string Id { get; }
        string Kind { get; }
        IActivitySnippet Snippet { get; }
    }

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

    class MActivity : IActivity
    {
        public MActivity(Activity activity)
        {
            if (activity == null)
                return;

            Id = activity.Id;
            Kind = activity.Kind;
            Snippet = new MActivitySnippet(activity.Snippet);
        }

        public string Id
        {
            get;
            private set;
        }

        public string Kind
        {
            get;
            private set;
        }

        public IActivitySnippet Snippet
        {
            get;
            private set;
        }
    }

    class MActivitySnippet : IActivitySnippet
    {
        public MActivitySnippet(ActivitySnippet snippet)
        {
            if (snippet == null)
                return;

            ChannelId = snippet.ChannelId;
            ChannelTitle = snippet.ChannelTitle;
            Description = snippet.Description;
            GroupId = snippet.GroupId;
            PublishedAt = snippet.PublishedAt;
            PublishedAtRaw = snippet.PublishedAtRaw;
            Thumbnails = new MThumbnailDetails(snippet.Thumbnails);
            Title = snippet.Title;
            Type = snippet.Type;
        }

        public string ChannelId
        {
            get;
            private set;
        }

        public string ChannelTitle
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public string GroupId
        {
            get;
            private set;
        }

        public DateTime? PublishedAt
        {
            get;
            private set;
        }

        public string PublishedAtRaw
        {
            get;
            private set;
        }

        public IThumbnailDetails Thumbnails
        {
            get;
            private set;
        }

        public string Title
        {
            get;
            private set;
        }

        public string Type
        {
            get;
            private set;
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
