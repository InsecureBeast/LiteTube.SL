using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteTube.DataClasses;
using Windows.ApplicationModel.Activation;
using System.Diagnostics;
using MyToolkit.Multimedia;

namespace LiteTube.DataModel
{
    class DataSourceExceptionWrapper : IRemoteDataSource
    {
        private readonly IRemoteDataSource _remoteDataSource;

        public DataSourceExceptionWrapper(IRemoteDataSource remoteDataSource)
        {
            _remoteDataSource = remoteDataSource;
        }

        public Task Login()
        {
            try
            {
                Debug.WriteLine("Login method called");
                return _remoteDataSource.Login();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Login method called with exception " + e.Message);
                throw e;
            }
        }

        public void Logout()
        {
            try
            {
                Debug.WriteLine("Logout method called");
                _remoteDataSource.Logout();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Logout method called with exception " + e.Message);
                throw e;
            }
        }

        public async Task LoginSilently(string username)
        {
            try
            {
                Debug.WriteLine("LoginSilently method called");
                await _remoteDataSource.LoginSilently(username);
                Debug.WriteLine("LoginSilently method call ended");
            }
            catch (Exception e)
            {
                Debug.WriteLine("LoginSilently method called with exception " + e.Message);
                throw e;
            }
        }

        public bool IsAuthorized
        {
            get
            {
                var result = _remoteDataSource.IsAuthorized;
                Debug.WriteLine("IsAuthorized property called. Value = " + result);
                return result;
            }
        }
        public Task<IEnumerable<IVideoCategory>> GetCategories(string culture)
        {
            try
            {
                Debug.WriteLine("GetCategories method called");
                return _remoteDataSource.GetCategories(culture);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetCategories method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetActivity(string culture, int maxResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetActivity method called");
                return _remoteDataSource.GetActivity(culture, maxResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetActivity method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetRecommended(string pageToken)
        {
            try
            {
                Debug.WriteLine("GetRecommended method called");
                return _remoteDataSource.GetRecommended(pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetRecommended method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetMostPopular(string culture, int maxResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetMostPopular method called");
                return _remoteDataSource.GetMostPopular(culture, maxResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetMostPopular method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IChannel> GetChannel(string channelId)
        {
            try
            {
                Debug.WriteLine("GetChannel method called");
                return _remoteDataSource.GetChannel(channelId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetChannel method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetRelatedVideos(string videoId, int maxResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetRelatedVideos method called");
                return _remoteDataSource.GetRelatedVideos(videoId, maxResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetRelatedVideos method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetCategoryVideoList(string categoryId, string culture, int maxResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetCategoryVideoList method called");
                return _remoteDataSource.GetCategoryVideoList(categoryId, culture, maxResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetCategoryVideoList method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetChannelVideoList(string channelId, string culture, int maxPageResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetChannelVideoList method called");
                return _remoteDataSource.GetChannelVideoList(channelId, culture, maxPageResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetChannelVideoList method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IEnumerable<IGuideCategory>> GetGuideCategories(string culture)
        {
            try
            {
                Debug.WriteLine("GetGuideCategories method called");
                return _remoteDataSource.GetGuideCategories(culture);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetGuideCategories method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IChannelList> GetChannels(string categoryId, string culture, int maxPageResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetChannels method called");
                return _remoteDataSource.GetChannels(categoryId, culture, maxPageResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetChannels method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> Search(string searchString, int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("Search method called");
                return _remoteDataSource.Search(searchString, maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Search method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<ICommentList> GetComments(string videoId, int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetComments method called");
                return _remoteDataSource.GetComments(videoId, maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetComments method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<ISubscriptionList> GetSubscribtions(int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetSubscribtions method called");
                return _remoteDataSource.GetSubscribtions(maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetSubscribtions method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetHistory(int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetHistory method called");
                return _remoteDataSource.GetHistory(maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetHistory method called with exception " + e.Message);
                throw e;
            }
        }

        public bool IsSubscribed(string channelId)
        {
            try
            {
                Debug.WriteLine("IsSubscribed method called");
                return _remoteDataSource.IsSubscribed(channelId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("IsSubscribed method called with exception " + e.Message);
                throw e;
            }
        }

        public string GetSubscriptionId(string channelId)
        {
            try
            {
                Debug.WriteLine("GetSubscriptionId method called");
                return _remoteDataSource.GetSubscriptionId(channelId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetSubscriptionId method called with exception " + e.Message);
                throw e;
            }
        }

        public Task Subscribe(string channelId)
        {
            try
            {
                Debug.WriteLine("Subscribe method called");
                return _remoteDataSource.Subscribe(channelId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Subscribe method called with exception " + e.Message);
                throw e;
            }
        }

        public Task Unsubscribe(string subscriptionId)
        {
            try
            {
                Debug.WriteLine("Unsubscribe method called");
                return _remoteDataSource.Unsubscribe(subscriptionId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unsubscribe method called with exception " + e.Message);
                throw e;
            }
        }

        public Task SetRating(string videoId, RatingEnum rating)
        {
            try
            {
                Debug.WriteLine("SetRating method called");
                return _remoteDataSource.SetRating(videoId, rating);
            }
            catch (Exception e)
            {
                Debug.WriteLine("SetRating method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<RatingEnum> GetRating(string videoId)
        {
            try
            {
                Debug.WriteLine("GetRating method called");
                return _remoteDataSource.GetRating(videoId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetRating method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<YouTubeUri> GetVideoUriAsync(string videoId, YouTubeQuality quality)
        {
            try
            {
                Debug.WriteLine("GetVideoUriAsync method called");
                return _remoteDataSource.GetVideoUriAsync(videoId, quality);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetVideoUriAsync method called with exception " + e.Message);
                throw e;
            }
        }

        public Task AddToFavorites(string videoId)
        {
            try
            {
                Debug.WriteLine("AddToFavorites method called");
                return _remoteDataSource.AddToFavorites(videoId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("AddToFavorites method called with exception " + e.Message);
                throw e;
            }
        }

        public Task RemoveFromFavorites(string playlistItemId)
        {
            try
            {
                Debug.WriteLine("RemoveFromFavorites method called");
                return _remoteDataSource.RemoveFromFavorites(playlistItemId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("RemoveFromFavorites method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IResponceList> GetFavorites(int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetFavorites method called");
                return _remoteDataSource.GetFavorites(maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetFavorites method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoItem> GetVideoItem(string videoId)
        {
            try
            {
                Debug.WriteLine("GetVideoItem method called");
                return _remoteDataSource.GetVideoItem(videoId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetVideoItem method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IProfile> GetProfile()
        {
            try
            {
                Debug.WriteLine("GetProfile method called");
                return _remoteDataSource.GetProfile();
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetProfile method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IComment> AddComment(string channelId, string videoId, string text)
        {
            try
            {
                Debug.WriteLine("AddComment method called");
                return _remoteDataSource.AddComment(channelId, videoId, text);
            }
            catch (Exception e)
            {
                Debug.WriteLine("AddComment method called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IEnumerable<string>> GetAutoCompleteSearchItems(string query)
        {
            try
            {
                Debug.WriteLine("GetAutoCompleteSearchItems method called");
                return _remoteDataSource.GetAutoCompleteSearchItems(query);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetAutoCompleteSearchItems method called with exception " + e.Message);
                throw e;
            }
        }
    }
}
