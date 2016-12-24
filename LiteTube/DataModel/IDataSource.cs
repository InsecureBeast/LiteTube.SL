using System;
using LiteTube.DataClasses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.Common;
using LiteTube.Multimedia;

namespace LiteTube.DataModel
{
    public interface IDataSource
    {
        Task Login();
        bool IsAuthorized { get; }
        Task LoginSilently(string username);
        void Logout();
        void Update(string region, string quality);
        Task<IVideoList> GetActivity(string pageToken);
        Task<IVideoList> GetRecommended(string pageToken);
        Task<IVideoList> GetMostPopular(string pageToken);
        Task<IVideoList> GetCategoryVideoList(string categoryId, string pageToken);
        Task<IEnumerable<IVideoCategory>> GetCategories();
        Task<IEnumerable<IGuideCategory>> GetGuideCategories();
        Task<IChannel> GetChannel(string channelId);
        Task<IChannel> GetChannelByUsername(string username);
        Task<IVideoList> GetRelatedVideoList(string videoId, string pageToken);
        Task<IVideoList> GetChannelVideoList(string channelId, string pageToken);
        Task<IChannelList> GetChannels(string categoryId, string nextPageToken);
        Task<IResponceList> Search(string searchString, string nextPageToken, SearchType serachType, SearchFilter searchFilter);
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
#region playlists
        string FavoritesPlaylistId { get; }
        string WatchLaterPlaylistId { get; }
        string UploadedPlaylistId { get; }

        Task AddItemToPlaylist(string videoId, string playlistId);
        Task RemoveItemFromPlaylist(string playlistItemId);
        Task<IPlaylistItemList> GetPlaylistItems(string playlistId, string nextPageToken);
        //TODO remove
        Task<IResponceList> GetLiked(string nextPageToken);
        //----

        Task<IPlaylistList> GetPlaylists();
        Task<IPlaylistList> GetChannelPlaylistList(string channelId, string nextPageToken);
        Task<IVideoList> GetVideoPlaylist(string playListId, string nextPageToken);
        Task<IPlaylistList> GetMyPlaylistList(string nextPageToken);
        #endregion
        Task<IVideoItem> GetVideoItem(string videoId);
        IProfile GetProfile();
        Task<IComment> AddComment(string channelId, string videoId, string text);
        Task<IEnumerable<string>> GetAutoCompleteSearchItems(string query);

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
        private string _region;
        private readonly int _maxPageResult;
        private YouTubeQuality _quality;
        private readonly Notifier<UpdateContextEventArgs> _contextNotifier = new Notifier<UpdateContextEventArgs>();
        private readonly Notifier<UpdateSettingsEventArgs> _settingsNotifier = new Notifier<UpdateSettingsEventArgs>();

        public DataSource(IRemoteDataSource remoteDataSource, string region, int maxPageResult, string quality)
        {
            _remoteDataSource = remoteDataSource;
            _categories = new List<IVideoCategory>();
            _guideCategories = new List<IGuideCategory>();
            _channels = new List<IChannel>();
            _region = region.ToUpper();
            _maxPageResult = maxPageResult;
            _qualityHelper = new VideoQuality();
            _quality = _qualityHelper.GetQualityEnum(quality);
        }

        public bool IsAuthorized
        {
            get { return _remoteDataSource.IsAuthorized; }
        }

        public async Task Login()
        {
            try
            {
                await _remoteDataSource.Login();
                _contextNotifier.Notify(new UpdateContextEventArgs());
            }
            catch (Exception)
            {
                _contextNotifier.Notify(new UpdateContextEventArgs());
            }
        }
     
        public void Logout()
        {
            _remoteDataSource.Logout();
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
            return await _remoteDataSource.GetChannel(channelId);
        }

        public async Task<IChannel> GetChannelByUsername(string username)
        {
            return await _remoteDataSource.GetChannelByUsername(username);
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

        public async Task<IResponceList> Search(string searchString, string nextPageToken, SearchType serachType, SearchFilter searchFilter)
        {
            return await _remoteDataSource.Search(searchString, _maxPageResult, _region, nextPageToken, serachType, searchFilter);
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
            return await _remoteDataSource.GetVideoUriAsync(videoId, quality);
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

        public async Task<YouTubeUri> GetVideoUriAsync(string videoId)
        {
            return await _remoteDataSource.GetVideoUriAsync(videoId, _quality);
        }

        public async Task<IVideoList> GetRecommended(string pageToken)
        {
            return await _remoteDataSource.GetRecommended(pageToken);
        }

        public async Task AddItemToPlaylist(string videoId, string playlistId)
        {
            await _remoteDataSource.AddItemToPlaylist(videoId, playlistId);
        }

        public async Task RemoveItemFromPlaylist(string playlistItemId)
        {
            await _remoteDataSource.RemoveItemFromPlaylist(playlistItemId);
        }

        public async Task<IPlaylistItemList> GetPlaylistItems(string playlistId, string nextPageToken)
        {
            return await _remoteDataSource.GetPlaylistItems(playlistId, _maxPageResult, nextPageToken);
        }

        public async Task<IResponceList> GetLiked(string nextPageToken)
        {
            return await _remoteDataSource.GetLiked(_maxPageResult, nextPageToken);
        }

        public Task<IPlaylistList> GetPlaylists()
        {
            throw new NotImplementedException();
        }

        public async Task<IVideoItem> GetVideoItem(string videoId)
        {
            return await _remoteDataSource.GetVideoItem(videoId);
        }

        public IProfile GetProfile()
        {
            return _remoteDataSource.GetProfile();
        }

        public async Task<IComment> AddComment(string channelId, string videoId, string text)
        {
            return await _remoteDataSource.AddComment(channelId, videoId, text);
        }

        public async Task<IEnumerable<string>> GetAutoCompleteSearchItems(string query)
        {
            return await _remoteDataSource.GetAutoCompleteSearchItems(query);
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

        public async Task<IPlaylistList> GetChannelPlaylistList(string channelId, string nextPageToken)
        {
            return await _remoteDataSource.GetChannelPlaylistList(channelId, _maxPageResult, nextPageToken);
        }

        public async Task<IVideoList> GetVideoPlaylist(string playListId, string nextPageToken)
        {
            return await _remoteDataSource.GetVideoPlaylist(playListId, _maxPageResult, nextPageToken);
        }

        public async Task<IPlaylistList> GetMyPlaylistList(string nextPageToken)
        {
            return await _remoteDataSource.GetMyPlaylistList(_maxPageResult, nextPageToken);
        }
    }
}