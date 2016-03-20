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
        private static readonly DependencyProperty ItemClickCommandProperty = DependencyProperty.Register("ItemClickCommand", typeof(ICommand), typeof(LiteTubePlayer), null);


        public LiteTubePlayer() : base()
        {
            InteractiveViewModel = DefaultInteractiveViewModel = new PlayerInteractiveViewModel(this);
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
    }

    class PlayerInteractiveViewModel : InteractiveViewModel
    {
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
    }
}
