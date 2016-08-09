using Google.Apis.YouTube.v3.Data;

namespace LiteTube.Core.DataClasses
{
    public interface IChannelStatistics
    {
        ulong? ViewCount { get; }
        ulong? CommentCount { get; }
        ulong? SubscriberCount { get; }
        bool? HiddenSubscriberCount { get; }
        ulong? VideoCount { get; }
    }
    
    public interface IChannel
    {
        string Id { get; }
        string Title { get; }
        string Description { get; }
        IThumbnailDetails Thumbnails { get; }
        IChannelStatistics Statistics { get; }
        string Image { get; }
    }

    public class MChannel : IChannel
    {
        public MChannel(Channel channel)
        {
            if (channel == null)
                return;
            
            Id = channel.Id;
            if (channel.BrandingSettings != null)
            {
                if (channel.BrandingSettings.Image != null)
                    Image = channel.BrandingSettings.Image.BannerMobileImageUrl;
            }

            Thumbnails = new MThumbnailDetails();
            Statistics = new MChannelStatistics();

            if (channel.Snippet == null)
                return;

            Title = channel.Snippet.Title;
            Description = channel.Snippet.Description;
            Thumbnails = new MThumbnailDetails(channel.Snippet.Thumbnails);
            Statistics = new MChannelStatistics(channel.Statistics);
        }

        public MChannel(SearchResult channel)
        {
            if (channel == null)
                return;

            if (channel.Id != null)
                Id = channel.Id.ChannelId;

            Thumbnails = new MThumbnailDetails();
            Statistics = new MChannelStatistics();

            if (channel.Snippet == null)
                return;

            Title = channel.Snippet.Title;
            Description = channel.Snippet.Description;
            Thumbnails = new MThumbnailDetails(channel.Snippet.Thumbnails);
        }

        public static IChannel Empty
        {
            get { return new MChannel((Channel)null);}
        }

        public string Id
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

        public IThumbnailDetails Thumbnails
        {
	        get;
            private set;
        }

        public IChannelStatistics Statistics
        {
	        get;
            private set;
        }

        public string Image
        {
            get;
            private set;
        }
    }

    public class MChannelStatistics : IChannelStatistics
    {
        public MChannelStatistics(ChannelStatistics channelStatistics)
        {
            ViewCount = channelStatistics.ViewCount;
            CommentCount = channelStatistics.CommentCount;
            SubscriberCount = channelStatistics.SubscriberCount;
            HiddenSubscriberCount = channelStatistics.HiddenSubscriberCount;
            VideoCount = channelStatistics.VideoCount;
        }

        public MChannelStatistics()
        {
        }

        public ulong? ViewCount
        {
	        get;
            private set;
        }

        public ulong? CommentCount
        {
	        get;
            private set;
        }

        public ulong? SubscriberCount
        {
	        get;
            private set;
        }

        public bool? HiddenSubscriberCount
        {
	        get;
            private set;
        }

        public ulong? VideoCount
        {
	        get;
            private set;
        }
    }
}
