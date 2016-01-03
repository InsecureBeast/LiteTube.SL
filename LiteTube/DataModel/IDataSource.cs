using LiteTube.DataClasses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.Common;
using System;
using Windows.ApplicationModel.Activation;
using MyToolkit.Multimedia;

namespace LiteTube.DataModel
{
    public interface IDataSource
    {
        void Login();
        Task<string> ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args, string username);
        bool IsAuthorized { get; }
        Task LoginSilently(string username);
        Task Logout();
        void Update(string region, string quality);
        Task<IVideoList> GetActivity(string pageToken);
        Task<IVideoList> GetRecommended(string pageToken);
        Task<IVideoList> GetMostPopular(string pageToken);
        Task<IVideoList> GetCategoryVideoList(string categoryId, string pageToken);
        Task<IEnumerable<IVideoCategory>> GetCategories();
        Task<IEnumerable<IGuideCategory>> GetGuideCategories();
        Task<IChannel> GetChannel(string channelId);
        Task<IVideoList> GetRelatedVideoList(string videoId, string pageToken);
        Task<IVideoList> GetChannelVideoList(string channelId, string pageToken);
        Task<IChannelList> GetChannels(string categoryId, string nextPageToken);
        Task<IVideoList> Search(string searchString, string nextPageToken);
        Task<ICommentList> GetComments(string videoId, string nextPageToken);
        Task<ISubscriptionList> GetSubscribtions(string nextPageToken);
        Task<IVideoList> GetHistory(string nextPageToken);
        bool IsSubscribed(string channelId);
        string GetSubscriptionId(string channelId);
        Task Subscribe(string channelId);
        Task Unsubscribe(string subscriptionId);
        Task SetRating(string videoId, RatingEnum rating);
        Task<RatingEnum> GetRating(string videoId);
        Task<YouTubeUri> GetVideoUriAsync(string videoId);
        Task<YouTubeUri> GetVideoUriAsync(string videoId, YouTubeQuality quality);
        Task AddToFavorites(string videoId);
        Task RemoveFromFavorites(string playlistItemId);
        Task<IResponceList> GetFavorites(string nextPageToken);
        Task<IVideoItem> GetVideoItem(string videoId);
        Task<IProfile> GetProfile();

        void Subscribe(IListener<UpdateSettingsEventArgs> listener);
        void Subscribe(IListener<UpdateContextEventArgs> listener);
        void Unsubscribe(IListener<UpdateSettingsEventArgs> listener);
        void Unsubscribe(IListener<UpdateContextEventArgs> listener);
    }

    class DataSource : IDataSource
    {
        private readonly IRemoteDataSource _remoteDataSource;
        private readonly IList<IVideoCategory> _categories;
        private readonly VideoQuality _qualityHelper;
        private IList<IGuideCategory> _guideCategories;
        private readonly List<IChannel> _channels;
        private readonly IDeviceHistory _deviceHistory;
        private  EventHandler _settingsUpdated;
        private string _region;
        private readonly int _maxPageResult;
        private YouTubeQuality _quality;
        private EventHandler _contextUpdated;
        private readonly Notifier<UpdateContextEventArgs> _contextNotifier = new Notifier<UpdateContextEventArgs>();
        private readonly Notifier<UpdateSettingsEventArgs> _settingsNotifier = new Notifier<UpdateSettingsEventArgs>();

        public DataSource(IRemoteDataSource remoteDataSource, string region, int maxPageResult, IDeviceHistory deviceHistory, string quality)
        {
            _remoteDataSource = remoteDataSource;
            _categories = new List<IVideoCategory>();
            _guideCategories = new List<IGuideCategory>();
            _channels = new List<IChannel>();
            _region = region.ToUpper();
            _maxPageResult = maxPageResult;
            _deviceHistory = deviceHistory;
            _qualityHelper = new VideoQuality();
            _quality = _qualityHelper.GetQualityEnum(quality);
        }

        public bool IsAuthorized
        {
            get { return _remoteDataSource.IsAuthorized; }
        }

        public void Login()
        {
            _remoteDataSource.Login();
        }

        public async Task<string> ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args, string username)
        {
            var result = await _remoteDataSource.ContinueWebAuthentication(args, username);
            _contextNotifier.Notify(new UpdateContextEventArgs());
            return result;
        }
     
        public async Task Logout()
        {
            await _remoteDataSource.Logout();
            _contextNotifier.Notify(new UpdateContextEventArgs());
        }

        public async Task LoginSilently(string username)
        {
            await _remoteDataSource.LoginSilently(username);
            _contextNotifier.Notify(new UpdateContextEventArgs());
        }

        public void Update(string region, string quality)
        {
            _region = region;
            _quality = _qualityHelper.GetQualityEnum(quality);
            _categories.Clear();
            _guideCategories.Clear();
            _channels.Clear();

            _settingsNotifier.Notify(new UpdateSettingsEventArgs());
    }

        public async Task<IVideoList> GetActivity(string pageToken)
        {
            return await _remoteDataSource.GetActivity(_region, _maxPageResult, pageToken);
        }

        public async Task<IVideoList> GetMostPopular(string pageToken)
        {
            return await _remoteDataSource.GetMostPopular(_region, _maxPageResult, pageToken);
        }

        public async Task<IGuideCategory> GetRecommendedForYouCategory()
        {
            var recommended = _guideCategories.FirstOrDefault();
            if (recommended == null)
            {
                var list = await _remoteDataSource.GetGuideCategories(_region);
                _guideCategories = list.ToList();
                recommended = _guideCategories.FirstOrDefault();
            }
            return recommended;
        }

        //public async Task<IGuideCategory> GetBestOfCategory()
        //{
        //    await LoadCategories();
        //    if (bestOf == null)
        //        bestOf = categories.FirstOrDefault(c => c.Title.Equals(YouTubeConstants.BEST_OF, StringComparison.OrdinalIgnoreCase));
        //    return bestOf;
        //}
        
        public async Task<IEnumerable<IVideoCategory>> GetCategories()
        {
            if (_categories.Any())
                return _categories;
             
            var list = await _remoteDataSource.GetCategories(_region);
            foreach (var item in list)
            {
                _categories.Add(item);
            }

            return _categories;
        }

        public async Task<IChannel> GetChannel(string channelId)
        {
            var ch = _channels.FirstOrDefault(c => c.Id == channelId);
            if (ch != null)
                return ch;
            
            ch = await _remoteDataSource.GetChannel(channelId);
            if (ch != null)
                _channels.Add(ch);
            return ch;
        }

        public async Task<IVideoList> GetRelatedVideoList(string videoId, string pageToken)
        {
            return await _remoteDataSource.GetRelatedVideos(videoId, _maxPageResult, pageToken);
        }

        public async Task<IVideoList> GetCategoryVideoList(string categoryId, string pageToken)
        {
            return await _remoteDataSource.GetCategoryVideoList(categoryId, _region, _maxPageResult, pageToken);
        }

        public async Task<IVideoList> GetChannelVideoList(string channelId, string pageToken)
        {
            return await _remoteDataSource.GetChannelVideoList(channelId, _region, _maxPageResult, pageToken);
        }

        public async Task<IEnumerable<IGuideCategory>> GetGuideCategories()
        {
            if (_guideCategories.Any())
                return _guideCategories;

            var list = await _remoteDataSource.GetGuideCategories(_region);
            foreach (var item in list)
            {
                _guideCategories.Add(item);
            }

            return _guideCategories;
        }

        public Task<IChannelList> GetChannels(string categoryId, string nextPageToken)
        {
            return _remoteDataSource.GetChannels(categoryId, _region, _maxPageResult, nextPageToken);
        }

        public async Task<IVideoList> Search(string searchString, string nextPageToken)
        {
            return await _remoteDataSource.Search(searchString, _maxPageResult, nextPageToken);
        }

        public async Task<ICommentList> GetComments(string videoId, string nextPageToken)
        {
            //TODO: cache
            return await _remoteDataSource.GetComments(videoId, _maxPageResult, nextPageToken);
        }

        public async Task<ISubscriptionList> GetSubscribtions(string nextPageToken)
        {
            //TODO: cache
            return await _remoteDataSource.GetSubscribtions(_maxPageResult, nextPageToken);
        }

        public async Task<IVideoList> GetHistory(string nextPageToken)
        {
            //TODO: cache
            return await _remoteDataSource.GetHistory(_maxPageResult, nextPageToken);
        }

        public async Task Subscribe(string channelId)
        {
            await _remoteDataSource.Subscribe(channelId);
        }

        public bool IsSubscribed(string channelId)
        {
            return _remoteDataSource.IsSubscribed(channelId);
        }

        public async Task Unsubscribe(string subscriptionId)
        {
            await _remoteDataSource.Unsubscribe(subscriptionId);
        }

        public string GetSubscriptionId(string channelId)
        {
            return _remoteDataSource.GetSubscriptionId(channelId);
        }

        public async Task SetRating(string videoId, RatingEnum rating)
        {
            await _remoteDataSource.SetRating(videoId, rating);
        }

        public async Task<RatingEnum> GetRating(string videoId)
        {
            return await _remoteDataSource.GetRating(videoId);
        }

        public async Task<YouTubeUri> GetVideoUriAsync(string videoId, YouTubeQuality quality)
        {
            _deviceHistory.Add(videoId);
            return await _remoteDataSource.GetVideoUriAsync(videoId, quality);
        }

        public async Task<YouTubeUri> GetVideoUriAsync(string videoId)
        {
            _deviceHistory.Add(videoId);
            return await _remoteDataSource.GetVideoUriAsync(videoId, _quality);
        }

        public async Task<IVideoList> GetRecommended(string pageToken)
        {
            return await _remoteDataSource.GetRecommended(pageToken);
        }

        public async Task AddToFavorites(string videoId)
        {
            await _remoteDataSource.AddToFavorites(videoId);
        }

        public async Task RemoveFromFavorites(string playlistItemId)
        {
            await _remoteDataSource.RemoveFromFavorites(playlistItemId);
        }

        public async Task<IResponceList> GetFavorites(string nextPageToken)
        {
            return await _remoteDataSource.GetFavorites(_maxPageResult, nextPageToken);
        }

        public async Task<IVideoItem> GetVideoItem(string videoId)
        {
            return await _remoteDataSource.GetVideoItem(videoId);
        }

        public async Task<IProfile> GetProfile()
        {
            return await _remoteDataSource.GetProfile();
        }

        public void Subscribe(IListener<UpdateSettingsEventArgs> listener)
        {
            _settingsNotifier.Subscribe(listener);
        }

        public void Subscribe(IListener<UpdateContextEventArgs> listener)
        {
            _contextNotifier.Subscribe(listener);
        }

        public void Unsubscribe(IListener<UpdateSettingsEventArgs> listener)
        {
            _settingsNotifier.Unsubscribe(listener);
        }

        public void Unsubscribe(IListener<UpdateContextEventArgs> listener)
        {
            _contextNotifier.Unsubscribe(listener);
        }
    }
}
