using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.YouTube.v3.Data;

namespace LiteTube.DataClasses
{
    public interface IPlaylistItemContentDetails
    {
        string EndAt { get; }
        string ETag { get; }
        string Note { get; }
        string StartAt { get; }
        string VideoId { get; }
    }

    public interface IResourceId
    {
        string ChannelId { get; }
        string ETag { get; }
        string Kind { get; }
        string PlaylistId { get; }
        string VideoId { get; }
    }

    public interface IThumbnail
    {
        string ETag { get; }
        long? Height { get; }
        string Url { get; }
        long? Width { get; }
    }

    public interface IThumbnailDetails
    {
        IThumbnail Default { get; }
        string ETag { get; }
        IThumbnail High { get; }
        IThumbnail Maxres { get; }
        IThumbnail Medium { get; }
        IThumbnail Standard { get; }
        IThumbnail GetDefaultThumbnail();
    }

    public interface IPlaylistItemSnippet
    {
        string ChannelId { get; }
        string ChannelTitle { get; }
        string Description { get; }
        string ETag { get; }
        string PlaylistId { get; }
        long? Position { get; }
        DateTime? PublishedAt { get; }
        string PublishedAtRaw { get; }
        IResourceId ResourceId { get; }
        IThumbnailDetails Thumbnails { get; }
        string Title { get; }
    }

    public interface IPlaylistItemStatus
    {
        string ETag { get; }
        string PrivacyStatus { get; } //TODO enum
    }

    public interface IPlayListItem
    {
        string ETag { get; }
        string Id { get; }
        IPlaylistItemContentDetails ContentDetails { get; }
        IPlaylistItemSnippet Snippet { get; }
        IPlaylistItemStatus Status { get; }
    }

    class MPlayListItem : IPlayListItem
    {
        public MPlayListItem(PlaylistItem playListItem)
        {
            ContentDetails = new MPlaylistItemContentDetails(playListItem.ContentDetails);
            Snippet = new MPlaylistItemSnippet(playListItem.Snippet);
            Status = new MPlaylistItemStatus(playListItem.Status);
            ETag = playListItem.ETag;
            Id = playListItem.Id;
        }

        public IPlaylistItemContentDetails ContentDetails
        {
            get;
            private set;
        }

        public IPlaylistItemSnippet Snippet
        {
            get;
            private set;
        }

        public IPlaylistItemStatus Status
        {
            get;
            private set;
        }

        public string ETag
        {
            get;
            private set;
        }

        public string Id
        {
            get;
            private set;
        }
    }

    class MPlaylistItemContentDetails : IPlaylistItemContentDetails
    {
        public MPlaylistItemContentDetails(PlaylistItemContentDetails contentDetails)
        {
            EndAt = contentDetails.EndAt;
            ETag = contentDetails.ETag;
            Note = contentDetails.Note;
            StartAt = contentDetails.StartAt;
            VideoId = contentDetails.VideoId;
        }

        public string EndAt
        {
            get;
            private set;
        }

        public string ETag
        {
            get;
            private set;
        }

        public string Note
        {
            get;
            private set;
        }

        public string StartAt
        {
            get;
            private set;
        }

        public string VideoId
        {
            get;
            private set;
        }
    }

    class MResourceId : IResourceId
    {
        public MResourceId(ResourceId resourceId)
        {
            ChannelId = resourceId.ChannelId;
            ETag = resourceId.ETag;
            Kind = resourceId.Kind;
            PlaylistId = resourceId.PlaylistId;
            VideoId = resourceId.VideoId;
        }

        public string ChannelId
        {
            get;
            private set;
        }

        public string ETag
        {
            get;
            private set;
        }

        public string Kind
        {
            get;
            private set;
        }

        public string PlaylistId
        {
            get;
            private set;
        }

        public string VideoId
        {
            get;
            private set;
        }
    }

    class MThumbnail : IThumbnail
    {
        public MThumbnail(Thumbnail thumbnail)
        {
            if (thumbnail == null)
                return;

            ETag = thumbnail.ETag;
            Height = thumbnail.Height;
            Url = thumbnail.Url;
            Width = thumbnail.Width;
        }

        public string ETag
        {
            get;
            private set;
        }

        public long? Height
        {
            get;
            private set;
        }

        public string Url
        {
            get;
            private set;
        }

        public long? Width
        {
            get;
            private set;
        }

        public static IThumbnail Empty
        {
            get { return new MThumbnail(null); }
        }
    }

    class MThumbnailDetails : IThumbnailDetails
    {
        private readonly ThumbnailDetails _thumbnailDetails;
        public MThumbnailDetails(ThumbnailDetails thumbnailDetails)
        {
            if (thumbnailDetails == null)
                return;

            _thumbnailDetails = thumbnailDetails;
            High = new MThumbnail(thumbnailDetails.High);
            Maxres = new MThumbnail(thumbnailDetails.Maxres);
            Medium = new MThumbnail(thumbnailDetails.Medium);
            Standard = new MThumbnail(thumbnailDetails.Standard);
        }

        public IThumbnail Default
        {
            get;
            private set;
        }

        public string ETag
        {
            get;
            private set;
        }

        public IThumbnail High
        {
            get;
            private set;
        }

        public IThumbnail Maxres
        {
            get;
            private set;
        }

        public IThumbnail Medium
        {
            get;
            private set;
        }

        public IThumbnail Standard
        {
            get;
            private set;
        }

        public IThumbnail GetDefaultThumbnail()
        {
            if (_thumbnailDetails == null)
                return MThumbnail.Empty;

            if (_thumbnailDetails.Medium != null)
                return new MThumbnail(_thumbnailDetails.Medium);

            if (_thumbnailDetails.Standard != null)
                return new MThumbnail(_thumbnailDetails.Standard);

            if (_thumbnailDetails.High != null)
                return new MThumbnail(_thumbnailDetails.High);

            return new MThumbnail(_thumbnailDetails.Maxres);
        }
    }

    class MPlaylistItemSnippet : IPlaylistItemSnippet
    {
        public MPlaylistItemSnippet(PlaylistItemSnippet playlistItemSnippet)
        {
            ChannelId = playlistItemSnippet.ChannelId;
            ChannelTitle = playlistItemSnippet.ChannelTitle;
            Description = playlistItemSnippet.Description;
            ETag = playlistItemSnippet.ETag;
            PlaylistId = playlistItemSnippet.PlaylistId;
            Position = playlistItemSnippet.Position;
            PublishedAt = playlistItemSnippet.PublishedAt;
            PublishedAtRaw = playlistItemSnippet.PublishedAtRaw;
            ResourceId = new MResourceId(playlistItemSnippet.ResourceId);
            Thumbnails = new MThumbnailDetails(playlistItemSnippet.Thumbnails);
            Title = playlistItemSnippet.Title;
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

        public string PlaylistId
        {
            get;
            private set;
        }

        public long? Position
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

        public IResourceId ResourceId
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
    }

    class MPlaylistItemStatus : IPlaylistItemStatus
    {
        public MPlaylistItemStatus(PlaylistItemStatus playlistStatus)
        {
            if (playlistStatus == null)
                return;

            ETag = playlistStatus.ETag;
            PrivacyStatus = playlistStatus.PrivacyStatus;
        }

        public string ETag
        {
            get;
            private set;
        }

        public string PrivacyStatus
        {
            get;
            private set;
        }
    }
}
