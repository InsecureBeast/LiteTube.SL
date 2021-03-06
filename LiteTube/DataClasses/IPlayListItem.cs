﻿using System;
using Google.Apis.YouTube.v3.Data;

namespace LiteTube.DataClasses
{
    public interface IPlaylistItemContentDetails
    {
        string EndAt { get; }
        string Note { get; }
        string StartAt { get; }
        string VideoId { get; }
    }

    public interface IResourceId
    {
        string ChannelId { get; }
        string Kind { get; }
        string PlaylistId { get; }
        string VideoId { get; }
    }

    public interface IThumbnail
    {
        long? Height { get; }
        string Url { get; }
        long? Width { get; }
    }

    public interface IThumbnailDetails
    {
        string GetThumbnailUrl();
    }

    public interface IPlaylistItemSnippet
    {
        string ChannelId { get; }
        string ChannelTitle { get; }
        string Description { get; }
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
        string PrivacyStatus { get; } //TODO enum
    }

    public interface IPlayListItem
    {
        string Id { get; }
        IPlaylistItemContentDetails ContentDetails { get; }
        IPlaylistItemSnippet Snippet { get; }
        IPlaylistItemStatus Status { get; }
        string Kind { get; }
    }

    class MPlayListItem : IPlayListItem
    {
        public MPlayListItem(PlaylistItem playListItem)
        {
            ContentDetails = new MPlaylistItemContentDetails(playListItem.ContentDetails);
            Snippet = new MPlaylistItemSnippet(playListItem.Snippet);
            Status = new MPlaylistItemStatus(playListItem.Status);
            Id = playListItem.Id;
            Kind = playListItem.Kind;
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
    }

    class MPlaylistItemContentDetails : IPlaylistItemContentDetails
    {
        public MPlaylistItemContentDetails(PlaylistItemContentDetails contentDetails)
        {
            EndAt = contentDetails.EndAt;
            Note = contentDetails.Note;
            StartAt = contentDetails.StartAt;
            VideoId = contentDetails.VideoId;
        }

        public string EndAt
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
        private MResourceId()
        {
        }

        public MResourceId(ResourceId resourceId)
        {
            ChannelId = resourceId.ChannelId;
            Kind = resourceId.Kind;
            PlaylistId = resourceId.PlaylistId;
            VideoId = resourceId.VideoId;
        }

        public static IResourceId Empty
        {
            get { return new MResourceId(); }
        }

        public string ChannelId
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

            Height = thumbnail.Height;
            Url = thumbnail.Url;
            Width = thumbnail.Width;
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
        private readonly IThumbnail _default;
        private readonly IThumbnail _high;
        private readonly IThumbnail _maxres;
        private readonly IThumbnail _medium;
        private readonly IThumbnail _standard;

        public static IThumbnailDetails Empty
        {
            get { return new MThumbnailDetails(); }
        }

        public MThumbnailDetails(ThumbnailDetails thumbnailDetails)
        {
            if (thumbnailDetails == null)
                return;

            _high = new MThumbnail(thumbnailDetails.High);
            _maxres = new MThumbnail(thumbnailDetails.Maxres);
            _medium = new MThumbnail(thumbnailDetails.Medium);
            _standard = new MThumbnail(thumbnailDetails.Standard);
            _default = new MThumbnail(thumbnailDetails.Default__);
        }

        public MThumbnailDetails()
        {
        }

        public string GetThumbnailUrl()
        {
            if (_medium != null && !string.IsNullOrEmpty(_medium.Url))
                return _medium.Url;
            if (_maxres != null && !string.IsNullOrEmpty(_maxres.Url))
                return _maxres.Url;
            if (_high != null && !string.IsNullOrEmpty(_high.Url))
                return _high.Url;
            if (_standard != null && !string.IsNullOrEmpty(_standard.Url))
                return _standard.Url;
            if (_default != null && !string.IsNullOrEmpty(_default.Url))
                return _default.Url;
            return null;
        }
    }

    class MPlaylistItemSnippet : IPlaylistItemSnippet
    {
        public MPlaylistItemSnippet(PlaylistItemSnippet playlistItemSnippet)
        {
            if (playlistItemSnippet != null)
            {
                ChannelId = playlistItemSnippet.ChannelId;
                ChannelTitle = playlistItemSnippet.ChannelTitle;
                Description = playlistItemSnippet.Description;                PlaylistId = playlistItemSnippet.PlaylistId;
                Position = playlistItemSnippet.Position;
                PublishedAt = playlistItemSnippet.PublishedAt;
                PublishedAtRaw = playlistItemSnippet.PublishedAtRaw;
                ResourceId = new MResourceId(playlistItemSnippet.ResourceId);
                Thumbnails = new MThumbnailDetails(playlistItemSnippet.Thumbnails);
                Title = playlistItemSnippet.Title;
                return;
            }
            ResourceId = MResourceId.Empty;
            Thumbnails = MThumbnailDetails.Empty;
            ChannelId = playlistItemSnippet.ChannelId;
            ChannelTitle = playlistItemSnippet.ChannelTitle;
            Description = playlistItemSnippet.Description;
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

            PrivacyStatus = playlistStatus.PrivacyStatus;
        }

        public string PrivacyStatus
        {
            get;
            private set;
        }
    }
}
