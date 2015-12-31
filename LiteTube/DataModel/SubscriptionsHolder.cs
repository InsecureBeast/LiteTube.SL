using System.Diagnostics;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiteTube.DataModel
{
    class SubscriptionsHolder
    {
        private readonly IYouTubeService _youTubeService;
        private readonly Dictionary<string, string> _subscriptions;

        public SubscriptionsHolder(IYouTubeService youTubeService)
        {
            if (youTubeService == null)
                throw new ArgumentException("youTubeService");

            _youTubeService = youTubeService;
            _subscriptions = new Dictionary<string, string>();
        }

        public async Task Init()
        {
            var response = await LoadSubscriptions(string.Empty);
            while (response.NextPageToken != null)
            {
                response = await LoadSubscriptions(response.NextPageToken);
            }
        }

        public IEnumerable<string> GetSubscriptions()
        {
            return _subscriptions.Values;
        }

        public bool IsSubsribed(string channelId)
        {
            string subscriptionId;
            return _subscriptions.TryGetValue(channelId, out subscriptionId);
        }

        public string GetSubsriptionId(string channelId)
        {
            string subscriptionId;
            if (_subscriptions.TryGetValue(channelId, out subscriptionId))
                return subscriptionId;
            return null;
        }

        public void Clear()
        {
            _subscriptions.Clear();
        }

        public async Task Subscribe(string channelId)
        {
            if (string.IsNullOrEmpty(channelId))
                return;
            try
            {
                var resourcId = new ResourceId()
                {
                    ChannelId = channelId,
                    Kind = "youtube#channel"
                };

                var snippet = new SubscriptionSnippet()
                {
                    ResourceId = resourcId
                };

                var body = new Subscription()
                {
                    Snippet = snippet
                };

                var request = _youTubeService.GetAuthorizedService().Subscriptions.Insert(body, "snippet");
                request.Key = _youTubeService.ApiKey;
            
                var response = await request.ExecuteAsync();
                Add(channelId, response.Id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public async Task Unsubscribe(string subscriptionId)
        {
            if (string.IsNullOrEmpty(subscriptionId))
                return;

            try
            {
                var request = _youTubeService.GetAuthorizedService().Subscriptions.Delete(subscriptionId);
                request.Key = _youTubeService.ApiKey;
            
                var response = await request.ExecuteAsync();
                Remove(subscriptionId);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private async Task<SubscriptionListResponse> LoadSubscriptions(string nexPageToken)
        {
            var request = _youTubeService.GetAuthorizedService().Subscriptions.List("id,snippet");
            request.Key = _youTubeService.ApiKey;
            request.Mine = true;
            request.MaxResults = 45;
            request.Fields = "items(id,snippet),nextPageToken";
            request.PageToken = nexPageToken;
            request.OauthToken = _youTubeService.OAuthToken;

            var response = await request.ExecuteAsync();
            foreach (var item in response.Items)
            {
                _subscriptions[item.Snippet.ResourceId.ChannelId] = item.Id;
            }
            return response;
        }

        private void Remove(string subscriptionId)
        {
            var key = _subscriptions.FirstOrDefault(v => v.Value == subscriptionId).Key;
            if (key == null)
                return;

            _subscriptions.Remove(key);
        }

        private void Add(string channelId, string subscriptionId)
        {
            if (channelId == null || subscriptionId == null)
                return;

            _subscriptions[channelId] = subscriptionId;
        }
    }
}
