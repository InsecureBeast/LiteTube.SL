using Google.Apis.YouTube.v3.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace LiteTube.DataClasses
{
    public interface IPlaylistContentDetails
    {
        string ETag { get; }
        long? ItemCount { get; }
    }

    public interface IPlaylistLocalization
    {
        string Description { get; }
        string ETag { get; }
        string Title { get; }
    }

    public interface IPlaylistPlayer
    {
        string EmbedHtml { get; }
        string ETag { get; }
    }

    public interface IPlaylistSnippet
    {
        string ChannelId { get; }
        string ChannelTitle { get; }
        string DefaultLanguage { get; }
        string Description { get; }
        string ETag { get; }
        IPlaylistLocalization Localized { get; }
        DateTime? PublishedAt { get; }
        string PublishedAtRaw { get; }
        IList<string> Tags { get; }
        IThumbnailDetails Thumbnails { get; }
        string Title { get; }
    }

    public interface IPlaylistStatus
    {
        string ETag { get; }
        string PrivacyStatus { get; }
    }
    
    public interface IPlaylist
    {
        IPlaylistContentDetails ContentDetails { get; }
        string ETag { get; }
        string Id { get; }
        string Kind { get; }
        IDictionary<string, IPlaylistLocalization> Localizations { get; }
        IPlaylistPlayer Player { get; }
        IPlaylistSnippet Snippet { get; }
        IPlaylistStatus Status { get; }
    }

    class MPlaylist : IPlaylist
    {
        public MPlaylist(Playlist playlist)
        {
            ContentDetails = new MPlaylistContentDetails(playlist.ContentDetails);
            ETag = playlist.ETag;
            Id = playlist.Id;
            Kind = playlist.Kind;
            if (playlist.Localizations != null)
            {
                Localizations = new Dictionary<string, IPlaylistLocalization>();
                foreach (var localization in playlist.Localizations)
                {
                    Localizations.Add(localization.Key, new MPlaylistLocalization(localization.Value));
                }
            }
            Player = new MPlaylistPlayer(playlist.Player);
            Snippet = new MPlaylistSnippet(playlist.Snippet);
            Status = new MPlaylistStatus(playlist.Status);
        }

        public IPlaylistContentDetails ContentDetails
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

        public IPlaylistPlayer Player
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
    }

    class MPlaylistContentDetails : IPlaylistContentDetails
    {
        public MPlaylistContentDetails(PlaylistContentDetails details)
        {
            ETag = details.ETag;
            ItemCount = details.ItemCount;
        }

        public string ETag
        {
            get;
            private set;
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
            Description = playlistLocalization.Description;
            ETag = playlistLocalization.ETag;
            Title = playlistLocalization.Title;
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

        public string Title
        {
            get;
            private set;
        }
    }

    class MPlaylistPlayer : IPlaylistPlayer
    {
        public MPlaylistPlayer(PlaylistPlayer player)
        {
            EmbedHtml = player.EmbedHtml;
            ETag = player.ETag;
        }

        public string EmbedHtml
        {
            get;
            private set;
        }

        public string ETag
        {
            get;
            private set;
        }
    }

    class MPlaylistSnippet : IPlaylistSnippet
    {
        public MPlaylistSnippet(PlaylistSnippet snippet)
        {
            ChannelId = snippet.ChannelId;
            ChannelTitle = snippet.ChannelTitle;
            DefaultLanguage = snippet.DefaultLanguage;
            Description = snippet.Description;    
            ETag = snippet.ETag;
            Localized = new MPlaylistLocalization(snippet.Localized);
            PublishedAt = snippet.PublishedAt;
            PublishedAtRaw = snippet.PublishedAtRaw;
            Tags = snippet.Tags;
            Thumbnails = new MThumbnailDetails(snippet.Thumbnails);
            Title = snippet.Title;
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

        public string ETag
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

            ETag = status.ETag;
            PrivacyStatus = status.PrivacyStatus;
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
