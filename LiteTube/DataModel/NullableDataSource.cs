using System.Collections.Generic;
using System.Threading.Tasks;
using LiteTube.Common;
using LiteTube.DataClasses;
using MyToolkit.Multimedia;

namespace LiteTube.DataModel
{
    class NullableDataSource : IDataSource
    {
        public Task Login()
        {
            return Task.Run(() => { });
        }

        public bool IsAuthorized
        {
            get { return false; }
        }
        public Task LoginSilently(string username)
        {
            return Task.Run(() => { });
        }

        public void Logout()
        {
        }

        public void Update(string region, string quality)
        {
        }

        public Task<IVideoList> GetActivity(string pageToken)
        {
            return new Task<IVideoList>(() => MVideoList.Empty);
        }

        public Task<IVideoList> GetRecommended(string pageToken)
        {
            return new Task<IVideoList>(() => MVideoList.Empty);
        }

        public Task<IVideoList> GetMostPopular(string pageToken)
        {
            return new Task<IVideoList>(() => MVideoList.Empty);
        }

        public Task<IVideoList> GetCategoryVideoList(string categoryId, string pageToken)
        {
            return new Task<IVideoList>(() => MVideoList.Empty);
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
            return new Task<IVideoList>(() => MVideoList.Empty);
        }

        public Task<IVideoList> GetChannelVideoList(string channelId, string pageToken)
        {
            return new Task<IVideoList>(() => MVideoList.Empty);
        }

        public Task<IChannelList> GetChannels(string categoryId, string nextPageToken)
        {
            return new Task<IChannelList>(() => MChannelList.EmptyList);
        }

        public Task<IVideoList> Search(string searchString, string nextPageToken)
        {
            return new Task<IVideoList>(() => MVideoList.Empty);
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
            return new Task<IVideoList>(() => MVideoList.Empty);
        }

        public bool IsSubscribed(string channelId)
        {
            return false;
        }

        public string GetSubscriptionId(string channelId)
        {
            return string.Empty;
        }

        public Task Subscribe(string channelId)
        {
            return Task.Run(() => { });
        }

        public Task Unsubscribe(string subscriptionId)
        {
            return Task.Run(() => { });
        }

        public Task SetRating(string videoId, RatingEnum rating)
        {
            return Task.Run(() => { });
        }

        public Task<RatingEnum> GetRating(string videoId)
        {
            return Task.Run(() => { return RatingEnum.None; });
        }

        public Task<YouTubeUri> GetVideoUriAsync(string videoId)
        {
            return Task.Run(() => { return new YouTubeUri(); });
        }

        public Task<YouTubeUri> GetVideoUriAsync(string videoId, YouTubeQuality quality)
        {
            return Task.Run(() => { return new YouTubeUri(); });
        }

        public Task AddToFavorites(string videoId)
        {
            return Task.Run(() => { });
        }

        public Task RemoveFromFavorites(string playlistItemId)
        {
            return Task.Run(() => { });
        }

        public Task<IResponceList> GetFavorites(string nextPageToken)
        {
            return new Task<IResponceList>(() => MVideoList.Empty);
        }

        public Task<IVideoItem> GetVideoItem(string videoId)
        {
            return new Task<IVideoItem>(() => MVideoItem.Empty);
        }

        public IProfile GetProfile()
        {
            return new MProfile(string.Empty, string.Empty, string.Empty);
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
