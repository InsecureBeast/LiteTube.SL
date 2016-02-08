using Google.Apis.YouTube.v3.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using LiteTube.Common;

namespace LiteTube.DataClasses
{
    public enum Definition
    {
        HD = 0
    }
    
    public interface IVideoDetails
    {
        TimeSpan Duration { get; }
        string VideoId { get; }
        string Title { get; }
        string Description { get; }
        string Dimension { get; }
        string Definition { get; }
        IVideo Video { get; }
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

    public interface IVideo
    {
        string Id { get; }
        IVideoStatistics Statistics { get; }
        Uri PlayerUri { get; }
        string EmbedHtml { get; }
    }

    public interface IVideoItem
    {
        string ChannelId { get; }
        string ChannelTitle { get; set; }
        string ETag { get; }
        IThumbnailDetails Thumbnails { get; }
        DateTime? PublishedAt { get; }
        string PublishedAtRaw { get; }
        IVideoDetails Details { get; }
    }

    public interface IVideoList : IResponceList
    {
        IList<IVideoItem> Items { get; }
    }

    class MVideoList : IVideoList
    {
        public MVideoList(VideoListResponse response)
        {
            if (response == null)
            {
                Items = new List<IVideoItem>();
                return;
            }

            ETag = response.ETag;
            NextPageToken = response.NextPageToken;
            PageInfo = new MPageInfo(response.PageInfo);
            PrevPageToken = response.PrevPageToken;
            var items = response.Items.Where(i => i.Snippet.Title != "Deleted video");
            Items = items.Select(i => new MVideoItem(i)).ToList<IVideoItem>();
            Id = Guid.NewGuid().ToString();
            EventId = response.EventId;
            TokenPagination = new MTokenPagination(response.TokenPagination);
            VisitorId = response.VisitorId;
        }

        public MVideoList(SearchListResponse response, IEnumerable<IVideoDetails> videoDetails)
        {
            ETag = response.ETag;
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
            Id = Guid.NewGuid().ToString();
            EventId = response.EventId;
            TokenPagination = new MTokenPagination(response.TokenPagination);
            VisitorId = response.VisitorId;
        }

        public MVideoList(VideoListResponse response, IPlaylistItemList playListItems)
        {
            ETag = playListItems.ETag;
            NextPageToken = playListItems.NextPageToken;
            PageInfo = playListItems.PageInfo;
            PrevPageToken = playListItems.PrevPageToken;
            var items = response.Items.Where(i => i.Snippet.Title != "Deleted video");
            Items = items.Select(i => new MVideoItem(i)).ToList<IVideoItem>();
            Id = Guid.NewGuid().ToString();
            EventId = playListItems.EventId;
            TokenPagination = playListItems.TokenPagination;
            VisitorId = playListItems.VisitorId;
        }

        public MVideoList()
        {
            Items = new List<IVideoItem>();
        }

        public string ETag
        {
            get;
            private set;
        }

        public IList<IVideoItem> Items
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


        public string Id
        {
            get;
            private set;
        }

        public string EventId
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
                Title = video.Snippet.Localized.Title;
                Description = video.Snippet.Localized.Description;
            }
            Definition = video.ContentDetails.Definition;
            Video = new MVideo(video);
            Statistics = new MVideoStatistics(video.Statistics);
            IsPaid = video.ContentDetails.RegionRestriction != null;
            //if (video.ContentDetails.RegionRestriction != null)
            //{
            //    var isBlocked = video.ContentDetails.RegionRestriction.Allowed.C
            //}
            
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

        public IVideo Video
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

    class MVideo : IVideo
    {
        public MVideo(Video video)
        {
            this.Id = video.Id;
            this.Statistics = new MVideoStatistics(video.Statistics);
        }

        public string Id
        {
            get;
            private set;
        }

        public IVideoStatistics Statistics
        {
            get;
            private set;
        }

        public Uri PlayerUri
        {
            get;
            private set;
        }

        public string EmbedHtml
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
            ETag = video.ETag;
            Thumbnails = new MThumbnailDetails(video.Snippet.Thumbnails);
            PublishedAt = video.Snippet.PublishedAt;
            PublishedAtRaw = video.Snippet.PublishedAtRaw;
            Details = new MVideoDetails(video);
        }

        public MVideoItem(SearchResult item, IVideoDetails details)
        {
            ChannelId = item.Snippet.ChannelId;
            ChannelTitle = item.Snippet.ChannelTitle;
            ETag = item.ETag;
            Thumbnails = new MThumbnailDetails(item.Snippet.Thumbnails);
            PublishedAt = item.Snippet.PublishedAt;
            PublishedAtRaw = item.Snippet.PublishedAtRaw;
            Details = details;
        }

        public MVideoItem(IPlayListItem item, IVideoDetails details)
        {
            ChannelId = item.Snippet.ChannelId;
            ChannelTitle = item.Snippet.ChannelTitle;
            ETag = item.ETag;
            Thumbnails = item.Snippet.Thumbnails;
            PublishedAt = item.Snippet.PublishedAt;
            PublishedAtRaw = item.Snippet.PublishedAtRaw;
            Details = details;
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

        public string ETag
        {
            get;
            private set;
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
