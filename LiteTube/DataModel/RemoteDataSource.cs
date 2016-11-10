using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using LiteTube.Common;
using LiteTube.DataClasses;
using LiteTube.Multimedia;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Google;
using VideoLibrary;

namespace LiteTube.DataModel
{
    interface IRemoteDataSource
    {
        Task Login();
        void Logout();
        Task LoginSilently(string username);
        bool IsAuthorized { get; }
        string FavoritesPlaylistId { get; }
        string WatchLaterPlaylistId { get; }
        string UploadedPlaylistId { get; }
        Task<IEnumerable<IVideoCategory>> GetCategories(string culture);
        Task<IVideoList> GetActivity(string culture, int maxResult, string pageToken);
        Task<IVideoList> GetRecommended(string pageToken);
        Task<IVideoList> GetMostPopular(string culture, int maxResult, string pageToken);
        Task<IChannel> GetChannel(string channelId);
        Task<IChannel> GetChannelByUsername(string username);
        Task<IVideoList> GetRelatedVideos(string videoId, int maxResult, string pageToken);
        Task<IVideoList> GetCategoryVideoList(string categoryId, string culture, int maxResult, string pageToken);
        Task<IVideoList> GetChannelVideoList(string channelId, string culture, int maxPageResult, string pageToken);
        Task<IEnumerable<IGuideCategory>> GetGuideCategories(string culture);
        Task<IChannelList> GetChannels(string categoryId, string culture, int maxPageResult, string nextPageToken);
        Task<IResponceList> Search(string searchString, int maxResult, string nextPageToken, string culture, SearchType serachType, SearchFilter searchFilter);
        Task<ICommentList> GetComments(string videoId, int maxResult, string nextPageToken);
        Task<ISubscriptionList> GetSubscribtions(int maxResult, string nextPageToken);
        bool IsSubscribed(string channelId);
        string GetSubscriptionId(string channelId);
        Task Subscribe(string channelId);
        Task Unsubscribe(string subscriptionId);
        Task SetRating(string videoId, RatingEnum rating);
        Task<RatingEnum> GetRating(string videoId);
        Task<YouTubeUri> GetVideoUriAsync(string videoId, Multimedia.YouTubeQuality quality);
        Task<IVideoItem> GetVideoItem(string videoId);
        IProfile GetProfile();
        Task<IComment> AddComment(string channelId, string videoId, string text);
        Task<IEnumerable<string>> GetAutoCompleteSearchItems(string query);
#region playlists
        Task<IVideoList> GetHistory(int maxResult, string nextPageToken);
        Task AddToPlaylist(string videoId, string playlistId);
        Task RemovePlaylistItem(string playlistItemId);
        Task<IResponceList> GetFavorites(int maxResult, string nextPageToken);
        Task<IResponceList> GetLiked(int maxResult, string nextPageToken);
        Task<IPlaylistList> GetChannelPlaylistList(string channelId, int maxResult, string nextPageToken);
        Task<IPlaylistList> GetMyPlaylistList(int maxResult, string nextPageToken);
        //Task<IPlaylistItemList> GetPlaylistItems(string playlistId, int maxResult, string nextPageToken);
        Task<IVideoList> GetVideoPlaylist(string playListId, int maxResult, string nextPageToken);
#endregion
    }

    class RemoteDataSource : IRemoteDataSource
    {
        private readonly IYouTubeService _youTubeServiceControl;
        private YouTubeService _youTubeService;
        private string _historyPlayListId;
        private string _uploadPlayListId;
        private string _watchLaterPlayListId;
        private string _likedPlayListId;
        private string _favoritesPlaylistId;
        private readonly SubscriptionsHolder _subscriptionsHolder;
        private readonly YouTubeWeb _youTubeWeb;
        private const long SEARCH_PAGE_MAX_RESULT = 45;
        private MProfile _profileInfo;
        private readonly IEnumerable<IPlaylistList> _playlists;

        public RemoteDataSource(IYouTubeService youTubeServiceControl)
        {
            _youTubeServiceControl = youTubeServiceControl;
            _youTubeService = _youTubeServiceControl.GetService();
            _subscriptionsHolder = new SubscriptionsHolder(_youTubeServiceControl);
            _youTubeWeb = new YouTubeWeb();
            _playlists = new List<IPlaylistList>();
        }

        public async Task Login()
        {
            await _youTubeServiceControl.Login();
            _youTubeService = _youTubeServiceControl.GetAuthorizedService();
            await _subscriptionsHolder.Init();
            await LoadProfileInfo();
        }

        public async Task LoginSilently(string username)
        {
            await _youTubeServiceControl.RefreshToken(username);
            await _subscriptionsHolder.Init();
            _youTubeService = _youTubeServiceControl.GetAuthorizedService();
            await LoadProfileInfo();
        }

        public void Logout()
        {
            _youTubeServiceControl.Logout();
            _youTubeService = _youTubeServiceControl.GetService();
            _subscriptionsHolder.Clear();
            ClearProfileInfo();
        }

        public async Task<IVideoList> GetActivity(string culture, int maxResult, string pageToken)
        {
            if (!IsAuthorized)
                return MActivityList.Empty;

            //return await GetActivityApi(culture, maxResult, pageToken);
            return await GetActivityWeb(culture, maxResult, pageToken);
        }

        private async Task<IVideoList> GetActivityApi(string culture, int maxResult, string pageToken)
        {
            var activityRequest = _youTubeService.Activities.List("contentDetails");
            activityRequest.RegionCode = I18nLanguages.GetRegionCode(culture);
            activityRequest.MaxResults = SEARCH_PAGE_MAX_RESULT;
            activityRequest.PageToken = pageToken;
            //activityRequest.Home = true; //deprecated from 12 sept 2016
            activityRequest.Key = _youTubeServiceControl.ApiKey;
            activityRequest.PublishedAfter = DateTime.Today;
            //activityRequest.OauthToken = _youTubeServiceControl.OAuthToken;

            var activityResponse = await activityRequest.ExecuteAsync();
            var videoIds = new StringBuilder();
            var filteredList = activityResponse.Items.Where(i => i.ContentDetails.Upload != null);// || i.ContentDetails.Recommendation != null);
            foreach (var item in filteredList)
            {
                videoIds.AppendLine(MVideoItem.GetVideoId(item.ContentDetails));
                videoIds.AppendLine(",");
            }
            var videos = await GetVideo(videoIds.ToString());
            return new MActivityList(activityResponse, videos);
        }

        private async Task<IVideoList> GetActivityWeb(string culture, int maxResult, string pageToken)
        {
            var res = await _youTubeWeb.GetActivity(_youTubeServiceControl.OAuthToken, pageToken);
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
            var videoRequest = _youTubeService.Videos.List("snippet,contentDetails,statistics");
            videoRequest.Id = videoId;
            videoRequest.Key = _youTubeServiceControl.ApiKey;
            videoRequest.PrettyPrint = true;

            var videoResponse = await videoRequest.ExecuteAsync();
            return videoResponse;
        }

        public async Task<IVideoList> GetMostPopular(string culture, int maxResult, string pageToken)
        {
            var videoRequest = _youTubeService.Videos.List("snippet,contentDetails,statistics");
            videoRequest.RegionCode = I18nLanguages.GetRegionCode(culture);
            videoRequest.Chart = VideosResource.ListRequest.ChartEnum.MostPopular;
            videoRequest.Hl = I18nLanguages.GetHl(culture); ;
            videoRequest.Key = _youTubeServiceControl.ApiKey;
            videoRequest.MaxResults = maxResult;
            videoRequest.PageToken = pageToken;

            var videoResponse = await videoRequest.ExecuteAsync();
            return new MVideoList(videoResponse);
        }

        public IEnumerable<IPlaylistList> Playlists
        {
            get { return _playlists; }
        }

        public async Task<IEnumerable<IVideoCategory>> GetCategories(string culture)
        {
            var categoriesListRequest = _youTubeService.VideoCategories.List("snippet");
            categoriesListRequest.Key = _youTubeServiceControl.ApiKey;
            categoriesListRequest.RegionCode = I18nLanguages.GetRegionCode(culture);
            categoriesListRequest.Hl = I18nLanguages.GetHl(culture);

            var categoriesListResponse = await categoriesListRequest.ExecuteAsync();
            var items = categoriesListResponse.Items.Where(c => c.Snippet != null && c.Snippet.Assignable == true);
            return items.Select(category => new MVideoCategory(category)).Cast<IVideoCategory>().ToList();
        }

        public bool IsAuthorized
        {
            get { return _youTubeServiceControl.IsAuthorized && _profileInfo != null; }
        }

        public string FavoritesPlaylistId
        {
            get { return _favoritesPlaylistId; }
        }

        public string WatchLaterPlaylistId
        {
            get { return _watchLaterPlayListId; }
        }

        public string UploadedPlaylistId
        {
            get { return _uploadPlayListId; }
        }

        public async Task<IPlaylistItemList> GetPlaylistItems(string playlistId, int maxResult, string nextPageToken)
        {
            if (string.IsNullOrEmpty(playlistId))
                return MPlaylistItemList.Empty;

            var listRequest = _youTubeService.PlaylistItems.List("snippet,contentDetails");
            listRequest.Key = _youTubeServiceControl.ApiKey;
            listRequest.MaxResults = maxResult;
            listRequest.PlaylistId = playlistId;
            listRequest.PageToken = nextPageToken;

            PlaylistItemListResponse playlistResponse = await listRequest.ExecuteAsync();
            return new MPlaylistItemList(playlistResponse);
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

        public async Task<IChannel> GetChannelByUsername(string username)
        {
            var channelRequest = _youTubeService.Channels.List("snippet,id,statistics,brandingSettings");
            channelRequest.Key = _youTubeServiceControl.ApiKey;
            channelRequest.ForUsername = username;

            var response = await channelRequest.ExecuteAsync();
            var channel = response.Items.FirstOrDefault();
            return channel != null ? new MChannel(channel) : null;
        }

        public async Task<IVideoList> GetRelatedVideos(string videoId, int maxResult, string pageToken)
        {
            var res = await _youTubeWeb.GetRelatedVideo(videoId, _youTubeServiceControl.OAuthToken, pageToken);
            if (res == null)
                return MVideoList.Empty;

            var videoIds = new StringBuilder();
            foreach (var id in res.Ids)
            {
                videoIds.AppendLine(id);
                videoIds.AppendLine(",");
            }

            var videos = await GetVideo(videoIds.ToString());
            if (videoIds == null)
                return MVideoList.Empty;

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

        public async Task<IResponceList> Search(string searchString, int maxResult, string nextPageToken, string culture, SearchType searchType, SearchFilter searchFilter)
        {
            var request = _youTubeService.Search.List("snippet,id");
            request.Key = _youTubeServiceControl.ApiKey;
            request.PageToken = nextPageToken;
            request.MaxResults = SEARCH_PAGE_MAX_RESULT;
            request.Type = searchType.ToTypeString();
            request.Q = searchString;
            request.Order = searchFilter.Order;
            request.PublishedAfter = searchFilter.PublishedAfter;
            request.PublishedBefore = searchFilter.PublishedBefore;
            request.RegionCode = I18nLanguages.GetRegionCode(culture);
            request.RelevanceLanguage = I18nLanguages.GetHl(culture);

            if (searchType == SearchType.Video)
            {
                request.VideoDuration = searchFilter.VideoDuration;
                request.VideoDefinition = searchFilter.VideoDefinition;
            }

            var response = await request.ExecuteAsync();
            
            if (searchType == SearchType.Video)
            {
                var ids = new StringBuilder();
                foreach (var item in response.Items)
                {
                    ids.AppendLine(item.Id.VideoId);
                    ids.AppendLine(",");
                }
                var videoDetails = await GetVideoDetails(ids.ToString());
                return new MVideoList(response, videoDetails);
            }

            if (searchType == SearchType.Channel)
            {
                return new MChannelList(response);
            }

            if (searchType == SearchType.Playlist)
            {
                var ids = new StringBuilder();
                foreach (var item in response.Items)
                {
                    ids.AppendLine(item.Id.PlaylistId);
                    ids.AppendLine(",");
                }
                var list = await GetPlaylistList(ids.ToString(), response.NextPageToken);
                return list;
            }

            return MVideoList.Empty;
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
                
                var response = await request.ExecuteAsync();
                return new MCommentList(response);
            }
            catch (GoogleApiException e)
            {
                if (e.HttpStatusCode == HttpStatusCode.Forbidden)
                    return MCommentList.EmptyList;

                return MCommentList.EmptyList;
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
            if (!IsAuthorized)
                return MVideoList.Empty;

            if (string.IsNullOrEmpty(_historyPlayListId))
                return MVideoList.Empty;

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

        public async Task<IVideoList> GetVideoPlaylist(string playListId, int maxResult, string nextPageToken)
        {
            if (string.IsNullOrEmpty(playListId))
                return MVideoList.Empty;

            var playListItems = await GetPlaylistItems(playListId, maxResult, nextPageToken);
            var videoList = await GetVideoList(playListItems);
            return videoList;
        }

        public async Task<YouTubeUri> GetVideoUriAsync(string videoId, YouTubeQuality quality)
        {
            var uri = string.Format("https://www.youtube.com/watch?v={0}", videoId);
            var video = await VideoLibrary.YouTube.GetVideoAsync(uri, VideoQualityHelper.GetVideoQuality(quality));
            var url = await video.GetUriAsync();
            return new YouTubeUri() { Uri = new Uri(url) };
        }

        public async Task AddToPlaylist(string videoId, string playlistId)
        {
            if (!IsAuthorized)
                return;

            if (string.IsNullOrEmpty(playlistId))
                return;

            var newPlaylistItem = new PlaylistItem
            {
                Snippet = new PlaylistItemSnippet
                {
                    PlaylistId = playlistId,
                    ResourceId = new ResourceId { Kind = "youtube#video", VideoId = videoId }
                }
            };
            var youtubeService = _youTubeServiceControl.GetAuthorizedService();
            var request = youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet");
            request.Key = _youTubeServiceControl.ApiKey;
            //request.OauthToken = _youTubeServiceControl.OAuthToken;

            await request.ExecuteAsync();
        }

        public async Task RemovePlaylistItem(string playlistItemId)
        {
            if (!IsAuthorized)
                return;

            var youtubeService = _youTubeServiceControl.GetAuthorizedService();
            var request = youtubeService.PlaylistItems.Delete(playlistItemId);
            request.Key = _youTubeServiceControl.ApiKey;
            //request.OauthToken = _youTubeServiceControl.OAuthToken;

            await request.ExecuteAsync();
        }

        public async Task<IResponceList> GetFavorites(int maxResult, string nextPageToken)
        {
            if (!IsAuthorized)
                return MResponceList.Empty;

            if (string.IsNullOrEmpty(_favoritesPlaylistId))
                return MResponceList.Empty;

            var playListItems = await GetPlaylistItems(_favoritesPlaylistId, maxResult, nextPageToken);
            return playListItems;
        }

        public async Task<IResponceList> GetLiked(int maxResult, string nextPageToken)
        {
            if (!IsAuthorized)
                return MResponceList.Empty;

            if (string.IsNullOrEmpty(_likedPlayListId))
                return MResponceList.Empty;

            var playListItems = await GetPlaylistItems(_likedPlayListId, maxResult, nextPageToken);
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

        public IProfile GetProfile()
        {
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

        public async Task<IEnumerable<string>> GetAutoCompleteSearchItems(string query)
        {
            return await YouTubeWeb.HttpGetAutoCompleteAsync(query);
        }

        public async Task<IPlaylistList> GetChannelPlaylistList(string channelId, int maxResult, string nextPageToken)
        {
            var request = _youTubeService.Playlists.List("snippet,id,contentDetails");
            request.Key = _youTubeServiceControl.ApiKey;
            request.PageToken = nextPageToken;
            request.MaxResults = SEARCH_PAGE_MAX_RESULT;
            request.ChannelId = channelId;

            var response = await request.ExecuteAsync();
            return new MPlaylistList(response);
        }

        public async Task<IPlaylistList> GetMyPlaylistList(int maxResult, string nextPageToken)
        {
            var youTubeService = _youTubeServiceControl.GetAuthorizedService();
            var request = youTubeService.Playlists.List("contentDetails,snippet");
            request.Key = _youTubeServiceControl.ApiKey;
            request.PageToken = nextPageToken;
            request.Mine = true;
            request.MaxResults = maxResult;

            var response = await request.ExecuteAsync();
            return new MPlaylistList(response);
        }

        //private void CheckProfile()
        //{
        //    if (!_youTubeServiceControl.IsAuthorized)
        //    {
        //        const string image = "https://yt3.ggpht.com/-v6fA9YDXkMs/AAAAAAAAAAI/AAAAAAAAAAA/_GjtZC3QejY/s88-c-k-no/photo.jpg";
        //        const string displayName = "";
        //        new MProfile(string.Empty, image, displayName);
        //    }

        //    if (_profileInfo == null)
        //    {
        //        _youTubeServiceControl.Logout();
        //        throw new LiteTubeException("YouTube channel not found");
        //    }
        //}

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

        private async Task<IEnumerable<IVideoDetails>> GetVideoDetails(string ids)
        {
            var request = _youTubeService.Videos.List("snippet,contentDetails,statistics");
            request.Key = _youTubeServiceControl.ApiKey;
            request.Id = ids;

            var response = await request.ExecuteAsync();
            var result = new List<IVideoDetails>(response.Items.Select(v => new MVideoDetails(v)));
            return result;
        }

        private async Task LoadProfileInfo()
        {
            if (_historyPlayListId == null || _uploadPlayListId == null ||
                _watchLaterPlayListId == null || _likedPlayListId == null ||
                _favoritesPlaylistId == null || _profileInfo == null)
            {
                var youTubeService = _youTubeServiceControl.GetAuthorizedService();
                var request = youTubeService.Channels.List("contentDetails,snippet");
                request.Key = _youTubeServiceControl.ApiKey;
                request.PageToken = string.Empty;
                request.Mine = true;
                //request.OauthToken = _youTubeServiceControl.OAuthToken;

                var response = await request.ExecuteAsync();
                var item = response.Items.FirstOrDefault();
                if (item == null)
                {
                    Logout();
                    return;
                }

                _historyPlayListId = item.ContentDetails.RelatedPlaylists.WatchHistory;
                _uploadPlayListId = item.ContentDetails.RelatedPlaylists.Uploads;
                _watchLaterPlayListId = item.ContentDetails.RelatedPlaylists.WatchLater;
                _likedPlayListId = item.ContentDetails.RelatedPlaylists.Likes;
                _favoritesPlaylistId = item.ContentDetails.RelatedPlaylists.Favorites;

                var name = item.Snippet.Title;
                var image = new MThumbnailDetails(item.Snippet.Thumbnails).GetThumbnailUrl();
                var registered = item.Snippet.PublishedAt;
                var channelId = item.Id;
                _profileInfo = new MProfile(channelId, image, name, registered);
            }
        }

        private void ClearProfileInfo()
        {
            _historyPlayListId = null;
            _uploadPlayListId = null;
            _watchLaterPlayListId = null;
            _likedPlayListId = null;
            _favoritesPlaylistId = null;
            _profileInfo = null;
        }

        private async Task InsertHistory(string videoId)
        {
            if (!IsAuthorized)
                return;

            await LoadProfileInfo();
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
                return MVideoList.Empty;

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
            var request = _youTubeService.Search.List("snippet");
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

        private async Task<IPlaylistList> GetPlaylistList(string playListIds, string nextPageToken)
        {
            var request = _youTubeService.Playlists.List("snippet,id,contentDetails");
            request.Key = _youTubeServiceControl.ApiKey;
            request.PageToken = string.Empty;
            request.MaxResults = SEARCH_PAGE_MAX_RESULT;
            request.Id = playListIds;

            var response = await request.ExecuteAsync();
            response.NextPageToken = nextPageToken;
            return new MPlaylistList(response);
        }
    }
}
