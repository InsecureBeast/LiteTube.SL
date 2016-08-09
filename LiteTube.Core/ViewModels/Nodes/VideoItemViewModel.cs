using System;
using LiteTube.Core.DataClasses;

namespace LiteTube.Core.ViewModels.Nodes
{
    public class VideoItemViewModel : NodeViewModelBase
    {
        private readonly string _videoId;
        private readonly string _id;
        private bool _isNowPlaying;

        public VideoItemViewModel(IVideoItem videoItem)
        {
            VideoItem = videoItem;
            _videoId = videoItem.Details.VideoId;
            _id = Guid.NewGuid().ToString();
            Title = videoItem.Details.Title;
            ChannelTitle = videoItem.ChannelTitle;
            Description = videoItem.Details.Description;
            ImagePath = videoItem.Thumbnails.GetThumbnailUrl();
            Duration = videoItem.Details.Duration;
            ViewCount = videoItem.Details.Statistics.ViewCount;
            PublishedAt = videoItem.PublishedAt;
        }

        public IVideoItem VideoItem { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public TimeSpan Duration { get; private set; }
        public string ChannelTitle { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public ulong? ViewCount { get; private set; }

        public bool IsNowPlaying
        {
            get { return _isNowPlaying; }
            set
            {
                _isNowPlaying = value;
                NotifyOfPropertyChanged(() => IsNowPlaying);
            }
        }

        public override string Id
        {
            get { return _id; }
        }

        public override string VideoId
        {
            get { return _videoId; }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}