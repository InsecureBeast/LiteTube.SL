﻿using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using LiteTube.DataClasses;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteTube.Common;
using MyToolkit.Multimedia;
using System.Net;
using Google;
using Windows.ApplicationModel.Activation;

namespace LiteTube.DataModel
{
    interface IRemoteDataSource
    {
        void Login();
        Task<string> ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args, string username);
        Task Logout();
        Task LoginSilently(string username);
        bool IsAuthorized { get; }
        Task<IEnumerable<IVideoCategory>> GetCategories(string culture);
        Task<IVideoList> GetActivity(string culture, int maxResult, string pageToken);
        Task<IVideoList> GetRecommended(string pageToken);
        Task<IVideoList> GetMostPopular(string culture, int maxResult, string pageToken);
        Task<IChannel> GetChannel(string channelId);
        Task<IVideoList> GetRelatedVideos(string videoId, int maxResult, string pageToken);
        Task<IVideoList> GetCategoryVideoList(string categoryId, string culture, int maxResult, string pageToken);
        Task<IVideoList> GetChannelVideoList(string channelId, string culture, int maxPageResult, string pageToken);
        Task<IEnumerable<IGuideCategory>> GetGuideCategories(string culture);
        Task<IChannelList> GetChannels(string categoryId, string culture, int maxPageResult, string nextPageToken);
        Task<IVideoList> Search(string searchString, int maxResult, string nextPageToken);
        Task<ICommentList> GetComments(string videoId, int maxResult, string nextPageToken);
        Task<ISubscriptionList> GetSubscribtions(int maxResult, string nextPageToken);
        Task<IVideoList> GetHistory(int maxResult, string nextPageToken);
        bool IsSubscribed(string channelId);
        string GetSubscriptionId(string channelId);
        Task Subscribe(string channelId);
        Task Unsubscribe(string subscriptionId);
        Task SetRating(string videoId, RatingEnum rating);
        Task<RatingEnum> GetRating(string videoId);
        Task<YouTubeUri> GetVideoUriAsync(string videoId, YouTubeQuality quality);
        Task AddToFavorites(string videoId);
        Task RemoveFromFavorites(string playlistItemId);
        Task<IResponceList> GetFavorites(int maxResult, string nextPageToken);
        Task<IVideoItem> GetVideoItem(string videoId);
        Task<IProfile> GetProfile();
        Task<IComment> AddComment(string channelId, string videoId, string text);
    }
    
    class RemoteDataSource : IRemoteDataSource
    {
        private readonly IYouTubeService _youTubeServiceControl;
        private YouTubeService _youTubeService;
        private string _historyPlayListId;
        private string _uploadPlayList;
        private string _watchLaterPlayList;
        private string _likesPlayList;
        private string _favorites;
        private readonly PlaylistCahce _playlistCache;
        private readonly SubscriptionsHolder _subscriptionsHolder;
        private readonly YouTubeWeb _youTubeWeb;
        private const long SEARCH_PAGE_MAX_RESULT = 45;
        private MProfile _profileInfo;

        public RemoteDataSource(IYouTubeService youTubeServiceControl)
        {
            _youTubeServiceControl = youTubeServiceControl;
            _youTubeService = _youTubeServiceControl.GetService();
            _playlistCache = new PlaylistCahce();
            _subscriptionsHolder = new SubscriptionsHolder(youTubeServiceControl);
            _youTubeWeb = new YouTubeWeb();

            //TODO proxy!!! Обернуть вызовы через прокси
            //WebRequest.DefaultWebProxy = new WebProxy();
        }

        public void Login()
        {
            _youTubeServiceControl.Login();
        }

        public async Task<string> ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args, string username)
        {
            var userId = await _youTubeServiceControl.ContinueWebAuthentication(args, username);
            _youTubeService = _youTubeServiceControl.GetAuthorizedService();
            await _subscriptionsHolder.Init();
            return userId;
        }

        public async Task LoginSilently(string username)
        {
            await _youTubeServiceControl.RefreshToken(username);
            await _subscriptionsHolder.Init();
            _youTubeService = _youTubeServiceControl.GetAuthorizedService();
        }

        public async Task Logout()
        {
            await _youTubeServiceControl.Logout();
            _youTubeService = _youTubeServiceControl.GetService();
            _subscriptionsHolder.Clear();
        }

        public async Task<IVideoList> GetActivity(string culture, int maxResult, string pageToken)
        {
            if (!IsAuthorized)
                return MActivityList.Empty;

            var activityRequest = _youTubeService.Activities.List("contentDetails");
            activityRequest.RegionCode = I18nLanguages.GetRegionCode(culture);
            activityRequest.MaxResults = maxResult;
            activityRequest.PageToken = pageToken;
            activityRequest.Home = true;
            activityRequest.Key = _youTubeServiceControl.ApiKey;
            //activityRequest.OauthToken = _youTubeServiceControl.OAuthToken;

            var activityResponse = await activityRequest.ExecuteAsync();
            var videoIds = new StringBuilder();
            var filteredList = activityResponse.Items.Where(i => i.ContentDetails.Upload != null || i.ContentDetails.Recommendation != null);
            foreach (var item in filteredList)
            {
                videoIds.AppendLine(MVideoItem.GetVideoId(item.ContentDetails));
                videoIds.AppendLine(",");
            }
            var videos = await GetVideo(videoIds.ToString());
            return new MActivityList(activityResponse, videos);
        }


        public async Task<IVideoList> GetRecommended(string pageToken)
        {
            if (!IsAuthorized)
                return MVideoList.Empty;

            var res = await _youTubeWeb.GetRecommended(_youTubeServiceControl.OAuthToken, pageToken);
            if (res == null)
                return null;

            var videoIds = new StringBuilder();
            foreach (var id in res.Ids)
            {
                videoIds.AppendLine(id);
                videoIds.AppendLine(",");
            }

            var videos = await GetVideo(videoIds.ToString());
            videos.NextPageToken = res.NextPageToken;
            return new MVideoList(videos);
        }

        private async Task<VideoListResponse> GetVideo(string videoId)
        {
            var videoRequest = _youTubeService.Videos.List("snippet,contentDetails,statistics,player");
            videoRequest.Id = videoId;
            videoRequest.Key = _youTubeServiceControl.ApiKey;

            var videoResponse = await videoRequest.ExecuteAsync();
            return videoResponse;
        }

        public async Task<IVideoList> GetMostPopular(string culture, int maxResult, string pageToken)
        {
            var videoRequest = _youTubeService.Videos.List("snippet,contentDetails,statistics,player");
            videoRequest.RegionCode = I18nLanguages.GetRegionCode(culture);
            videoRequest.Chart = VideosResource.ListRequest.ChartEnum.MostPopular;
            videoRequest.Hl = I18nLanguages.GetHl(culture); ;
            videoRequest.Key = _youTubeServiceControl.ApiKey;
            videoRequest.MaxResults = maxResult;
            videoRequest.PageToken = pageToken;

            var videoResponse = await videoRequest.ExecuteAsync();
            return new MVideoList(videoResponse);
        }

        public async Task<IEnumerable<IVideoCategory>> GetCategories(string culture)
        {
            var categoriesListRequest = _youTubeService.VideoCategories.List("snippet");
            categoriesListRequest.Key = _youTubeServiceControl.ApiKey;
            categoriesListRequest.RegionCode = I18nLanguages.GetRegionCode(culture);
            categoriesListRequest.Hl = I18nLanguages.GetHl(culture);

            var categoriesListResponse = await categoriesListRequest.ExecuteAsync();
            return categoriesListResponse.Items.Select(category => new MVideoCategory(category)).Cast<IVideoCategory>().ToList();
        }

        public bool IsAuthorized
        {
            get { return _youTubeServiceControl.IsAuthorized; }
        }

        public async Task<IEnumerable<IPlaylist>> GetPlayLists(string channelId, string culture, int maxResult)
        {
            var listRequest = _youTubeService.Playlists.List("snippet,contentDetails,player");
            listRequest.ChannelId = channelId;
            listRequest.Hl = I18nLanguages.GetHl(culture); ;
            listRequest.Key = _youTubeServiceControl.ApiKey;
            listRequest.MaxResults = maxResult;

            var playlistResponse = await listRequest.ExecuteAsync();
            return playlistResponse.Items.Select(playlist => new MPlaylist(playlist)).Cast<IPlaylist>().ToList();
        }

        public async Task<IPlaylistItemList> GetPlaylistItems(string playlistId, int maxResult, string nextPageToken)
        {
            //var playlistItemList = _playlistCache.GetPlaylistItemList(nextPageToken);
            //if (playlistItemList != null)
            //    return playlistItemList;

            var listRequest = _youTubeService.PlaylistItems.List("snippet,contentDetails");
            listRequest.Key = _youTubeServiceControl.ApiKey;
            listRequest.MaxResults = maxResult;
            listRequest.PlaylistId = playlistId;
            listRequest.PageToken = nextPageToken;

            PlaylistItemListResponse playlistResponse = await listRequest.ExecuteAsync();
            var playlistItemList = new MPlaylistItemList(playlistResponse);
            //_playlistCache.AddPlaylistItemList(nextPageToken, playlistItemList);
            return playlistItemList;
        }

        public async Task<IChannel> GetChannel(string channelId)
        {
            var channelRequest = _youTubeService.Channels.List("snippet,statistics,brandingSettings");
            channelRequest.Key = _youTubeServiceControl.ApiKey;
            channelRequest.Id = channelId;

            var response = await channelRequest.ExecuteAsync();
            var channel = response.Items.FirstOrDefault();
            return channel != null ? new MChannel(channel) : null;
        }

        public async Task<IVideoList> GetRelatedVideos(string videoId, int maxResult, string pageToken)
        {
            var res = await _youTubeWeb.GetRelatedVideo(videoId, _youTubeServiceControl.OAuthToken, pageToken);
            if (res == null)
                return null;

            var videoIds = new StringBuilder();
            foreach (var id in res.Ids)
            {
                videoIds.AppendLine(id);
                videoIds.AppendLine(",");
            }

            var videos = await GetVideo(videoIds.ToString());
            videos.NextPageToken = res.NextPageToken;
            return new MVideoList(videos);
            //var request = _youTubeService.Search.List("snippet,id");
            //request.Key = _youTubeServiceControl.ApiKey;
            //request.RelatedToVideoId = videoId;
            //request.PageToken = pageToken;
            //request.MaxResults = 45;
            //request.Type = "video";

            //var response = await request.ExecuteAsync();
            //var ids = new StringBuilder();
            //foreach (var item in response.Items)
            //{
            //    ids.AppendLine(item.Id.VideoId);
            //    ids.AppendLine(",");
            //}
            //var videoDetails = await GetVideoDetails(ids.ToString());
            //return new MVideoList(response, videoDetails);
        }

        public async Task<IVideoList> GetCategoryVideoList(string categoryId, string culture, int maxResult, string pageToken)
        {
            var videoRequest = _youTubeService.Videos.List("snippet,contentDetails,statistics,player");
            videoRequest.RegionCode = I18nLanguages.GetRegionCode(culture);
            videoRequest.Chart = VideosResource.ListRequest.ChartEnum.MostPopular;
            videoRequest.Key = _youTubeServiceControl.ApiKey;
            videoRequest.MaxResults = maxResult;
            videoRequest.PageToken = pageToken;
            videoRequest.VideoCategoryId = categoryId;

            var videoResponse = await videoRequest.ExecuteAsync();
            return new MVideoList(videoResponse);
        }

        public async Task<IVideoList> GetChannelVideoList(string channelId, string culture, int maxPageResult, string nextPageToken)
        {
            if (string.IsNullOrEmpty(nextPageToken))
            {
                var response = await GetChannelVideosApi(channelId, maxPageResult, nextPageToken);
                if (response.Items.Count != 0) 
                    return response;
                
                return await GetChannelVideosWeb(channelId, nextPageToken);
            }

            var index = 0;
            if (int.TryParse(nextPageToken, out index))
                return await GetChannelVideosWeb(channelId, nextPageToken);

            return await GetChannelVideosApi(channelId, maxPageResult, nextPageToken);
        }

        public async Task<IEnumerable<IGuideCategory>> GetGuideCategories(string culture)
        {
            var categoriesListRequest = _youTubeService.GuideCategories.List("snippet");
            categoriesListRequest.Key = _youTubeServiceControl.ApiKey;
            categoriesListRequest.RegionCode = I18nLanguages.GetRegionCode(culture);
            categoriesListRequest.Hl = I18nLanguages.GetHl(culture); ;

            var categoriesListResponse = await categoriesListRequest.ExecuteAsync();
            return categoriesListResponse.Items.Select(category => new MGuideCategory(category)).Cast<IGuideCategory>().ToList();
        }

        public async Task<IChannelList> GetChannels(string categoryId, string culture, int maxPageResult, string nextPageToken)
        {
            var channelListRequest = _youTubeService.Channels.List("snippet,statistics,brandingSettings");
            channelListRequest.Key = _youTubeServiceControl.ApiKey;
            channelListRequest.Hl = I18nLanguages.GetHl(culture);
            channelListRequest.CategoryId = categoryId;
            channelListRequest.MaxResults = maxPageResult;
            channelListRequest.PageToken = nextPageToken;

            var channelListResponse = await channelListRequest.ExecuteAsync();
            return new MChannelList(channelListResponse);
        }

        public async Task<IVideoList> Search(string searchString, int maxResult, string nextPageToken)
        {
            var request = _youTubeService.Search.List("snippet,id");
            request.Key = _youTubeServiceControl.ApiKey;
            request.PageToken = nextPageToken;
            request.MaxResults = SEARCH_PAGE_MAX_RESULT;
            request.Type = "video";
            request.Q = searchString;

            var response = await request.ExecuteAsync();
            var ids = new StringBuilder();
            foreach (var item in response.Items)
            {
                ids.AppendLine(item.Id.VideoId);
                ids.AppendLine(",");
            }
            var videoDetails = await GetVideoDetails(ids.ToString());
            return new MVideoList(response, videoDetails);
        }

        public async Task<ICommentList> GetComments(string videoId, int maxResult, string nextPageToken)
        {
            try
            {
                var youTubeService = _youTubeServiceControl.GetService();
                var request = youTubeService.CommentThreads.List("snippet,replies");
                request.Key = _youTubeServiceControl.ApiKey;
                request.PageToken = nextPageToken;
                request.MaxResults = maxResult;
                request.VideoId = videoId;
                request.TextFormat = CommentThreadsResource.ListRequest.TextFormatEnum.PlainText;
                //request.Order = CommentThreadsResource.ListRequest.OrderEnum.Time;
                //request.PrettyPrint = true;

                var response = await request.ExecuteAsync();
                return new MCommentList(response);
            }
            catch (GoogleApiException e)
            {
                if (e.HttpStatusCode == HttpStatusCode.Forbidden)
                    return MCommentList.EmptyList;

                throw e;
            }
        }

        public async Task<ISubscriptionList> GetSubscribtions(int maxResult, string nextPageToken)
        {
            var youTubeService = _youTubeServiceControl.GetAuthorizedService();
            var request = youTubeService.Subscriptions.List("snippet,contentDetails");
            request.Key = _youTubeServiceControl.ApiKey;
            request.PageToken = nextPageToken;
            request.MaxResults = maxResult;
            request.Mine = true;
            request.Order = SubscriptionsResource.ListRequest.OrderEnum.Unread;
            //request.OauthToken = _youTubeServiceControl.OAuthToken;

            var response = await request.ExecuteAsync();
            return new MSubscriptionList(response);
        }

        public async Task<IVideoList> GetHistory(int maxResult, string nextPageToken)
        {
            await SetRelatedPlaylists();
            var playListItems = await GetPlaylistItems(_historyPlayListId, maxResult, nextPageToken);
            var videoList = await GetVideoList(playListItems);
            return videoList;
        }

        public bool IsSubscribed(string channelId)
        {
            return _subscriptionsHolder.IsSubsribed(channelId);
        }

        public async Task Subscribe(string channelId)
        {
            if (string.IsNullOrEmpty(channelId))
                return;

            await _subscriptionsHolder.Subscribe(channelId);
        }

        public async Task Unsubscribe(string subscriptionId)
        {
            if (string.IsNullOrEmpty(subscriptionId))
                return;

            await _subscriptionsHolder.Unsubscribe(subscriptionId);
        }

        public string GetSubscriptionId(string channelId)
        {
            return _subscriptionsHolder.GetSubsriptionId(channelId);
        }

        public async Task SetRating(string videoId, RatingEnum rating)
        {
            if (string.IsNullOrEmpty(videoId))
                return;

            var request = _youTubeService.Videos.Rate(videoId, rating.GetYTRating());
            request.Key = _youTubeServiceControl.ApiKey;
            //request.OauthToken = _youTubeServiceControl.OAuthToken;

            var res = await request.ExecuteAsync();
        }

        public async Task<RatingEnum> GetRating(string videoId)
        {
            if (string.IsNullOrEmpty(videoId))
                return RatingEnum.None;

            var request = _youTubeService.Videos.GetRating(videoId);
            request.Key = _youTubeServiceControl.ApiKey;
            //request.OauthToken = _youTubeServiceControl.OAuthToken;

            var rating = await request.ExecuteAsync();
            var item = rating.Items.FirstOrDefault();
            if (item == null)
                return RatingEnum.None;

            return item.Rating.GetRating();
        }

        private async Task<IEnumerable<IVideoDetails>> GetVideoDetails(string ids)
        {
            var request = _youTubeService.Videos.List("snippet,contentDetails,statistics");
            request.Key = _youTubeServiceControl.ApiKey;
            request.Id = ids;

            var response = await request.ExecuteAsync();
            var result = new List<IVideoDetails>(response.Items.Select(v => new MVideoDetails(v)));
            return result;
        }

        private async Task<IVideoList> GetVideoList(IPlaylistItemList playListItems)
        {
            var ids = new StringBuilder();
            foreach (var item in playListItems.Items)
            {
                ids.AppendLine(item.Snippet.ResourceId.VideoId);
                ids.AppendLine(",");
            }

            var request = _youTubeService.Videos.List("snippet,contentDetails,statistics");
            request.Key = _youTubeServiceControl.ApiKey;
            request.Id = ids.ToString();

            var response = await request.ExecuteAsync();
            return new MVideoList(response, playListItems);
        }

        public async Task<YouTubeUri> GetVideoUriAsync(string videoId, YouTubeQuality quality)
        {
            //await InsertHistory(videoId);
            YouTubeWeb.OpenVideo(videoId, _youTubeServiceControl.OAuthToken);
            var url = await YouTube.GetVideoUriAsync(videoId, /*_youTubeServiceControl.OAuthToken, */quality);
            return url;
        }

        private async Task SetRelatedPlaylists()
        {
            if (_historyPlayListId == null || _uploadPlayList == null ||
                _watchLaterPlayList == null || _likesPlayList == null ||
                _favorites == null || _profileInfo == null)
            {
                var youTubeService = _youTubeServiceControl.GetAuthorizedService();
                var request = youTubeService.Channels.List("contentDetails,snippet");
                request.Key = _youTubeServiceControl.ApiKey;
                request.PageToken = string.Empty;
                request.Mine = true;
                //request.OauthToken = _youTubeServiceControl.OAuthToken;

                var response = await request.ExecuteAsync();
                _historyPlayListId = response.Items.First().ContentDetails.RelatedPlaylists.WatchHistory;
                _uploadPlayList = response.Items.First().ContentDetails.RelatedPlaylists.Uploads;
                _watchLaterPlayList = response.Items.First().ContentDetails.RelatedPlaylists.WatchLater;
                _likesPlayList = response.Items.First().ContentDetails.RelatedPlaylists.Likes;
                _favorites = response.Items.First().ContentDetails.RelatedPlaylists.Favorites;

                var name = response.Items.First().Snippet.Title;
                var image = new MThumbnailDetails(response.Items.First().Snippet.Thumbnails).GetThumbnailUrl();
                var registered = response.Items.First().Snippet.PublishedAt;
                var channelId = response.Items.First().Id;
                _profileInfo = new MProfile(channelId, image, name, registered);
            }
        }

        private async Task InsertHistory(string videoId)
        {
            if (!IsAuthorized)
                return;

            await SetRelatedPlaylists();
            var newPlaylistItem = new PlaylistItem
            {
                Snippet = new PlaylistItemSnippet
                {
                    PlaylistId = _historyPlayListId,
                    ResourceId = new ResourceId { Kind = "youtube#video", VideoId = videoId }
                }
            };
            var youtubeService = _youTubeServiceControl.GetAuthorizedService();
            var request = youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet");
            request.Key = _youTubeServiceControl.ApiKey;
            //request.OauthToken = _youTubeServiceControl.OAuthToken;

            newPlaylistItem = await request.ExecuteAsync();
        }

        private async Task<IVideoList> GetChannelVideosWeb(string channelId, string nextPageToken)
        {
            var res = await _youTubeWeb.GetChannelVideos(channelId, _youTubeServiceControl.OAuthToken, nextPageToken);
            if (res == null)
                return MVideoList.EmptyVideoList;

            var videoIds = new StringBuilder();
            foreach (var id in res.Ids)
            {
                videoIds.AppendLine(id);
                videoIds.AppendLine(",");
            }

            var videos = await GetVideo(videoIds.ToString());
            videos.NextPageToken = res.NextPageToken;
            return new MVideoList(videos);
        }

        private async Task<IVideoList> GetChannelVideosApi(string channelId, int maxPageResult, string nextPageToken)
        {
            var request = _youTubeService.Search.List("snippet,id");
            request.Key = _youTubeServiceControl.ApiKey;
            request.PageToken = nextPageToken;
            request.MaxResults = maxPageResult;
            request.Type = "video";
            request.ChannelId = channelId;
            request.Order = SearchResource.ListRequest.OrderEnum.Date;

            var response = await request.ExecuteAsync();
            var ids = new StringBuilder();
            foreach (var item in response.Items)
            {
                ids.AppendLine(item.Id.VideoId);
                ids.AppendLine(",");
            }
            var videoDetails = await GetVideoDetails(ids.ToString());
            return new MVideoList(response, videoDetails);
        }

        public async Task AddToFavorites(string videoId)
        {
            if (!IsAuthorized)
                return;

            await SetRelatedPlaylists();
            var newPlaylistItem = new PlaylistItem
            {
                Snippet = new PlaylistItemSnippet
                {
                    PlaylistId = _favorites,
                    ResourceId = new ResourceId { Kind = "youtube#video", VideoId = videoId }
                }
            };
            var youtubeService = _youTubeServiceControl.GetAuthorizedService();
            var request = youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet");
            request.Key = _youTubeServiceControl.ApiKey;
            //request.OauthToken = _youTubeServiceControl.OAuthToken;

            newPlaylistItem = await request.ExecuteAsync();
        }

        public async Task RemoveFromFavorites(string playlistItemId)
        {
            if (!IsAuthorized)
                return;

            await SetRelatedPlaylists();
            var youtubeService = _youTubeServiceControl.GetAuthorizedService();
            var request = youtubeService.PlaylistItems.Delete(playlistItemId);
            request.Key = _youTubeServiceControl.ApiKey;
            //request.OauthToken = _youTubeServiceControl.OAuthToken;

            await request.ExecuteAsync();
        }

        public async Task<IResponceList> GetFavorites(int maxResult, string nextPageToken)
        {
            await SetRelatedPlaylists();
            var playListItems = await GetPlaylistItems(_favorites, maxResult, nextPageToken);
            return playListItems;
        }

        public async Task<IVideoItem> GetVideoItem(string videoId)
        {
            var res = await GetVideo(videoId);
            var video = res.Items.FirstOrDefault();
            if (video == null)
                return MVideoItem.Empty;

            return new MVideoItem(video);
        }

        public async Task<IProfile> GetProfile()
        {
            if (!IsAuthorized)
            {
                const string image = "https://yt3.ggpht.com/-v6fA9YDXkMs/AAAAAAAAAAI/AAAAAAAAAAA/_GjtZC3QejY/s88-c-k-no/photo.jpg";
                const string displayName = "";
                return new MProfile(string.Empty, image, displayName);
            }

            await SetRelatedPlaylists();
            return _profileInfo;
        }

        public async Task<IComment> AddComment(string channelId, string videoId, string text)
        {
            if (!IsAuthorized)
                return null;

            if (string.IsNullOrEmpty(text))
                return null;

            // Insert channel comment by omitting videoId.
            // Create a comment snippet with text.
            var commentSnippet = new CommentSnippet { TextOriginal = text };

            // Create a top-level comment with snippet.
            var topLevelComment = new Comment {Snippet = commentSnippet};

            // Create a comment thread snippet with channelId and top-level comment.
            var commentThreadSnippet  = new CommentThreadSnippet
            {
                ChannelId = channelId,
                VideoId = videoId,
                TopLevelComment = topLevelComment
            };
            
            // Create a comment thread with snippet.
            var commentThread = new CommentThread { Snippet = commentThreadSnippet };

            var youtubeService = _youTubeServiceControl.GetAuthorizedService();
            var request = youtubeService.CommentThreads.Insert(commentThread, "snippet");
            request.Key = _youTubeServiceControl.ApiKey;

            var response = await request.ExecuteAsync();
            //небольшой хак. т.к. response не содержит текст((
            response.Snippet.TopLevelComment.Snippet.TextDisplay = text;
            return new MComment(response);
        }
    }
}
