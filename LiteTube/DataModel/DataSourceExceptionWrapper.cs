using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteTube.DataClasses;
using System.Diagnostics;
using Google;
using LiteTube.Common;
using LiteTube.Multimedia;
using LiteTube.Common.Exceptions;

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
                throw;
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
                throw;
            }
        }

        public async Task LoginSilently(string username)
        {
            try
            {
                Debug.WriteLine("LoginSilently method called");
                if (_remoteDataSource != null) 
                    await _remoteDataSource.LoginSilently(username);
                Debug.WriteLine("LoginSilently method call ended");
            }
            catch (Exception e)
            {
                //где-то внутри google api происходит исключение. Пока так
                Debug.WriteLine("LoginSilently method called with exception " + e.Message);
#if DEBUG
                throw;
#endif
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

        public string FavoritesPlaylistId
        {
            get { return _remoteDataSource.FavoritesPlaylistId; }
        }

        public string WatchLaterPlaylistId
        {
            get { return _remoteDataSource.WatchLaterPlaylistId; }
        }

        public string UploadedPlaylistId
        {
            get { return _remoteDataSource.UploadedPlaylistId; }
        }

        public async Task<IEnumerable<IVideoCategory>> GetCategories(string culture)
        {
            try
            {
                Debug.WriteLine("GetCategories method called");
                return await _remoteDataSource.GetCategories(culture);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetCategories method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IVideoList> GetActivity(string culture, int maxResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetActivity method called");
                return await _remoteDataSource.GetActivity(culture, maxResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetActivity method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IVideoList> GetRecommended(string pageToken)
        {
            try
            {
                Debug.WriteLine("GetRecommended method called");
                return await _remoteDataSource.GetRecommended(pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetRecommended method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IVideoList> GetMostPopular(string culture, int maxResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetMostPopular method called");
                return await _remoteDataSource.GetMostPopular(culture, maxResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetMostPopular method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IChannel> GetChannel(string channelId)
        {
            try
            {
                Debug.WriteLine("GetChannel method called");
                return await _remoteDataSource.GetChannel(channelId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetChannel method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IChannel> GetChannelByUsername(string username)
        {
            try
            {
                Debug.WriteLine("GetChannelByUsername method called");
                return await _remoteDataSource.GetChannelByUsername(username);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetChannelByUsername method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IVideoList> GetRelatedVideos(string videoId, int maxResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetRelatedVideos method called");
                return await _remoteDataSource.GetRelatedVideos(videoId, maxResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetRelatedVideos method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IVideoList> GetCategoryVideoList(string categoryId, string culture, int maxResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetCategoryVideoList method called");
                return await _remoteDataSource.GetCategoryVideoList(categoryId, culture, maxResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetCategoryVideoList method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IVideoList> GetChannelVideoList(string channelId, string culture, int maxPageResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetChannelVideoList method called");
                return await _remoteDataSource.GetChannelVideoList(channelId, culture, maxPageResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetChannelVideoList method called with exception " + e.Message);
                return MVideoList.Empty;
            }
        }

        public async Task<IEnumerable<IGuideCategory>> GetGuideCategories(string culture)
        {
            try
            {
                Debug.WriteLine("GetGuideCategories method called");
                return await _remoteDataSource.GetGuideCategories(culture);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetGuideCategories method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IChannelList> GetChannels(string categoryId, string culture, int maxPageResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetChannels method called");
                return await _remoteDataSource.GetChannels(categoryId, culture, maxPageResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetChannels method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IResponceList> Search(string searchString, int maxResult, string nextPageToken, string culture, SearchType serachType, SearchFilter searchFilter)
        {
            try
            {
                Debug.WriteLine("Search method called");
                return await _remoteDataSource.Search(searchString, maxResult, culture, nextPageToken, serachType, searchFilter);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Search method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<ICommentList> GetComments(string videoId, int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetComments method called");
                return await _remoteDataSource.GetComments(videoId, maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetComments method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<ISubscriptionList> GetSubscribtions(int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetSubscribtions method called");
                return await _remoteDataSource.GetSubscribtions(maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetSubscribtions method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IVideoList> GetHistory(int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetHistory method called");
                return await _remoteDataSource.GetHistory(maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetHistory method called with exception " + e.Message);
                throw;
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
                throw;
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
                throw;
            }
        }

        public Task Subscribe(string channelId)
        {
            try
            {
                Debug.WriteLine("Subscribe method called");
                return _remoteDataSource.Subscribe(channelId);
            }
            catch (GoogleApiException e)
            {
                throw new LiteTubeException(e);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Subscribe method called with exception " + e.Message);
                throw;
            }
        }

        public Task Unsubscribe(string subscriptionId)
        {
            try
            {
                Debug.WriteLine("Unsubscribe method called");
                return _remoteDataSource.Unsubscribe(subscriptionId);
            }
            catch (GoogleApiException e)
            {
                throw new LiteTubeException(e);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unsubscribe method called with exception " + e.Message);
                throw;
            }
        }

        public Task SetRating(string videoId, RatingEnum rating)
        {
            try
            {
                Debug.WriteLine("SetRating method called");
                return _remoteDataSource.SetRating(videoId, rating);
            }
            catch (GoogleApiException e)
            {
                throw new LiteTubeException(e);
            }
            catch (Exception e)
            {
                Debug.WriteLine("SetRating method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<RatingEnum> GetRating(string videoId)
        {
            try
            {
                Debug.WriteLine("GetRating method called");
                return await _remoteDataSource.GetRating(videoId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetRating method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<YouTubeUri> GetVideoUriAsync(string videoId, YouTubeQuality quality)
        {
            try
            {
                Debug.WriteLine("GetVideoUriAsync method called");
                return await _remoteDataSource.GetVideoUriAsync(videoId, quality);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetVideoUriAsync method called with exception " + e.Message);
                throw;
            }
        }

        public Task AddToPlaylist(string videoId, string playlistId)
        {
            try
            {
                Debug.WriteLine("AddToPlaylist method called");
                return _remoteDataSource.AddToPlaylist(videoId, playlistId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("AddToPlaylist method called with exception " + e.Message);
                throw;
            }
        }

        public Task RemovePlaylistItem(string playlistItemId)
        {
            try
            {
                Debug.WriteLine("RemovePlaylistItem method called");
                return _remoteDataSource.RemovePlaylistItem(playlistItemId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("RemovePlaylistItem method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IResponceList> GetFavorites(int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetFavorites method called");
                return await _remoteDataSource.GetFavorites(maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetFavorites method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IResponceList> GetLiked(int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetLiked method called");
                return await _remoteDataSource.GetLiked(maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetLiked method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IVideoItem> GetVideoItem(string videoId)
        {
            try
            {
                Debug.WriteLine("GetVideoItem method called");
                return await _remoteDataSource.GetVideoItem(videoId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetVideoItem method called with exception " + e.Message);
                throw;
            }
        }

        public IProfile GetProfile()
        {
            try
            {
                Debug.WriteLine("GetProfile method called");
                return _remoteDataSource.GetProfile();
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetProfile method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IComment> AddComment(string channelId, string videoId, string text)
        {
            try
            {
                Debug.WriteLine("AddComment method called");
                return await _remoteDataSource.AddComment(channelId, videoId, text);
            }
            catch (Exception e)
            {
                Debug.WriteLine("AddComment method called with exception " + e.Message);
                throw new LiteTubeException(e);
            }
        }

        public async Task<IEnumerable<string>> GetAutoCompleteSearchItems(string query)
        {
            try
            {
                Debug.WriteLine("GetAutoCompleteSearchItems method called");
                return await _remoteDataSource.GetAutoCompleteSearchItems(query);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetAutoCompleteSearchItems method called with exception " + e.Message);
                return await new Task<IEnumerable<string>>(() => new List<string>());
            }
        }

        public async Task<IPlaylistList> GetChannelPlaylistList(string channelId, int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetChannelPlaylistList method called");
                return await _remoteDataSource.GetChannelPlaylistList(channelId, maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetChannelPlaylistList method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IVideoList> GetVideoPlaylist(string playListId, int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetVideoPlaylist method called");
                return await _remoteDataSource.GetVideoPlaylist(playListId, maxResult, nextPageToken);
            }
            catch (GoogleApiException e)
            {
                if (e.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new PlaylistNotFoundException(e);
                throw e;
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetVideoPlaylist method called with exception " + e.Message);
                throw e;
            }
        }

        public async Task<IPlaylistList> GetMyPlaylistList(int maxResult, string nextPageToken)
        {
            try
            {
                Debug.WriteLine("GetMyPlaylistList method called");
                return await _remoteDataSource.GetMyPlaylistList(maxResult, nextPageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetMyPlaylistList method called with exception " + e.Message);
                throw;
            }
        }

        public async Task<IVideoList> GetWatchLater(string culture, int maxResult, string pageToken)
        {
            try
            {
                Debug.WriteLine("GetWatchLater method called");
                return await _remoteDataSource.GetWatchLater(culture, maxResult, pageToken);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetWatchLater method called with exception " + e.Message);
                throw;
            }
        }
    }
}
