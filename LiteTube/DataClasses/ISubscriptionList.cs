﻿using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;
using System.Linq;

namespace LiteTube.DataClasses
{
    public interface ISubscriptionList : IResponceList
    {
        IList<ISubscription> Items { get; }
    }

    public interface ISubscription
    {
        string ChannelId { get; }
        string Title { get; }
        string Description { get; }
        IThumbnailDetails Thumbnails { get; }
    }

    class MSubscription : ISubscription
    {
        public MSubscription(Subscription subscription)
        {
            ChannelId = subscription.Snippet.ResourceId.ChannelId;
            Title = subscription.Snippet.Title;
            Description = subscription.Snippet.Description;
            Thumbnails = new MThumbnailDetails(subscription.Snippet.Thumbnails);
        }

        public string ChannelId
        {
            get; private set;          
        }

        public string Description
        {
            get; private set;
        }

        public IThumbnailDetails Thumbnails
        {
            get; private set;
        }

        public string Title
        {
            get; private set;
        }
    }

    class MSubscriptionList : ISubscriptionList
    {
        public MSubscriptionList(SubscriptionListResponse response)
        {
            ETag = response.ETag;
            EventId = response.EventId;
            Items = response.Items.Select(s => new MSubscription(s)).ToList<ISubscription>();
            NextPageToken = response.NextPageToken;
            PageInfo = new MPageInfo(response.PageInfo);
            PrevPageToken = response.PrevPageToken;
            TokenPagination = new MTokenPagination(response.TokenPagination);
            VisitorId = response.VisitorId;
        }

        public string ETag
        {
            get;
            private set;
        }

        public string EventId
        {
            get;
            private set;
        }

        public IList<ISubscription> Items
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
    }
}
