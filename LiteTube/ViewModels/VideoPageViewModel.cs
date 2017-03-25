using LiteTube.Common;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.Multimedia;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteTube.Common.Helpers;
using System.Collections.Generic;
using LiteTube.Controls;
using System.Linq;
using LiteTube.ViewModels.Playlist;

namespace LiteTube.ViewModels
{
    public class VideoPageViewModel : PropertyChangedBase, IListener<ConnectionEventArgs>, IPlaylistsSevice
    {
        private Uri _videoUri;
        private readonly Func<IDataSource> _getDataSource;
        private readonly IConnectionListener _connectionListener;
        private RelatedVideosViewModel _relatedViewModel;
        private CommentsViewModel _commentsViewModel;
        private string _channelImage;
        private ulong? _channelVideoCount;
        private ulong? _channelSubscribers;
        private readonly SubscribeCommand _subscribeCommand;
        private readonly UnsubscribeCommand _unsubscribeCommand;
        private readonly RelayCommand _likeCommand;
        private readonly RelayCommand _dislikeCommand;
        private readonly RelayCommand _addFavoritesCommand;
        private readonly RelayCommand _videoQualityCommand;
        private readonly VideoQuality _qualityConverter;
        private List<VideoQualityItem> _videoQualities;
        private string _channelId;
        private bool _isSubscribed;
        private bool _isLiked = false;
        private bool _isDisliked = false;
        private bool _isPaid;
        private bool _isLoading;
        private string _videoId;
        private string _title;
        private string _subtitle;
        private string _description;
        private string _imagePath;
        private string _content;
        private TimeSpan _duration;
        private string _channelTitle;
        private string _publishedAt;
        private ulong? _viewCount;
        private ulong _likes;
        private ulong _dislikes;
        private VideoQualityItem _selectedVideoQualityItem;

        public VideoPageViewModel(string videoId, Func<IDataSource> getDataSource, IConnectionListener connectionListener)
        {
            Likes = 0;
            Dislikes = 0;
            VideoId = videoId;
            _getDataSource = getDataSource;
            _connectionListener = connectionListener;
            _connectionListener.Subscribe(this);
            _channelSubscribers = 0;
            _channelVideoCount = 0;
            _qualityConverter = new VideoQuality();

            _subscribeCommand = new SubscribeCommand(_getDataSource, () => _channelId, InvalidateCommands);
            _unsubscribeCommand = new UnsubscribeCommand(_getDataSource, () => _channelId, InvalidateCommands);

            _likeCommand = new RelayCommand(Like, CanLike);
            _dislikeCommand = new RelayCommand(Dislike, CanLike);
            _addFavoritesCommand = new RelayCommand(AddFavorites);
            _videoQualityCommand = new RelayCommand(ChangeVideoQuality);

            LoadVideoQualities();
            LoadVideoItem(videoId);
        }

        public VideoPageViewModel()
        {
            LoadVideoQualities();
        }

        public string VideoId
        {
            get { return _videoId; }
            set
            {   _videoId = value;
                NotifyOfPropertyChanged(() => VideoId);
            }
        }

        public string Title
        {
            get { return _title; }
            private set
            {
                _title = value;
                NotifyOfPropertyChanged(() => Title);
            }
        }

        public string Subtitle
        {
            get { return _subtitle; }
            private set
            {
                _subtitle = value;
                NotifyOfPropertyChanged(() => Subtitle);
            }
        }

        public string Description
        {
            get { return _description; }
            private set
            {
                _description = value;
                NotifyOfPropertyChanged(() => Description);
            }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            private set
            {
                _imagePath = value;
                NotifyOfPropertyChanged(() => ImagePath);
            }
        }

        public string Content
        {
            get { return _content; }
            private set
            {
                _content = value;
                NotifyOfPropertyChanged(() => Content);
            }
        }

        public TimeSpan Duration
        {
            get { return _duration; }
            private set
            {
                _duration = value;
                NotifyOfPropertyChanged(() => Duration);
            }
        }

        public string ChannelTitle
        {
            get { return _channelTitle; }
            private set
            {
                _channelTitle = value;
                NotifyOfPropertyChanged(() => ChannelTitle);
            }
        }

        public string PublishedAt
        {
            get { return _publishedAt; }
            private set
            {
                _publishedAt = value;
                NotifyOfPropertyChanged(() => PublishedAt);
            }
        }

        public ulong? ViewCount
        {
            get { return _viewCount; }
            private set
            {
                _viewCount = value;
                NotifyOfPropertyChanged(() => ViewCount);
            }
        }

        public bool IsLiked
        {
            get { return _isLiked; }
            set
            {
                _isLiked = value;
                if (_isLiked)
                    IsDisliked = false;

                NotifyOfPropertyChanged(() => IsLiked);
            }
        }

        public bool IsDisliked
        {
            get { return _isDisliked; }
            set
            {
                _isDisliked = value;
                if (_isDisliked)
                    IsLiked = false;

                NotifyOfPropertyChanged(() => IsDisliked);
            }
        }

        public NavigationPanelViewModel NavigationPanelViewModel
        {
            get { return App.NavigationPanelViewModel; }
        }

        public PlaylistsContainerViewModel PlaylistListViewModel
        {
            get { return App.ViewModel.PlaylistListViewModel; }
        }

        public Uri VideoUri 
        {
            get { return _videoUri; }
            private set
            {
                _videoUri = value;
                NotifyOfPropertyChanged(() => VideoUri);
            }
        }

        public string ChannelImage
        {
            get { return _channelImage; }
            private set
            {
                _channelImage = value;
                NotifyOfPropertyChanged(() => ChannelImage);
            }
        }

        public ulong? ChannelVideoCount
        {
            get { return _channelVideoCount; }
            private set
            {
                _channelVideoCount = value;
                NotifyOfPropertyChanged(() => ChannelVideoCount);
            }
        }

        public ulong? ChannelSubscribers
        {
            get { return _channelSubscribers; }
            private set
            {
                _channelSubscribers = value;
                NotifyOfPropertyChanged(() => ChannelSubscribers);
            }
        }

        public ulong Likes
        {
            get { return _likes; }
            private set
            {
                _likes = value;
                NotifyOfPropertyChanged(() => Likes);
            }
        }

        public ulong Dislikes
        {
            get { return _dislikes; }
            private set
            {
                _dislikes = value;
                NotifyOfPropertyChanged(() => Dislikes);
            }
        }

        public double LikesProgress
        {
            get
            {
                var total = Likes + Dislikes;
                var div = (double)Likes * 100 / (double)total;
                return Math.Round(div);
            }
        }

        public bool IsLikesProgressVisible
        {
            get { return Likes != 0 && Dislikes != 0; }
        }

        public ICommand SubscribeCommand
        {
            get { return _subscribeCommand; }
        }

        public ICommand UnsubscribeCommand
        {
            get { return _unsubscribeCommand; }
        }

        public ICommand LikeCommand
        {
            get { return _likeCommand; }
        }

        public ICommand DislikeCommand
        {
            get { return _dislikeCommand; }
        }

        public ICommand AddFavoritesCommand
        {
            get { return _addFavoritesCommand; }
        }

        public ICommand VideoQualityCommand
        {
            get { return _videoQualityCommand; }
        }

        public RelatedVideosViewModel RelatedVideosViewModel
        {
            get { return _relatedViewModel; }
            private set
            {
                _relatedViewModel = value;
                NotifyOfPropertyChanged(() => RelatedVideosViewModel);
            }
        }

        public CommentsViewModel CommentsViewModel
        {
            get { return _commentsViewModel; }
            set
            {
                _commentsViewModel = value;
                NotifyOfPropertyChanged(() => CommentsViewModel);
            }
        }

        public string ChannelId
        {
            get { return _channelId; }
            set
            {
                _channelId = value;
                NotifyOfPropertyChanged(() => ChannelId);
            }
        }

        public bool IsSubscribed
        {
            get { return _isSubscribed; }
            set
            {
                _isSubscribed = value;
                NotifyOfPropertyChanged(() => IsSubscribed);
            }
        }

        public bool IsPaid
        {
            get { return _isPaid; }
            set
            {
                _isPaid = value;
                NotifyOfPropertyChanged(() => IsPaid);
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            protected set
            {
                _isLoading = value;
                NotifyOfPropertyChanged(() => IsLoading);
            }
        }

        public List<VideoQualityItem> VideoQualities
        {
            get { return _videoQualities; }
        }

        public VideoQualityItem SelectedVideoQualityItem
        {
            get { return _selectedVideoQualityItem; }
            set
            {
                if (value == null)
                    return;

                var firstLoad = _selectedVideoQualityItem == null;

                _selectedVideoQualityItem = value;
                NotifyOfPropertyChanged(() => SelectedVideoQualityItem);

                if (!firstLoad)
                    ChangeVideoQuality();
            }
        }

        public void Notify(ConnectionEventArgs e)
        {
            if (e.IsConnected)
                LoadVideoItem(_videoId);
        }

        public override string ToString()
        {
            return Title;
        }

        public void Reload()
        {
            LoadVideoItem(_videoId);
        }

        private async void SetVideoUri(string videoId, YouTubeQuality quality)
        {
            try
            {
                var url = await _getDataSource().GetVideoUriAsync(videoId, quality);
                LayoutHelper.InvokeFromUiThread(() =>
                {
                    IsPaid = false;
                    if (url == null)
                        return;

                    VideoUri = url.Uri;
                });
            }
            //Todo разделить исключения по типу и добавить соответсвующий баннер
            catch (YouTubeUriNotFoundException)
            {
                IsPaid = true;
            }
            catch (Exception)
            {
                IsPaid = true;
            }
        }

        private void SetLikesAndDislikes(IVideoStatistics statistics)
        {
            var likes = statistics.LikeCount;
            var dislike = statistics.DislikeCount;
            if (likes.HasValue)
                Likes = likes.Value;
            if (dislike.HasValue)
                Dislikes = dislike.Value;
        }

        private async void SetChannelInfo(string channelId)
        {
            var channelInfo = await _getDataSource().GetChannel(channelId);
            if (channelInfo == null)
                return;

            LayoutHelper.InvokeFromUiThread(() =>
            {
                IsSubscribed = _getDataSource().IsSubscribed(channelId);
                ChannelTitle = channelInfo.Title;

                if (channelInfo.Thumbnails != null)
                    ChannelImage = channelInfo.Thumbnails.GetThumbnailUrl();

                if (channelInfo.Statistics != null)
                {
                    ChannelSubscribers = channelInfo.Statistics.SubscriberCount;
                    ChannelVideoCount = channelInfo.Statistics.VideoCount;
                }
            });
        }

        private async void SetVideoRating(string videoId)
        {
            if (!_getDataSource().IsAuthorized)
                return;

            var rating = await _getDataSource().GetRating(videoId);

            LayoutHelper.InvokeFromUiThread(() =>
            {
                if (rating == RatingEnum.Dislike)
                {
                    IsDisliked = true;
                    return;
                }
                if (rating == RatingEnum.Like)
                {
                    IsLiked = true;
                    return;
                }

                IsLiked = IsDisliked = false;
            });
        }

        private async void Dislike()
        {
            await SetRating();
            if (_isDisliked)
                Dislikes++;
            else
                Dislikes--;
        }

        private async void Like()
        {
            await SetRating();
            if (_isLiked)
                Likes++;
            else
                Likes--;
        }

        private bool CanLike()
        {
            return _getDataSource().IsAuthorized;
        }

        private async Task SetRating()
        {
            var rating = RatingEnum.None;
            if (_isLiked)
                rating = RatingEnum.Like;
            if (_isDisliked)
                rating = RatingEnum.Dislike;

            await _getDataSource().SetRating(VideoId, rating);
        }

        private async void LoadVideoItem(string videoId)
        {
            RelatedVideosViewModel = new RelatedVideosViewModel(videoId, _getDataSource, _connectionListener, this);
            CommentsViewModel = new CommentsViewModel(videoId, _getDataSource, _connectionListener);

            var videoItem = await _getDataSource().GetVideoItem(videoId);
            if (videoItem == null)
                return;

            _channelId = videoItem.ChannelId;
            SetChannelInfo(_channelId);

            LayoutHelper.InvokeFromUiThread(() =>
            {
                if (!_connectionListener.CheckNetworkAvailability())
                    return;

                if (videoItem.Details != null)
                {
                    Title = videoItem.Details.Title;
                    Description = videoItem.Details.Description;
                    Duration = videoItem.Details.Duration;
                    ViewCount = videoItem.Details.Statistics.ViewCount;
                    SetLikesAndDislikes(videoItem.Details.Statistics);
                }

                if (videoItem.Thumbnails != null)
                    ImagePath = videoItem.Thumbnails.GetThumbnailUrl();

                if (videoItem.PublishedAt != null) 
                    PublishedAt = videoItem.PublishedAt.Value.ToString("D");
                
                SetVideoRating(videoId);
                var defaultQuality = SettingsHelper.GetQuality();
                var quality = _qualityConverter.GetQualityEnum(defaultQuality);
                SetVideoUri(videoId, quality);
            });
        }

        private async void AddFavorites()
        {
            var playlistId = _getDataSource().FavoritesPlaylistId;
            await _getDataSource().AddItemToPlaylist(_videoId, playlistId);
        }

        private void InvalidateCommands()
        {
            IsSubscribed = _getDataSource().IsSubscribed(_channelId);
        }

        private void LoadVideoQualities()
        {
            _videoQualities = new List<VideoQualityItem>();
            var currentQuality = SettingsHelper.GetQuality();
            var qualities = new VideoQuality().GetQualityNames();
            foreach (var item in qualities)
            {
                _videoQualities.Add(new VideoQualityItem(item, item == currentQuality));
            }

            SelectedVideoQualityItem = _videoQualities.FirstOrDefault(i => i.IsSelected);
        }

        private void ChangeVideoQuality()
        {
            if (SelectedVideoQualityItem == null)
                return;

            VideoUri = null;
            SetVideoUri(_videoId, SelectedVideoQualityItem.Quality);
            SettingsHelper.SaveQuality(SelectedVideoQualityItem.QualityName);
        }

        public void ShowContainer(bool show, string videoId)
        {
            PlaylistListViewModel.IsContainerShown = show;
            LayoutHelper.InvokeFromUiThread(async () =>
            {
                PlaylistListViewModel.SetVideoId(videoId);
                await PlaylistListViewModel.FirstLoad();
            });
        }
    }
}
