using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LiteTube.Common.Tools;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LiteTube.Common.Helpers;
using LiteTube.ViewModels;
using System.Diagnostics;
using Windows.Devices.Sensors;
using Microsoft.PlayerFramework;
using LiteTube.Resources;
using LiteTube.Controls;
using System.Threading.Tasks;
using System.Windows.Data;
using LiteTube.Tools;

namespace LiteTube
{
    public partial class PlaylistVideoPage : PhoneApplicationPage
    {
        private double _normalHeight;
        private double _normalWidth;
        private readonly SimpleOrientationSensor _sensor;
        private readonly IApplicationBar _currentApplicationBar;
        private readonly IApplicationBar _sendApplicationBar;
        private readonly ApplicationBarIconButton _sendApplicationBarButton;
        private readonly ApplicationBarIconButton _favoritesApplicationBarButton;
        private MediaState _playerState;
        private bool _resumed;
        private TimeSpan _playerPosition;
        private bool _isFullScreen = false;
        private bool _isRelatedLoading = false;
        private const string RelatedListBoxName = "RelatedListBox";

        public PlaylistVideoPage()
        {
            InitializeComponent();
            Pivot.SelectionChanged += PivotOnSelectionChanged;
            SubscribePlayerEvents(player);
            CommentTextBox.GotFocus += CommentTextBoxOnGotFocus;
            CommentTextBox.LostFocus += CommentTextBoxOnLostFocus;
            CommentTextBox.TextChanged += CommentTextBoxOnTextChanged;
            PhoneApplicationService.Current.Deactivated += Current_Deactivated;
            PhoneApplicationService.Current.Activated += Current_Activated;
            LayoutRoot.SizeChanged += OnLayoutRootSizeChanged;

            _sensor = SimpleOrientationSensor.GetDefault();

            _sendApplicationBar = new ApplicationBar();
            _sendApplicationBarButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Send.png", AppResources.Send, Send_Click);
            _sendApplicationBar.Buttons.Add(_sendApplicationBarButton);

            _currentApplicationBar = new ApplicationBar();
            _currentApplicationBar.Mode = ApplicationBarMode.Minimized;
            _currentApplicationBar.Buttons.Add(ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Home.png", AppResources.Home, Home_Click));
            _currentApplicationBar.MenuItems.Add(ApplicationBarHelper.CreateAApplicationBarMenuItem(AppResources.CopyVideoLink, CopyVideoUrl_Click));

            _favoritesApplicationBarButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.StarAdd.png", AppResources.AddToFavorites, AddToFavorites_Click);

            ApplicationBar = _currentApplicationBar;
        }

        private void OnCurrentStateChanged(object sender, RoutedEventArgs e)
        {
            var media = e.OriginalSource as MediaElement;
            if (media == null)
                return;

            switch (media.CurrentState)
            {
                case System.Windows.Media.MediaElementState.Closed:
                    player.IsRelatedItemsEnabled = false;
                    break;
                case System.Windows.Media.MediaElementState.Opening:
                    player.IsRelatedItemsEnabled = true;
                    break;
                case System.Windows.Media.MediaElementState.Individualizing:
                    break;
                case System.Windows.Media.MediaElementState.AcquiringLicense:
                    break;
                case System.Windows.Media.MediaElementState.Buffering:
                    break;
                case System.Windows.Media.MediaElementState.Playing:
                    break;
                case System.Windows.Media.MediaElementState.Paused:
                    break;
                case System.Windows.Media.MediaElementState.Stopped:
                    break;
                default:
                    player.IsRelatedItemsEnabled = true;
                    break;
            }
        }

        private void OnMediaEnded(object sender, MediaPlayerActionEventArgs e)
        {
            var viewModel = DataContext as PlaylistVideoPageViewModel;
            if (viewModel == null)
                return;

            viewModel.SkipNext();
            Skip();
        }

        private void Skip()
        {
            ScrollIntoView(player, RelatedListBoxName);
            ScrollIntoView(playlistPresenter);
        }

        private void OnSkipPreviousChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as PlaylistVideoPageViewModel;
            if (viewModel == null)
                return;

            viewModel.SkipPrevious();
            Skip();
        }

        private void OnSkipNextChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as PlaylistVideoPageViewModel;
            if (viewModel == null)
                return;

            viewModel.SkipNext();
            Skip();
        }

        private void ScrollIntoView(FrameworkElement element, string name = null)
        {
            var viewModel = DataContext as PlaylistVideoPageViewModel;
            if (viewModel == null)
                return;

            var listBox = VisualHelper.FindListBox(element, name);
            if (listBox == null)
                return;

            if (name != null)
            {
                if (listBox.Name != name)
                    return;
            }

            var item = viewModel.PlaylistVideosViewModel.GetNowPlayingVideo();
            listBox.ScrollIntoView(item);
        }

        private void PlayerOnMediaOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            LayoutHelper.InvokeFromUiThread(() =>
            {
                var viewModel = DataContext as PlaylistVideoPageViewModel;
                if (viewModel == null)
                    return;

                _resumed = false;
                player.Position = _playerPosition;
            });
        }

        private void CommentTextBoxOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var viewModel = DataContext as PlaylistVideoPageViewModel;
            if (viewModel == null)
                return;

            viewModel.VideoViewModel.CommentsViewModel.CommentText = CommentTextBox.Text;
            _sendApplicationBarButton.IsEnabled = !string.IsNullOrEmpty(CommentTextBox.Text);
        }

        private void CommentTextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            ApplicationBar = _currentApplicationBar;
        }

        private void CommentTextBoxOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            ApplicationBar = _sendApplicationBar;
            _sendApplicationBarButton.IsEnabled = !string.IsNullOrEmpty(CommentTextBox.Text);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(this);

            SubscribeModelEvents();
            _sensor.OrientationChanged += Sensor_OrientationChanged;
            
            if (VideoPageViewHelper.IsLandscapeOrientation(Orientation))
                SetVisibilityControls(Visibility.Collapsed);
            else
                SetVisibilityControls(Visibility.Visible);

            var viewModel = DataContext as PlaylistVideoPageViewModel;
            if (viewModel == null)
                return;

            if (viewModel.NavigationPanelViewModel == null)
                return;

            var binding = new Binding { Source = viewModel, Path = new PropertyPath("VideoViewModel.VideoUri") };
            player.SetBinding(LiteTubePlayer.SourceProperty, binding);

            if (viewModel.NavigationPanelViewModel.IsAuthorized)
            {
                if (_currentApplicationBar.Buttons.Contains(_favoritesApplicationBarButton))
                    return;
                _currentApplicationBar.Buttons.Add(_favoritesApplicationBarButton);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            try
            {
                _playerState = player.GetMediaState();
                _sensor.OrientationChanged -= Sensor_OrientationChanged;
                base.OnNavigatedFrom(e);
            }
            catch (Exception)
	        {
            }
        }

        private void Sensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            VideoPageViewHelper.SensorOrientationChanged(this, args);
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);
            ChangeOrientation(e.Orientation);
        }

        private void OnLayoutRootSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double gridWidth = e.NewSize.Width > e.NewSize.Height ? e.NewSize.Width : e.NewSize.Height;
            double gridHeight = e.NewSize.Width > e.NewSize.Height ? e.NewSize.Height : e.NewSize.Width;

            _normalHeight = gridWidth;
            _normalWidth = gridHeight;

            ChangeOrientation(Orientation);
        }

        private async void PivotOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = sender as Pivot;
            if (pivot == null)
                return;

            var viewModel = DataContext as PlaylistVideoPageViewModel;
            if (viewModel == null)
                return;

            switch (pivot.SelectedIndex)
            {
                case 0:
                    Debug.WriteLine("video");
                    break;

                case 1:
                    Debug.WriteLine("details");
                    //await LoadRelatedItems();
                    //TODO load info
                    break;

                case 2:
                    Debug.WriteLine("comments");
                    if (!viewModel.VideoViewModel.CommentsViewModel.IsEmpty)
                        await viewModel.VideoViewModel.CommentsViewModel.FirstLoad();
                    break;
            }
        }

        private void Home_Click(object sender, EventArgs e)
        {
            NavigationHelper.GoHome();
        }

        private void SetPlayerNormalState()
        {
            player.IsFullScreen = false;
            player.Width = _normalWidth;
            player.Height = _normalWidth / 1.778;
            player.AutoHideInterval = TimeSpan.FromSeconds(3);
            PlayerMover.Y = 0;
            PaidTextBlock.Width = player.Width;
            PaidTextBlock.Height = player.Height;
            playerBg.Width = player.Width;
            playerBg.Height = player.Height;
        }

        private void SetPlayerFullScreenState()
        {
            player.IsFullScreen = true;
            player.Width = _normalHeight;
            player.Height = _normalWidth;
            player.AutoHideInterval = TimeSpan.FromSeconds(15);
            PlayerMover.Y = 0;
            PlayerMover.X = 0;
            PaidTextBlock.Width = player.Width;
            PaidTextBlock.Height = player.Height;
            playerBg.Width = player.Width;
            playerBg.Height = player.Height;
        }

        private void SetVisibilityControls(Visibility visibility)
        {
            ApplicationBar.IsVisible = visibility == Visibility.Visible;
            NavControl.Visibility = visibility;
            Pivot.Visibility = visibility;
            SystemTray.IsVisible = visibility == Visibility.Visible;
        }

        private void PlayerIsFullScreenChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            _isFullScreen = player.IsFullScreen;
            VideoPageViewHelper.PlayerIsFullScreenChanged(this, player);
        }

        private void Send_Click(object sender, EventArgs e)
        {
             var viewModel = DataContext as PlaylistVideoPageViewModel;
            if (viewModel == null)
                return;

            VideoPageViewHelper.SendComment(viewModel.VideoViewModel);
        }

        private void AddToFavorites_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as PlaylistVideoPageViewModel;
            if (viewModel == null)
                return;

            VideoPageViewHelper.AddToFavorites(viewModel.VideoViewModel);
        }

        private void CopyVideoUrl_Click(object sender, EventArgs eventArgs)
        {
            var viewModel = DataContext as PlaylistVideoPageViewModel;
            if (viewModel == null)
                return;

            VideoPageViewHelper.CopyVideoUrl(viewModel.VideoViewModel);
        }

        private void Current_Activated(object sender, ActivatedEventArgs e)
        {
            try
            {
                if (_playerState == null)
                    return;

                RestorePlayer();
                if (_resumed)
                    return;

                ScrollIntoView(player, RelatedListBoxName);
                ScrollIntoView(playlistPresenter);

                _resumed = true;
            }
            catch (Exception)
            {
                ;
            }
        }

        private void RestorePlayer()
        {
            var viewModel = DataContext as PlaylistVideoPageViewModel;
            if (viewModel == null)
                return;

            //новый
            player = new LiteTubePlayer
            {
                Name = "player",
                IsFullScreenVisible = true,
                IsFullScreenEnabled = true,
                VerticalAlignment = VerticalAlignment.Center,
                IsSkipAheadVisible = false,
                IsSkipBackVisible = false,
                IsSkipNextVisible = true,
                IsSkipPreviousVisible = true,
                AllowMediaStartingDeferrals = false,
                AutoPlay = true,
                Position = TimeSpan.FromSeconds(0)
            };

            var binding = new Binding { Source = viewModel, Path = new PropertyPath("VideoViewModel.VideoUri") };
            player.SetBinding(LiteTubePlayer.SourceProperty, binding);

            binding = new Binding { Source = viewModel, Path = new PropertyPath("VideoViewModel.Title") };
            player.SetBinding(LiteTubePlayer.VideoTitleProperty, binding);

            binding = new Binding { Source = viewModel, Path = new PropertyPath("VideoViewModel.ChannelTitle") };
            player.SetBinding(LiteTubePlayer.ChannelTitleProperty, binding);

            binding = new Binding { Source = viewModel, Path = new PropertyPath("PlaylistVideosViewModel.Items") };
            player.SetBinding(LiteTubePlayer.RelatedItemsProperty, binding);

            binding = new Binding { Source = viewModel, Path = new PropertyPath("PlaylistVideosViewModel.ItemClickCommand") };
            player.SetBinding(LiteTubePlayer.ItemClickCommandProperty, binding);

            binding = new Binding { Source = viewModel, Path = new PropertyPath("PlaylistVideosViewModel.LoadMoreCommand") };
            player.SetBinding(LiteTubePlayer.LoadMoreCommandProperty, binding);

            binding = new Binding { Source = viewModel, Path = new PropertyPath("VideoViewModel.ImagePath") };
            player.SetBinding(LiteTubePlayer.PosterSourceProperty, binding);

            binding = new Binding { Source = viewModel, Path = new PropertyPath("VideoViewModel.VideoQualities") };
            player.SetBinding(LiteTubePlayer.VideoQualityItemsProperty, binding);

            var binding1 = new Binding { Source = viewModel, Path = new PropertyPath("VideoViewModel.SelectedVideoQualityItem"), Mode = BindingMode.TwoWay };
            player.SetBinding(LiteTubePlayer.SelectedVideoQualityItemProperty, binding1);

            playerBg.Children.Clear();
            playerBg.Children.Add(player);

            SubscribePlayerEvents(player);
            player.RestoreMediaState(_playerState);

            if (_isFullScreen)
                SetPlayerFullScreenState();
            else
                SetPlayerNormalState();
        }

        private void SubscribePlayerEvents(LiteTubePlayer player)
        {
            player.IsFullScreenChanged += PlayerIsFullScreenChanged;
            player.MediaOpened += PlayerOnMediaOpened;
            player.IsSkipNextChanged += OnSkipNextChanged;
            player.IsSkipPreviousChanged += OnSkipPreviousChanged;
            player.MediaEnded += OnMediaEnded;
            player.CurrentStateChanged += OnCurrentStateChanged;
        }

        private void UnsubscribePlayerEvents(LiteTubePlayer player)
        {
            player.MediaOpened -= PlayerOnMediaOpened;
            player.MediaEnded -= OnMediaEnded;
            player.CurrentStateChanged -= OnCurrentStateChanged;
        }

        private void Current_Deactivated(object sender, DeactivatedEventArgs e)
        {
            _playerPosition = player.Position;

            UnsubscribePlayerEvents(player);

            player.Close(); // shut things like ads down.
            player.Dispose();
        }

        private void ChangeOrientation(PageOrientation orientation)
        {
            if (VideoPageViewHelper.IsLandscapeOrientation(orientation))
            {
                SetPlayerFullScreenState();
                SetVisibilityControls(Visibility.Collapsed);
                return;
            }

            SetPlayerNormalState();
            SetVisibilityControls(Visibility.Visible);
        }

        private void SubscribeModelEvents()
        {
            var viewModel = DataContext as PlaylistVideoPageViewModel;
            if (viewModel == null)
                return;

            viewModel.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == "VideoViewModel")
                {
                    viewModel.VideoViewModel.PropertyChanged += (ss, aa) =>
                    {
                        //if (aa.PropertyName == "Description")
                        //{

                        //    if (string.IsNullOrEmpty(viewModel.VideoViewModel.Description))
                        //        return;

                        //    HyperlinkHighlighter.HighlightUrls(viewModel.VideoViewModel.Description, descriptionTextBlock);
                        //}

                        if (aa.PropertyName == "SelectedVideoQualityItem")
                        {
                            _playerPosition = player.Position;
                        }
                    };

                    _playerPosition = TimeSpan.FromSeconds(0);
                }
            };
        }
    }
}
