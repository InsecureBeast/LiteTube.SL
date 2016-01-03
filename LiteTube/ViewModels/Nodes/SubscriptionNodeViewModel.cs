using LiteTube.DataClasses;

namespace LiteTube.ViewModels.Nodes
{
    public class SubscriptionNodeViewModel : NodeViewModelBase
    {
        private readonly ISubscription _subscription;
        private readonly string _id;
        private readonly string _videoId;

        public SubscriptionNodeViewModel(ISubscription subscription)
        {
            _subscription = subscription;
            Title = subscription.Title;
            _id = subscription.ChannelId;
            Image = subscription.Thumbnails.GetThumbnailUrl();
        }

        public string Title
        {
            get;
            private set;
        }

        public string Image
        {
            get;
            private set;
        }

        public ISubscription Subscription
        {
            get { return _subscription; }
        }

        public override string Id
        {
            get { return _id; }
        }

        public override string VideoId
        {
            get { return _videoId; }
        }
    }
}
