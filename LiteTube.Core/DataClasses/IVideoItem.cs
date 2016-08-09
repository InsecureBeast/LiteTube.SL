using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.YouTube.v3.Data;
using LiteTube.Core.Converters;

namespace LiteTube.Core.DataClasses
{
    public interface IVideoDetails
    {
        TimeSpan Duration { get; }
        string VideoId { get; }
        string Title { get; }
        string Description { get; }
        string Dimension { get; }
        string Definition { get; }
        IVideoStatistics Statistics { get; }
        bool IsPaid { get; }
    }

    public interface IVideoStatistics
    {
        UInt64? ViewCount { get; }
        UInt64? LikeCount { get; }
        UInt64? DislikeCount { get; }
        UInt64? FavoriteCount { get; }
        UInt64? CommentCount { get; }
    }

    public interface IVideoItem
    {
        string ChannelId { get; }
        string ChannelTitle { get; set; }
        IThumbnailDetails Thumbnails { get; }
        DateTime? PublishedAt { get; }
        string PublishedAtRaw { get; }
        IVideoDetails Details { get; }
    }

    public interface IVideoList : IResponceList
    {
        List<IVideoItem> Items { get; }
    }

    class MVideoList : IVideoList
    {
        public MVideoList(VideoListResponse response) : this()
        {
            if (response == null)
            {
                Items = new List<IVideoItem>();
                return;
            }

            NextPageToken = response.NextPageToken;
            PageInfo = new MPageInfo(response.PageInfo);
            PrevPageToken = response.PrevPageToken;
            var items = response.Items.Where(i => i.Snippet.Title != "Deleted video");
            Items = items.Select(i => new MVideoItem(i)).ToList<IVideoItem>();
            VisitorId = response.VisitorId;
        }

        public MVideoList(SearchListResponse response, IEnumerable<IVideoDetails> videoDetails): this()
        {
            NextPageToken = response.NextPageToken;
            PageInfo = new MPageInfo(response.PageInfo);
            PrevPageToken = response.PrevPageToken;
            var items = response.Items.Where(i => i.Snippet.Title != "Deleted video");
            Items = new List<IVideoItem>();
            foreach (var details in videoDetails)
            {
                var item = items.FirstOrDefault(i => i.Id.VideoId == details.VideoId);
                if (item != null)
                    Items.Add(new MVideoItem(item, details));
            }
            VisitorId = response.VisitorId;
        }

        public MVideoList(VideoListResponse response, IPlaylistItemList playListItems): this()
        {
            NextPageToken = playListItems.NextPageToken;
            PageInfo = playListItems.PageInfo;
            PrevPageToken = playListItems.PrevPageToken;
            var items = response.Items.Where(i => i.Snippet.Title != "Deleted video");
            Items = items.Select(i => new MVideoItem(i)).ToList<IVideoItem>();
            VisitorId = playListItems.VisitorId;
        }

        public MVideoList()
        {
            Items = new List<IVideoItem>();
            NextPageToken = string.Empty;
            PageInfo = new MPageInfo();
            PrevPageToken = string.Empty;
            VisitorId = string.Empty;
        }

        public List<IVideoItem> Items
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

        public static IVideoList Empty
        {
            get { return new MVideoList(); }
        }
    }
    
    class MVideoDetails : IVideoDetails
    {
        public MVideoDetails(Video video)
        {
            Duration = DurationConverter.Convert(video.ContentDetails.Duration);
            VideoId = video.Id;
            if (video.Snippet != null)
            {
                Title = video.Snippet.Title;
                Description = video.Snippet.Description;
            }
            Definition = video.ContentDetails.Definition;
            Statistics = new MVideoStatistics(video.Statistics);
            IsPaid = video.ContentDetails.RegionRestriction != null;
        }

        public TimeSpan Duration
        {
            get;
            private set;
        }

        public string VideoId
        {
            get;
            private set;
        }

        public string Title
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public string Dimension
        {
            get;
            private set;
        }

        public string Definition
        {
            get;
            private set;
        }

        public IVideoStatistics Statistics
        {
            get;
            private set;
        }

        public bool IsPaid
        {
            get;
            private set;
        }
    }

    class MVideoStatistics : IVideoStatistics
    {
        public MVideoStatistics(VideoStatistics videoStatistics)
        {
            ViewCount = videoStatistics.ViewCount;
            LikeCount = videoStatistics.LikeCount;
            DislikeCount = videoStatistics.DislikeCount;
            FavoriteCount = videoStatistics.FavoriteCount;
            CommentCount = videoStatistics.CommentCount;
        }

        public UInt64? ViewCount
        {
            get;
            private set;
        }

        public UInt64? LikeCount
        {
            get;
            private set;
        }

        public UInt64? DislikeCount
        {
            get;
            private set;
        }

        public UInt64? FavoriteCount
        {
            get;
            private set;
        }

        public UInt64? CommentCount
        {
            get;
            private set;
        }
    }

    class MVideoItem : IVideoItem
    {
        public static IVideoItem Empty
        {
            get { return new MVideoItem(null); }
        }

        public MVideoItem(Video video)
        {
            if (video == null)
                return;

            ChannelId = video.Snippet.ChannelId;
            ChannelTitle = video.Snippet.ChannelTitle;
            Thumbnails = new MThumbnailDetails(video.Snippet.Thumbnails);
            PublishedAt = video.Snippet.PublishedAt;
            PublishedAtRaw = video.Snippet.PublishedAtRaw;
            Details = new MVideoDetails(video);
        }

        public MVideoItem(SearchResult item, IVideoDetails details)
        {
            Details = details;
            if (item.Snippet == null)
                return;

            ChannelId = item.Snippet.ChannelId;
            ChannelTitle = item.Snippet.ChannelTitle;
            Thumbnails = new MThumbnailDetails(item.Snippet.Thumbnails);
            PublishedAt = item.Snippet.PublishedAt;
            PublishedAtRaw = item.Snippet.PublishedAtRaw;
        }

        public MVideoItem(IPlayListItem item, IVideoDetails details)
        {
            Details = details;
            if (item.Snippet == null)
                return;

            ChannelId = item.Snippet.ChannelId;
            ChannelTitle = item.Snippet.ChannelTitle;
            Thumbnails = item.Snippet.Thumbnails;
            PublishedAt = item.Snippet.PublishedAt;
            PublishedAtRaw = item.Snippet.PublishedAtRaw;
        }

        public string ChannelId
        {
            get;
            private set;
        }

        public string ChannelTitle
        {
            get;
            set;
        }

        public IThumbnailDetails Thumbnails
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

        public IVideoDetails Details
        {
            get; 
            private set;
        }
        
        public static string GetVideoId(ActivityContentDetails details)
        {
            if (details == null)
                throw new ArgumentException("details");

            if (details.Bulletin != null)
                return details.Bulletin.ResourceId.VideoId;

            if (details.ChannelItem != null)
                return details.ChannelItem.ResourceId.VideoId;

            if (details.Comment != null)
                return details.Comment.ResourceId.VideoId;

            if (details.Favorite != null)
                return details.Favorite.ResourceId.VideoId;

            if (details.Like != null)
                return details.Like.ResourceId.VideoId;

            if (details.PlaylistItem != null)
                return details.PlaylistItem.ResourceId.VideoId;

            if (details.PromotedItem != null)
                return details.PromotedItem.VideoId;

            if (details.Recommendation != null)
                return details.Recommendation.ResourceId.VideoId;

            if (details.Social != null)
                return details.Social.ResourceId.VideoId;

            if (details.Subscription != null)
                return details.Subscription.ResourceId.VideoId;

            if (details.Upload != null)
                return details.Upload.VideoId;

            return null;
        }
    }
}
