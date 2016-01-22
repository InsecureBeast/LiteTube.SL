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

        public void Login()
        {
            try
            {
                Debug.WriteLine("Login methode called");
                _remoteDataSource.Login();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Login methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<string> ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args, string username)
        {
            try
            {
                Debug.WriteLine("ContinueWebAuthentication methode called");
                return _remoteDataSource.ContinueWebAuthentication(args, username);
            }
            catch (Exception e)
            {
                Debug.WriteLine("ContinueWebAuthentication methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task Logout()
        {
            try
            {
                Debug.WriteLine("Logout methode called");
                return _remoteDataSource.Logout();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Logout methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task LoginSilently(string username)
        {
            try
            {
                Debug.WriteLine("Logout methode called");
                return _remoteDataSource.LoginSilently(username);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Logout methode called with exception " + e.Message );
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
                Debug.WriteLine("GetCategories methode called");
                return _remoteDataSource.GetCategories(culture);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetCategories methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetActivity(string culture, int maxResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetActivity methode called");
                return _remoteDataSource.GetActivity(culture, maxResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetActivity methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetRecommended(string pageToken)
        {
            try
            {
                Debug.WriteLine("GetRecommended methode called");
                return _remoteDataSource.GetRecommended(pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetRecommended methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetMostPopular(string culture, int maxResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetMostPopular methode called");
                return _remoteDataSource.GetMostPopular(culture, maxResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetMostPopular methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IChannel> GetChannel(string channelId)
        {
            try
            {
                Debug.WriteLine("GetChannel methode called");
                return _remoteDataSource.GetChannel(channelId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetChannel methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetRelatedVideos(string videoId, int maxResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetRelatedVideos methode called");
                return _remoteDataSource.GetRelatedVideos(videoId, maxResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetRelatedVideos methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetCategoryVideoList(string categoryId, string culture, int maxResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetCategoryVideoList methode called");
                return _remoteDataSource.GetCategoryVideoList(categoryId, culture, maxResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetCategoryVideoList methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetChannelVideoList(string channelId, string culture, int maxPageResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetChannelVideoList methode called");
                return _remoteDataSource.GetChannelVideoList(channelId, culture, maxPageResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetChannelVideoList methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IEnumerable<IGuideCategory>> GetGuideCategories(string culture)
        {
            try
            {
                Debug.WriteLine("GetGuideCategories methode called");
                return _remoteDataSource.GetGuideCategories(culture);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetGuideCategories methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IChannelList> GetChannels(string categoryId, string culture, int maxPageResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetChannels methode called");
                return _remoteDataSource.GetChannels(categoryId, culture, maxPageResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetChannels methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> Search(string searchString, int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("Search methode called");
                return _remoteDataSource.Search(searchString, maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Search methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<ICommentList> GetComments(string videoId, int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetComments methode called");
                return _remoteDataSource.GetComments(videoId, maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetComments methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<ISubscriptionList> GetSubscribtions(int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetSubscribtions methode called");
                return _remoteDataSource.GetSubscribtions(maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetSubscribtions methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoList> GetHistory(int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetHistory methode called");
                return _remoteDataSource.GetHistory(maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetHistory methode called with exception " + e.Message);
                throw e;
            }
        }

        public bool IsSubscribed(string channelId)
        {
            try
            {
                Debug.WriteLine("IsSubscribed methode called");
                return _remoteDataSource.IsSubscribed(channelId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("IsSubscribed methode called with exception " + e.Message);
                throw e;
            }
        }

        public string GetSubscriptionId(string channelId)
        {
            try
            {
                Debug.WriteLine("GetSubscriptionId methode called");
                return _remoteDataSource.GetSubscriptionId(channelId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetSubscriptionId methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task Subscribe(string channelId)
        {
            try
            {
                Debug.WriteLine("Subscribe methode called");
                return _remoteDataSource.Subscribe(channelId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Subscribe methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task Unsubscribe(string subscriptionId)
        {
            try
            {
                Debug.WriteLine("Unsubscribe methode called");
                return _remoteDataSource.Unsubscribe(subscriptionId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unsubscribe methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task SetRating(string videoId, RatingEnum rating)
        {
            try
            {
                Debug.WriteLine("SetRating methode called");
                return _remoteDataSource.SetRating(videoId, rating);
            }
            catch (Exception e)
            {
                Debug.WriteLine("SetRating methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<RatingEnum> GetRating(string videoId)
        {
            try
            {
                Debug.WriteLine("GetRating methode called");
                return _remoteDataSource.GetRating(videoId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetRating methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<YouTubeUri> GetVideoUriAsync(string videoId, YouTubeQuality quality)
        {
            try
            {
                Debug.WriteLine("GetVideoUriAsync methode called");
                return _remoteDataSource.GetVideoUriAsync(videoId, quality);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetVideoUriAsync methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task AddToFavorites(string videoId)
        {
            try
            {
                Debug.WriteLine("AddToFavorites methode called");
                return _remoteDataSource.AddToFavorites(videoId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("AddToFavorites methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task RemoveFromFavorites(string playlistItemId)
        {
            try
            {
                Debug.WriteLine("RemoveFromFavorites methode called");
                return _remoteDataSource.RemoveFromFavorites(playlistItemId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("RemoveFromFavorites methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IResponceList> GetFavorites(int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetFavorites methode called");
                return _remoteDataSource.GetFavorites(maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetFavorites methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IVideoItem> GetVideoItem(string videoId)
        {
            try
            {
                Debug.WriteLine("GetVideoItem methode called");
                return _remoteDataSource.GetVideoItem(videoId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetVideoItem methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IProfile> GetProfile()
        {
            try
            {
                Debug.WriteLine("GetProfile methode called");
                return _remoteDataSource.GetProfile();
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetProfile methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IComment> AddComment(string channelId, string videoId, string text)
        {
            try
            {
                Debug.WriteLine("AddComment methode called");
                return _remoteDataSource.AddComment(channelId, videoId, text);
            }
            catch (Exception e)
            {
                Debug.WriteLine("AddComment methode called with exception " + e.Message);
                throw e;
            }
        }

        public Task<IEnumerable<string>> GetAutoCompleteSearchItems(string query)
        {
            try
            {
                Debug.WriteLine("GetAutoCompleteSearchItems methode called");
                return _remoteDataSource.GetAutoCompleteSearchItems(query);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetAutoCompleteSearchItems methode called with exception " + e.Message);
                throw e;
            }
        }
    }
}
