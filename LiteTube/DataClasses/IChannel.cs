using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiteTube.DataClasses
{
    public interface IChannelStatistics
    {
        UInt64? ViewCount { get; }
        UInt64? CommentCount { get; }
        UInt64? SubscriberCount { get; }
        bool? HiddenSubscriberCount { get; }
        UInt64? VideoCount { get; }
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
                Image = channel.BrandingSettings.Image.BannerMobileImageUrl;
            Title = channel.Snippet.Localized.Title;
            Description = channel.Snippet.Localized.Description;
            Thumbnails = new MThumbnailDetails(channel.Snippet.Thumbnails);
            Statistics = new MChannelStatistics(channel.Statistics);
        }

        public static IChannel Empty
        {
            get { return new MChannel(null);}
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
            this.ViewCount = channelStatistics.ViewCount;
            this.CommentCount = channelStatistics.CommentCount;
            this.SubscriberCount = channelStatistics.SubscriberCount;
            this.HiddenSubscriberCount = channelStatistics.HiddenSubscriberCount;
            this.VideoCount = channelStatistics.VideoCount;
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
