using System;
using System.ComponentModel;
using System.Globalization;
using LiteTube.DataClasses;

namespace LiteTube.ViewModels
{
    public class VideoItemViewModel : NodeViewModelBase
    {
        private readonly string _videoId;
        private readonly string _id;

        public VideoItemViewModel(IVideoItem videoItem)
        {
            VideoItem = videoItem;
            _videoId = videoItem.Details.Video.Id;
            _id = Guid.NewGuid().ToString();
            Title = videoItem.Details.Title;
            ChannelTitle = videoItem.ChannelTitle;
            Description = videoItem.Details.Description;
            ImagePath = videoItem.Thumbnails.Medium.Url;
            Duration = videoItem.Details.Duration;
            ViewCount = videoItem.Details.Video.Statistics.ViewCount;
            if (videoItem.PublishedAt != null)
                PublishedAt = videoItem.PublishedAt.Value.ToString("d", CultureInfo.CurrentCulture);
            //_channelId = videoItem.ChannelId;
        }

        public IVideoItem VideoItem { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public TimeSpan Duration { get; private set; }
        public string ChannelTitle { get; private set; }
        public string PublishedAt { get; private set; }
        public UInt64? ViewCount { get; private set; }

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

    public class VideoItemViewModel1 : INotifyPropertyChanged
    {
        private string _lineOne;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string LineOne
        {
            get
            {
                return _lineOne;
            }
            set
            {
                if (value != _lineOne)
                {
                    _lineOne = value;
                    NotifyPropertyChanged("LineOne");
                }
            }
        }

        private string _lineTwo;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string LineTwo
        {
            get
            {
                return _lineTwo;
            }
            set
            {
                if (value != _lineTwo)
                {
                    _lineTwo = value;
                    NotifyPropertyChanged("LineTwo");
                }
            }
        }

        private string _lineThree;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string LineThree
        {
            get
            {
                return _lineThree;
            }
            set
            {
                if (value != _lineThree)
                {
                    _lineThree = value;
                    NotifyPropertyChanged("LineThree");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}