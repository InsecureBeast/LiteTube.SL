using System;
using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;

namespace LiteTube.Core.DataClasses
{
    public interface IPlaylistContentDetails
    {
        long? ItemCount { get; }
    }

    public interface IPlaylistLocalization
    {
        string Description { get; }
        string Title { get; }
    }

    public interface IPlaylistSnippet
    {
        string ChannelId { get; }
        string ChannelTitle { get; }
        string DefaultLanguage { get; }
        string Description { get; }
        IPlaylistLocalization Localized { get; }
        DateTime? PublishedAt { get; }
        string PublishedAtRaw { get; }
        IList<string> Tags { get; }
        IThumbnailDetails Thumbnails { get; }
        string Title { get; }
    }

    public interface IPlaylistStatus
    {
        string PrivacyStatus { get; }
    }
    
    public interface IPlaylist
    {
        IPlaylistContentDetails ContentDetails { get; }
        string Id { get; }
        string Kind { get; }
        IDictionary<string, IPlaylistLocalization> Localizations { get; }
        IPlaylistSnippet Snippet { get; }
        IPlaylistStatus Status { get; }
    }

    class MPlaylist : IPlaylist
    {
        public MPlaylist(Playlist playlist)
        {
            ContentDetails = new MPlaylistContentDetails(playlist.ContentDetails);
            Id = playlist.Id;
            Kind = playlist.Kind;
            Snippet = new MPlaylistSnippet(playlist.Snippet);
        }

        private MPlaylist()
        {
            Localizations = new Dictionary<string, IPlaylistLocalization>();
            ContentDetails = new MPlaylistContentDetails();
            Snippet = new MPlaylistSnippet();
            Status = new MPlaylistStatus();
            Kind = string.Empty;
        }

        public IPlaylistContentDetails ContentDetails
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

        public IDictionary<string, IPlaylistLocalization> Localizations
        {
            get;
            private set;
        }

        public IPlaylistSnippet Snippet
        {
            get;
            private set;
        }

        public IPlaylistStatus Status
        {
            get;
            private set;
        }

        public static IPlaylist Empty
        {
            get { return new MPlaylist();}
        }
    }

    class MPlaylistContentDetails : IPlaylistContentDetails
    {
        public MPlaylistContentDetails(PlaylistContentDetails details)
        {
            ItemCount = details.ItemCount;
        }

        public MPlaylistContentDetails()
        {
        }

        public long? ItemCount
        {
            get;
            private set;
        }
    }

    class MPlaylistLocalization : IPlaylistLocalization
    {
        public MPlaylistLocalization(PlaylistLocalization playlistLocalization)
        {
            if (playlistLocalization == null)
            {
                Description = string.Empty;
                Title = string.Empty;
                return;
            }
            Description = playlistLocalization.Description;
            Title = playlistLocalization.Title;
        }

        public MPlaylistLocalization()
        {
            Description = string.Empty;
            Title = string.Empty;
        }

        public string Description
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

    class MPlaylistSnippet : IPlaylistSnippet
    {
        public MPlaylistSnippet(PlaylistSnippet snippet) : this()
        {
            if (snippet == null)
                return;

            ChannelId = snippet.ChannelId;
            ChannelTitle = snippet.ChannelTitle;
            DefaultLanguage = snippet.DefaultLanguage;
            Description = snippet.Description;    
            Localized = new MPlaylistLocalization(snippet.Localized);
            PublishedAt = snippet.PublishedAt;
            PublishedAtRaw = snippet.PublishedAtRaw;
            Tags = snippet.Tags;
            Thumbnails = new MThumbnailDetails(snippet.Thumbnails);
            Title = snippet.Title;
        }

        public MPlaylistSnippet()
        {
            Thumbnails = new MThumbnailDetails();
            Localized = new MPlaylistLocalization();
            ChannelId = string.Empty;
            ChannelTitle = string.Empty;
            DefaultLanguage = string.Empty;
            Description = string.Empty;
            PublishedAt = DateTime.MinValue;
            PublishedAtRaw = string.Empty;
            Tags = new List<string>();
            Title = string.Empty;
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

        public string DefaultLanguage
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public IPlaylistLocalization Localized
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

        public IList<string> Tags
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

    class MPlaylistStatus : IPlaylistStatus
    {
        public MPlaylistStatus(PlaylistStatus status)
        {
            if (status == null)
                return;

            PrivacyStatus = status.PrivacyStatus;
        }

        public MPlaylistStatus()
        {
            PrivacyStatus = string.Empty;
        }

        public string PrivacyStatus
        {
            get;
            private set;
        }
    }
}
