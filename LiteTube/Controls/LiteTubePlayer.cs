using LiteTube.ViewModels.Nodes;
using Microsoft.PlayerFramework;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace LiteTube.Controls
{
    public class LiteTubePlayer : MediaPlayer
    {
        private static readonly DependencyProperty VideoTitleProperty = DependencyProperty.Register("VideoTitle", typeof(string), typeof(LiteTubePlayer), null);
        private static readonly DependencyProperty ChannelTitleProperty = DependencyProperty.Register("ChannelTitle", typeof(string), typeof(LiteTubePlayer), null);
        private static readonly DependencyProperty RelatedItemsProperty = DependencyProperty.Register("RelatedItems", typeof(ObservableCollection<NodeViewModelBase>), typeof(LiteTubePlayer), null);
        private static readonly DependencyProperty RelatedItemsVisibleProperty = DependencyProperty.Register("IsRelatedItemsVisible", typeof(bool), typeof(LiteTubePlayer), null);
        private static readonly DependencyProperty ItemClickCommandProperty = DependencyProperty.Register("ItemClickCommand", typeof(ICommand), typeof(LiteTubePlayer), null);
        private static readonly DependencyProperty LoadMoreCommandProperty = DependencyProperty.Register("LoadMoreCommand", typeof(ICommand), typeof(LiteTubePlayer), null);

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

        public LiteTubePlayer() : base()
        {
            InteractiveViewModel = DefaultInteractiveViewModel = new PlayerInteractiveViewModel(this);
            IsFullScreenChanged += LiteTubePlayer_IsFullScreenChanged;
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
    }

    class PlayerInteractiveViewModel : InteractiveViewModel
    {
        public event RoutedEventHandler IsSkipNextChanged;
        public event RoutedEventHandler IsSkipPreviousChanged;

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

        protected override void OnSkipNext(VisualMarker marker)
        {
            IsSkipNextChanged?.Invoke(this, new RoutedEventArgs());
        }

        protected override void OnSkipPrevious(VisualMarker marker)
        {
            IsSkipPreviousChanged?.Invoke(this, new RoutedEventArgs());
        }
    }
}
