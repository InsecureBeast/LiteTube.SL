﻿using System;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.Common;
using LiteTube.ViewModels.Playlist;
using LiteTube.Common.Helpers;

namespace LiteTube.ViewModels.Nodes
{
    class VideoItemViewModel : NodeViewModelBase
    {
        private readonly string _videoId;
        private readonly string _id;
        private bool _isNowPlaying;
        private bool _isContextMenu;
        private string _channelLogo;

        public VideoItemViewModel(IVideoItem videoItem, IDataSource dataSource, 
            IContextMenuStrategy menuProvider, IPlaylistsSevice playlistService, bool isLargeItems) : base(dataSource, menuProvider, isLargeItems, playlistService)
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
            IsContexMenu = dataSource.IsAuthorized;
            IsLive = videoItem.Details.IsLive;

            if (isLargeItems)
            {
                LayoutHelper.InvokeFromUiThread(async () =>
                {
                    ChannelLogo = await _dataSource.GetChannelLogo(videoItem.ChannelId);
                });
            }
        }

        public IVideoItem VideoItem { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public TimeSpan Duration { get; private set; }
        public string ChannelTitle { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public ulong? ViewCount { get; private set; }
        public bool IsLive { get; private set; }

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

        public bool IsContexMenu
        {
            get { return _isContextMenu; }
            set
            {
                _isContextMenu = value;
                NotifyOfPropertyChanged(() => IsContexMenu);
            }
        }

        public string ChannelLogo
        {
            get { return _channelLogo; }
            set
            {
                _channelLogo = value;
                NotifyOfPropertyChanged(() => ChannelLogo);
            }
        }
    }

    public class PlaylistContainerItem
    {
        public PlaylistContainerItem(string id, string title)
        {
            Id = id;
            Title = title;
        }

        public string Title
        {
            get; private set;
        }

        public string Id
        {
            get; private set;
        }
    }
 }