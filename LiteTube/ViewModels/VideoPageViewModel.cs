﻿using LiteTube.Common;
using LiteTube.Common.Helpers;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using MyToolkit.Multimedia;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiteTube.ViewModels
{
    public class VideoPageViewModel : PropertyChangedBase, IListener<ConnectionEventArgs>
    {
        private Uri _videoUri;
        private IChannel _channel;
        private readonly IDataSource _dataSource;
        private readonly IConnectionListener _connectionListener;
        private readonly NavigationPanelViewModel _navigatioPanelViewModel;
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

        public VideoPageViewModel(string videoId, IDataSource dataSource, IConnectionListener connectionListener)
        {
            Likes = 0;
            Dislikes = 0;
            VideoId = videoId;
            _dataSource = dataSource;
            _connectionListener = connectionListener;
            _connectionListener.Subscribe(this);
            _channelSubscribers = 0;
            _channelVideoCount = 0;

            _subscribeCommand = new SubscribeCommand(_dataSource, () => _channelId, InvalidateCommands);
            _unsubscribeCommand = new UnsubscribeCommand(_dataSource, () => _channelId, InvalidateCommands);

            _likeCommand = new RelayCommand(Like, CanLike);
            _dislikeCommand = new RelayCommand(Dislike, CanLike);
            _addFavoritesCommand = new RelayCommand(AddFavorites);

            _navigatioPanelViewModel = new NavigationPanelViewModel(_dataSource, _connectionListener);

            LoadVideoItem(videoId);
        }

        public string VideoId
        {
            get { return _videoId; }
            private set
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
            get { return _navigatioPanelViewModel; }
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

        public IDataSource DataSource
        {
            get { return _dataSource; }
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

        public void Notify(ConnectionEventArgs e)
        {
            if (e.IsConnected)
            {
                var view = string.Format("/VideoPage.xaml?videoId={0}", _videoId);
                NavigationHelper.Navigate(view, new VideoPageViewModel(_videoId, _dataSource, _connectionListener));
            }
        }

        public override string ToString()
        {
            return Title;
        }

        private void SetVideoUri(string videoId)
        {
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                try
                {
                    IsPaid = false;
                    var url = await _dataSource.GetVideoUriAsync(videoId);
                    VideoUri = url.Uri;
                }
                catch(YouTubeUriNotFoundException)
                {
                    IsPaid = true;
                }
            });
        }

        private void SetLikesAndDislikes(IVideo video)
        {
            var likes = video.Statistics.LikeCount;
            var dislike = video.Statistics.DislikeCount;
            if (likes.HasValue)
                Likes = likes.Value;
            if (dislike.HasValue)
                Dislikes = dislike.Value;
        }

        private void SetChannelInfo(string channelId)
        {
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                var channelInfo = await _dataSource.GetChannel(channelId);
                ChannelImage = channelInfo.Thumbnails.Medium.Url;
                ChannelSubscribers = channelInfo.Statistics.SubscriberCount;
                ChannelVideoCount = channelInfo.Statistics.VideoCount;
                _channel = channelInfo;
                IsSubscribed = _dataSource.IsSubscribed(_channelId);
            });
        }

        private void SetVideoRating(string videoId)
        {
            if (!_dataSource.IsAuthorized)
                return;

            LayoutHelper.InvokeFromUIThread(async () =>
            {
                var rating = await _dataSource.GetRating(videoId);
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
        }

        private async void Like()
        {
            await SetRating();
        }

        private bool CanLike()
        {
            return _dataSource.IsAuthorized;
        }

        private async Task SetRating()
        {
            var rating = RatingEnum.None;
            if (_isLiked)
                rating = RatingEnum.Like;
            if (_isDisliked)
                rating = RatingEnum.Dislike;

            await _dataSource.SetRating(VideoId, rating);
        }

        private void LoadVideoItem(string videoId)
        {
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                var videoItem = await _dataSource.GetVideoItem(videoId);

                RelatedVideosViewModel = new RelatedVideosViewModel(videoItem, _dataSource, _connectionListener);
                CommentsViewModel = new CommentsViewModel(VideoId, _dataSource, _connectionListener);
                
                if (videoItem == null)
                    return;

                //VideoId = videoItem.Details.Video.Id;
                Title = videoItem.Details.Title;
                ChannelTitle = videoItem.ChannelTitle;
                Description = videoItem.Details.Description;
                ImagePath = videoItem.Thumbnails.Medium.Url;
                Duration = videoItem.Details.Duration;
                ViewCount = videoItem.Details.Video.Statistics.ViewCount;
                if (videoItem.PublishedAt != null) 
                    PublishedAt = videoItem.PublishedAt.Value.ToString("D");
                ChannelId = videoItem.ChannelId;
                
                SetLikesAndDislikes(videoItem.Details.Video);
                SetChannelInfo(_channelId);
                SetVideoRating(VideoId);
                SetVideoUri(videoId);
            });
        }

        private async void AddFavorites()
        {
            await _dataSource.AddToFavorites(_videoId);
        }

        private void InvalidateCommands()
        {
            IsSubscribed = _dataSource.IsSubscribed(_channelId);
        }
    }
}
