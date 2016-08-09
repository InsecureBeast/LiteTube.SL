using System.Collections.Generic;
using System.Linq;
using Google.Apis.YouTube.v3.Data;

namespace LiteTube.Core.DataClasses
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
            var snippet = subscription.Snippet;
            if (snippet == null)
                return;

            if (snippet.ResourceId != null)
                ChannelId = snippet.ResourceId.ChannelId;

            Title = snippet.Title;
            Description = snippet.Description;
            Thumbnails = new MThumbnailDetails(snippet.Thumbnails);
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
            if (response == null)
                return;

            Items = response.Items.Select(s => new MSubscription(s)).ToList<ISubscription>();
            NextPageToken = response.NextPageToken;
            PageInfo = new MPageInfo(response.PageInfo);
            PrevPageToken = response.PrevPageToken;
            VisitorId = response.VisitorId;
        }

        public static ISubscriptionList EmptyList
        {
            get { return new MSubscriptionList(null);}
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

        public string VisitorId
        {
            get;
            private set;
        }
    }
}
