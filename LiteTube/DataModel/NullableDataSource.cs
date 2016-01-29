using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using LiteTube.Common;
using LiteTube.DataClasses;
using MyToolkit.Multimedia;

namespace LiteTube.DataModel
{
    class NullableDataSource : IDataSource
    {
        public void Login()
        {
        }

        public Task<string> ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args, string username)
        {
            return null;
        }

        public bool IsAuthorized
        {
            get { return !string.IsNullOrEmpty(SettingsHelper.GetRefreshToken()); }
        }
        public Task LoginSilently(string username)
        {
            return null;
        }

        public Task Logout()
        {
            return null;
        }

        public void Update(string region, string quality)
        {
        }

        public Task<IVideoList> GetActivity(string pageToken)
        {
            return new Task<IVideoList>(() => MVideoList.EmptyVideoList);
        }

        public Task<IVideoList> GetRecommended(string pageToken)
        {
            return new Task<IVideoList>(() => MVideoList.EmptyVideoList);
        }

        public Task<IVideoList> GetMostPopular(string pageToken)
        {
            return new Task<IVideoList>(() => MVideoList.EmptyVideoList);
        }

        public Task<IVideoList> GetCategoryVideoList(string categoryId, string pageToken)
        {
            return new Task<IVideoList>(() => MVideoList.EmptyVideoList);
        }

        public Task<IEnumerable<IVideoCategory>> GetCategories()
        {
            return new Task<IEnumerable<IVideoCategory>>(() => new List<IVideoCategory>());
        }

        public Task<IEnumerable<IGuideCategory>> GetGuideCategories()
        {
            return new Task<IEnumerable<IGuideCategory>>(() => new List<IGuideCategory>());
        }

        public Task<IChannel> GetChannel(string channelId)
        {
            return new Task<IChannel>(() => MChannel.Empty);
        }

        public Task<IVideoList> GetRelatedVideoList(string videoId, string pageToken)
        {
            return new Task<IVideoList>(() => MVideoList.EmptyVideoList);
        }

        public Task<IVideoList> GetChannelVideoList(string channelId, string pageToken)
        {
            return new Task<IVideoList>(() => MVideoList.EmptyVideoList);
        }

        public Task<IChannelList> GetChannels(string categoryId, string nextPageToken)
        {
            return new Task<IChannelList>(() => MChannelList.EmptyList);
        }

        public Task<IVideoList> Search(string searchString, string nextPageToken)
        {
            return new Task<IVideoList>(() => MVideoList.EmptyVideoList);
        }

        public Task<ICommentList> GetComments(string videoId, string nextPageToken)
        {
            return new Task<ICommentList>(() => MCommentList.EmptyList);
        }

        public Task<ISubscriptionList> GetSubscribtions(string nextPageToken)
        {
            return new Task<ISubscriptionList>(() => MSubscriptionList.EmptyList);
        }

        public Task<IVideoList> GetHistory(string nextPageToken)
        {
            return new Task<IVideoList>(() => MVideoList.EmptyVideoList);
        }

        public bool IsSubscribed(string channelId)
        {
            return false;
        }

        public string GetSubscriptionId(string channelId)
        {
            return null;
        }

        public Task Subscribe(string channelId)
        {
            return null;
        }

        public Task Unsubscribe(string subscriptionId)
        {
            return null;
        }

        public Task SetRating(string videoId, RatingEnum rating)
        {
            return null;
        }

        public Task<RatingEnum> GetRating(string videoId)
        {
            return null;
        }

        public Task<YouTubeUri> GetVideoUriAsync(string videoId)
        {
            return null;
        }

        public Task<YouTubeUri> GetVideoUriAsync(string videoId, YouTubeQuality quality)
        {
            return null;
        }

        public Task AddToFavorites(string videoId)
        {
            return null;
        }

        public Task RemoveFromFavorites(string playlistItemId)
        {
            return null;
        }

        public Task<IResponceList> GetFavorites(string nextPageToken)
        {
            return new Task<IResponceList>(() => MVideoList.EmptyVideoList);
        }

        public Task<IVideoItem> GetVideoItem(string videoId)
        {
            return new Task<IVideoItem>(() => MVideoItem.Empty);
        }

        public Task<IProfile> GetProfile()
        {
            return new Task<IProfile>(() => new MProfile(string.Empty, string.Empty, string.Empty));
        }

        public Task<IComment> AddComment(string channelId, string videoId, string text)
        {
            return new Task<IComment>(() => MComment.Empty);
        }

        public Task<IEnumerable<string>> GetAutoCompleteSearchItems(string query)
        {
            return new Task<IEnumerable<string>>(() => new List<string>());
        }

        public void Subscribe(IListener<UpdateSettingsEventArgs> listener)
        {
        }

        public void Subscribe(IListener<UpdateContextEventArgs> listener)
        {
        }

        public void Unsubscribe(IListener<UpdateSettingsEventArgs> listener)
        {
        }

        public void Unsubscribe(IListener<UpdateContextEventArgs> listener)
        {

        }
    }
}
