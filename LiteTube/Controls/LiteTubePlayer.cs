using LiteTube.Common;
using LiteTube.Multimedia;
using LiteTube.ViewModels;
using LiteTube.ViewModels.Nodes;
using Microsoft.PlayerFramework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Platform;

namespace LiteTube.Controls
{
    public class LiteTubePlayer : MediaPlayer
    {
        public static readonly DependencyProperty VideoTitleProperty = DependencyProperty.Register("VideoTitle", typeof(string), typeof(LiteTubePlayer), null);
        public static readonly DependencyProperty ChannelTitleProperty = DependencyProperty.Register("ChannelTitle", typeof(string), typeof(LiteTubePlayer), null);
        public static readonly DependencyProperty RelatedItemsProperty = DependencyProperty.Register("RelatedItems", typeof(ObservableCollection<NodeViewModelBase>), typeof(LiteTubePlayer), null);
        public static readonly DependencyProperty RelatedItemsVisibleProperty = DependencyProperty.Register("IsRelatedItemsVisible", typeof(bool), typeof(LiteTubePlayer), null);
        public static readonly DependencyProperty RelatedItemsEnabledProperty = DependencyProperty.Register("IsRelatedItemsEnabled", typeof(bool), typeof(LiteTubePlayer), null);
        public static readonly DependencyProperty ItemClickCommandProperty = DependencyProperty.Register("ItemClickCommand", typeof(ICommand), typeof(LiteTubePlayer), null);
        public static readonly DependencyProperty LoadMoreCommandProperty = DependencyProperty.Register("LoadMoreCommand", typeof(ICommand), typeof(LiteTubePlayer), null);

        public static readonly DependencyProperty VideoQualityItemsProperty = DependencyProperty.Register("VideoQualityItems", typeof(List<VideoQualityItem>), typeof(LiteTubePlayer), null);
        public static readonly DependencyProperty SelectedVideoQualityItemProperty = DependencyProperty.Register("SelectedVideoQualityItem", typeof(VideoQualityItem), typeof(LiteTubePlayer), new PropertyMetadata(null, OnVideoQualityChanged));

        //public static readonly DependencyProperty RelatedItemsVisibleProperty = DependencyProperty.Register("IsRelatedItemsVisible", typeof(bool), typeof(LiteTubePlayer), null);

        private bool _isRelatedVisible;

        public event RoutedEventHandler IsSkipNextChanged
        {
            add { ((PlayerInteractiveViewModel)InteractiveViewModel).IsSkipNextChanged += value; }
            remove { ((PlayerInteractiveViewModel)InteractiveViewModel).IsSkipNextChanged -= value; }
        }
        public event RoutedEventHandler IsSkipPreviousChanged
        {
            add { ((PlayerInteractiveViewModel)InteractiveViewModel).IsSkipPreviousChanged += value; }
            remove { ((PlayerInteractiveViewModel)InteractiveViewModel).IsSkipPreviousChanged -= value; }
        }

        public event RoutedEventHandler Paused
        {
            add { ((PlayerInteractiveViewModel)InteractiveViewModel).Paused += value; }
            remove { ((PlayerInteractiveViewModel)InteractiveViewModel).Paused -= value; }
        }

        public Microsoft.PlayerFramework.Adaptive.AdaptivePlugin adaptivePlugin;
        public Microsoft.Media.AdaptiveStreaming.Dash.DashDownloaderPlugin dashDownloaderPlugin;

        public LiteTubePlayer() : base()
        {
            InteractiveViewModel = DefaultInteractiveViewModel = new PlayerInteractiveViewModel(this);
            IsFullScreenChanged += LiteTubePlayer_IsFullScreenChanged;
            IsRelatedItemsEnabled = true;

            IsLiveChanged += LiteTubePlayer_IsLiveChanged;
            
        }

        private void LiteTubePlayer_IsLiveChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (e.NewValue)
            {
                var asd = new StreamingMediaPlugin();
                Plugins.Add(asd);
                //RegisterPlugins();
                //RegisterDashPlugin(true);
            }
        }

        public string VideoTitle
        {
            get { return GetValue(VideoTitleProperty) as string; }
            set { SetValue(VideoTitleProperty, value); }
        }

        public string ChannelTitle
        {
            get { return GetValue(ChannelTitleProperty) as string; }
            set { SetValue(ChannelTitleProperty, value); }
        }

        public ObservableCollection<NodeViewModelBase> RelatedItems
        {
            get { return GetValue(RelatedItemsProperty) as ObservableCollection<NodeViewModelBase>; }
            set { SetValue(RelatedItemsProperty, value); }
        }

        public List<VideoQualityItem> VideoQualityItems
        {
            get { return GetValue(VideoQualityItemsProperty) as List<VideoQualityItem>; }
            set { SetValue(VideoQualityItemsProperty, value); }

        }

        public VideoQualityItem SelectedVideoQualityItem
        {
            get { return (VideoQualityItem)GetValue(SelectedVideoQualityItemProperty); }
            set { SetValue(SelectedVideoQualityItemProperty, value); }
        }

        public ICommand ItemClickCommand
        {
            get { return GetValue(ItemClickCommandProperty) as ICommand; }
            set { SetValue(ItemClickCommandProperty, value); }
        }

        public ICommand LoadMoreCommand
        {
            get { return GetValue(LoadMoreCommandProperty) as ICommand; }
            set { SetValue(LoadMoreCommandProperty, value); }
        }

        public bool IsRelatedItemsVisible
        {
            get { return (bool)GetValue(RelatedItemsVisibleProperty); }
            set { SetValue(RelatedItemsVisibleProperty, value); }
        }

        public bool IsRelatedItemsEnabled
        {
            get { return (bool)GetValue(RelatedItemsEnabledProperty); }
            set { SetValue(RelatedItemsEnabledProperty, value); }
        }

        private void LiteTubePlayer_IsFullScreenChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!IsFullScreen)
            {
                _isRelatedVisible = IsRelatedItemsVisible;
                IsRelatedItemsVisible = false;
                return;
            }

            IsRelatedItemsVisible = _isRelatedVisible;
        }

        void RegisterDashPlugin(bool bRegister)
        {
            Microsoft.PlayerFramework.Adaptive.AdaptivePlugin plugin = Plugins.OfType<Microsoft.PlayerFramework.Adaptive.AdaptivePlugin>().First();
            if (plugin != null)
            {
                if (bRegister == true)
                {
                    //LogMessage("Register DASH plugin");
                    if (dashDownloaderPlugin == null)
                        dashDownloaderPlugin = new Microsoft.Media.AdaptiveStreaming.Dash.DashDownloaderPlugin();
                    plugin.DownloaderPlugin = dashDownloaderPlugin;
                }
            }
        }
        void RegisterPlugins()
        {
            //LogMessage("Register plugins");
            if (adaptivePlugin == null)
                adaptivePlugin = new Microsoft.PlayerFramework.Adaptive.AdaptivePlugin();
            Plugins.Add(adaptivePlugin);
        }

        private static void OnVideoQualityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var player = d as MediaPlayer;
            if (e.NewValue == null)
                return;

            var quality = ((VideoQualityItem)e.NewValue).Quality;
            player.Tag = GetQuality(quality);
        }

        private static StreamVideo.VideoQuality GetQuality(YouTubeQuality quality)
        {
            return (StreamVideo.VideoQuality) (int) quality;
        }

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //}
    }

    class PlayerInteractiveViewModel : InteractiveViewModel
    {
        public event RoutedEventHandler IsSkipNextChanged;
        public event RoutedEventHandler IsSkipPreviousChanged;
        public event RoutedEventHandler Paused;

        public PlayerInteractiveViewModel(MediaPlayer mediaPlayer) : base(mediaPlayer)
        {
        }

        public string VideoTitle
        {
            get
            {
                var ltPlayer = MediaPlayer as LiteTubePlayer;
                if (ltPlayer == null)
                    return string.Empty;

                return ltPlayer.VideoTitle;
            }
        }

        public string ChannelTitle
        {
            get
            {
                var ltPlayer = MediaPlayer as LiteTubePlayer;
                if (ltPlayer == null)
                    return string.Empty;

                return ltPlayer.ChannelTitle;
            }
        }

        public ObservableCollection<NodeViewModelBase> RelatedItems
        {
            get
            {
                var ltPlayer = MediaPlayer as LiteTubePlayer;
                if (ltPlayer == null)
                    return new ObservableCollection<NodeViewModelBase>();

                return ltPlayer.RelatedItems;
            }
        }

        public ICommand ItemClickCommand
        {
            get
            {
                var ltPlayer = MediaPlayer as LiteTubePlayer;
                if (ltPlayer == null)
                    return new DelegateCommand(() => { });

                return ltPlayer.ItemClickCommand;
            }
        }

        public ICommand LoadMoreCommand
        {
            get
            {
                var ltPlayer = MediaPlayer as LiteTubePlayer;
                if (ltPlayer == null)
                    return new DelegateCommand(() => { });

                return ltPlayer.LoadMoreCommand;
            }
        }

        public override bool IsSkipNextEnabled
        {
            get
            {
                var last = RelatedItems.LastOrDefault();
                var lastNode = last as VideoItemViewModel;
                if (lastNode == null)
                    return base.IsSkipNextEnabled;

                return !lastNode.IsNowPlaying && base.IsSkipNextEnabled;
            }
        }

        public override bool IsSkipPreviousEnabled
        {
            get
            {
                var first = RelatedItems.FirstOrDefault();
                var firstNode = first as VideoItemViewModel;
                if (firstNode == null)
                    return base.IsSkipPreviousEnabled;

                return !firstNode.IsNowPlaying && base.IsSkipPreviousEnabled;
            }
        }

        protected override void OnSkipNext(VisualMarker marker)
        {
            IsSkipNextChanged?.Invoke(this, new RoutedEventArgs());
        }

        protected override void OnSkipPrevious(VisualMarker marker)
        {
            IsSkipPreviousChanged?.Invoke(this, new RoutedEventArgs());
        }

        protected override void OnPause()
        {
            base.OnPause();
            Paused?.Invoke(this, new RoutedEventArgs());
        }
    }

    public class VideoQualityItem : PropertyChangedBase
    {
        private readonly string _qualityName;
        private readonly VideoQuality _qualityConverter;
        private bool _isSelected;

        public VideoQualityItem(string qualityName, bool isSelected)
        {
            _qualityName = qualityName;
            _isSelected = isSelected;
            _qualityConverter = new VideoQuality();
        }

        public string QualityName
        {
            get { return _qualityName; }
        }

        public YouTubeQuality Quality
        {
            get { return _qualityConverter.GetQualityEnum(_qualityName); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChanged(() => IsSelected);
            }
        }
    }
}
