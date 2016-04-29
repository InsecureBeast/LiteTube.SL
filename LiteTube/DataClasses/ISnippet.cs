using Google.Apis.YouTube.v3.Data;
using System;

namespace LiteTube.DataClasses
{
    /// <summary>
    /// Интерфейс информации о видео. Не включает в себя само видео.
    /// так же выдается в поисковом запросе
    /// </summary>
    public interface ISnippet
    {
        string ChannelId { get; }
        string ChannelTitle { get; }
        string Description { get; }
        string ETag { get; }
        string LiveBroadcastContent { get; }
        DateTime? PublishedAt { get; }
        string PublishedAtRaw { get; }
        IThumbnailDetails Thumbnails { get; }
        string Title { get; }
        string VideoId { get; }
        IVideoDetails Details { get; }
    }

    class MSnippet : ISnippet
    {
        public MSnippet(SearchResult searchResult, IVideoDetails details)
        {
            ChannelId = searchResult.Snippet.ChannelId;
            ChannelTitle = searchResult.Snippet.ChannelTitle;
            Description = searchResult.Snippet.Description;
            ETag = searchResult.Snippet.ETag;
            LiveBroadcastContent = searchResult.Snippet.LiveBroadcastContent;
            PublishedAt = searchResult.Snippet.PublishedAt;
            PublishedAtRaw = searchResult.Snippet.PublishedAtRaw;
            Thumbnails = new MThumbnailDetails(searchResult.Snippet.Thumbnails);
            Title = searchResult.Snippet.Title;
            VideoId = searchResult.Id.VideoId;
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
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public string ETag
        {
            get;
            private set;
        }

        public string LiveBroadcastContent
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

        public string VideoId
        {
            get;
            private set;
        }

        public IVideoDetails Details
        {
            get;
            private set;
        }
    }
}
