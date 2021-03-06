﻿using System;
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
using System.Linq;
using System.Windows.Data;

namespace LiteTube
{
    public partial class VideoPage : PhoneApplicationPage
    {
        private double _normalHeight;
        private double _normalWidth;
        private readonly SimpleOrientationSensor _sensor;
        private readonly IApplicationBar _currentApplicationBar;
        private readonly IApplicationBar _sendApplicationBar;
        private readonly ApplicationBarIconButton _sendApplicationBarButton;
        private readonly ApplicationBarIconButton _favoritesApplicationBarButton;
        private readonly ApplicationBarIconButton _addToPlaylistApplicationBarButton;
        private MediaState _playerState;
        private bool _resumed;
        private TimeSpan _playerPosition;
        private bool _isFullScreen = false;
        private bool _isRelatedLoading = false;
        private bool _isPaused = false;
        private readonly bool _autoPlay = true;

        public VideoPage()
        {
            InitializeComponent();
            _autoPlay = SettingsHelper.GetIsAutoPlayVideo();
            Pivot.SelectionChanged += PivotOnSelectionChanged;
            SubscribePlayerEvents(player);
            player.AutoPlay = _autoPlay;

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
            //_currentApplicationBar.Buttons.Add(ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Refresh.png", AppResources.Refresh, Refresh_Click));
            _currentApplicationBar.MenuItems.Add(ApplicationBarHelper.CreateAApplicationBarMenuItem(AppResources.CopyVideoLink, CopyVideoUrl_Click));

            _favoritesApplicationBarButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.StarAdd.png", AppResources.AddToFavorites, AddToFavorites_Click);
            _addToPlaylistApplicationBarButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Add.png", AppResources.AddToPlaylist, AddToPlaylist_Click);

            ApplicationBar = _currentApplicationBar;
        }

        private void SubscribeModelEvents()
        {
            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;
            
            viewModel.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == "SelectedVideoQualityItem")
                {
                    _playerPosition = player.Position;
                }
            };
        }

        private void PlayerOnMediaOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            LayoutHelper.InvokeFromUiThread(() =>
            {
                var viewModel = DataContext as VideoPageViewModel;
                if (viewModel == null)
                    return;

                player.Position = _playerPosition;

                if (_isPaused || !_autoPlay)
                {
                    player.PlayResume();
                    player.Position = _playerPosition;
                    player.Pause();
                    _isPaused = false;    
                }
            });
        }

        private void CommentTextBoxOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            viewModel.CommentsViewModel.CommentText = CommentTextBox.Text;
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
            try
            {
                NavigationHelper.OnNavigatedTo(this);

                //RestorePlayer();
                SubscribeModelEvents();

                _sensor.OrientationChanged += Sensor_OrientationChanged;

                if (VideoPageViewHelper.IsLandscapeOrientation(Orientation))
                    SetVisibilityControls(Visibility.Collapsed);
                else
                    SetVisibilityControls(Visibility.Visible);


                var viewModel = DataContext as VideoPageViewModel;
                if (viewModel == null)
                    return;

                var binding = new Binding { Source = viewModel, Path = new PropertyPath("VideoUri") };
                player.SetBinding(LiteTubePlayer.SourceProperty, binding);

                if (e.NavigationMode == NavigationMode.Back && viewModel.IsLive)
                {
                    player = null;
                    RestorePlayer();
                    player?.Load();
                }

                if (!viewModel.NavigationPanelViewModel.IsAuthorized) 
                    return;
            
                if (_currentApplicationBar.Buttons.Contains(_favoritesApplicationBarButton))
                    return;
                _currentApplicationBar.Buttons.Add(_favoritesApplicationBarButton);

                if (_currentApplicationBar.Buttons.Contains(_addToPlaylistApplicationBarButton))
                    return;
                _currentApplicationBar.Buttons.Add(_addToPlaylistApplicationBarButton);
            }
            catch (Exception)
            {
                ;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            try
            {
                _playerState = player.GetMediaState();
                _sensor.OrientationChanged -= Sensor_OrientationChanged;
                player.Unload();
                base.OnNavigatedFrom(e);
            }
            catch (Exception)
	        {
                ;
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

            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            switch (pivot.SelectedIndex)
            {
                case 0:
                    Debug.WriteLine("details");
                    break;

                case 1:
                    Debug.WriteLine("related");
                    await LoadRelatedItems();
                    break;

                case 2:
                    Debug.WriteLine("comments");
                    if (viewModel.IsLive)
                        return;

                    var commentsViewModel = viewModel.CommentsViewModel;
                    if (commentsViewModel == null)
                        return;

                    if (!commentsViewModel.IsEmpty)
                        await commentsViewModel.FirstLoad();
                    break;
            }
        }

        private async void OnInteractiveChanged(object sender, RoutedEventArgs e)
        {
            if (player == null)
                return;

            if (!player.IsFullScreen)
                return;

            await LoadRelatedItems();
        }

        private async Task LoadRelatedItems()
        {
            if (_isRelatedLoading)
                return;

            _isRelatedLoading = true;
            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            if (!viewModel.RelatedVideosViewModel.IsEmpty)
                await viewModel.RelatedVideosViewModel.FirstLoad();

            _isRelatedLoading = false;
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

        private async void SetPlayerFullScreenState()
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
            
            await LoadRelatedItems();
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
            VideoPageViewHelper.SendComment(DataContext as VideoPageViewModel);
        }

        private void AddToFavorites_Click(object sender, EventArgs e)
        {
            VideoPageViewHelper.AddToFavorites(DataContext as VideoPageViewModel);
        }

        private void AddToPlaylist_Click(object sender, EventArgs e)
        {
            VideoPageViewHelper.AddToPlaylist(DataContext as VideoPageViewModel);
        }

        private void CopyVideoUrl_Click(object sender, EventArgs eventArgs)
        {
            VideoPageViewHelper.CopyVideoUrl(DataContext as VideoPageViewModel);
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
                _resumed = true;
            }
            catch (Exception)
            {
                ;
            }
        }

        private void RestorePlayer()
        {
            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            //новый
            player = new LiteTubePlayer
            {
                IsFullScreenVisible = true,
                IsFullScreenEnabled = true,
                VerticalAlignment = VerticalAlignment.Center,
                IsSkipAheadVisible = true,
                IsSkipBackVisible = true,
                SkipAheadInterval = TimeSpan.FromSeconds(10),
                SkipBackInterval = TimeSpan.FromSeconds(10),
                AllowMediaStartingDeferrals = false,
                AutoPlay = _autoPlay,
                VideoTitle = viewModel.Title,
                ChannelTitle = viewModel.ChannelTitle,
                RelatedItems = viewModel.RelatedVideosViewModel.Items,
                ItemClickCommand = viewModel.RelatedVideosViewModel.ItemClickCommand,
                LoadMoreCommand = viewModel.RelatedVideosViewModel.LoadMoreCommand,
                VideoQualityItems = viewModel.VideoQualities,
                SelectedVideoQualityItem = viewModel.SelectedVideoQualityItem,
                IsLive = viewModel.IsLive,
                DataContext = DataContext
            };

            var binding = new Binding { Source = viewModel, Path = new PropertyPath("VideoUri") };
            player.SetBinding(LiteTubePlayer.SourceProperty, binding);

            var binding1 = new Binding { Source = viewModel, Path = new PropertyPath("SelectedVideoQualityItem"), Mode = BindingMode.TwoWay };
            player.SetBinding(LiteTubePlayer.SelectedVideoQualityItemProperty, binding1);

            SubscribePlayerEvents(player);

            if (_playerState != null)
                player.RestoreMediaState(_playerState);

            var oldPlayer = playerBg.Children.FirstOrDefault(x => x is LiteTubePlayer);
            if (oldPlayer != null)
                playerBg.Children.Remove(oldPlayer);

            playerBg.Children.Add(player);

            if (_isFullScreen)
                SetPlayerFullScreenState();
            else
                SetPlayerNormalState();
        }

        private void Current_Deactivated(object sender, DeactivatedEventArgs e)
        {
            if (player == null)
                return;

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

        private void SubscribePlayerEvents(LiteTubePlayer tubePlayer)
        {
            tubePlayer.IsFullScreenChanged += PlayerIsFullScreenChanged;
            tubePlayer.MediaOpened += PlayerOnMediaOpened;
            tubePlayer.MediaFailed += TubePlayerOnMediaFailed;
            tubePlayer.CurrentStateChanged += OnCurrentStateChanged;
            tubePlayer.IsInteractiveChanged += OnInteractiveChanged;
            tubePlayer.Paused += Player_Paused;
        }

        private void TubePlayerOnMediaFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArgs)
        {
            
        }

        private void UnsubscribePlayerEvents(LiteTubePlayer tubePlayer)
        {
            tubePlayer.MediaOpened -= PlayerOnMediaOpened;
            tubePlayer.CurrentStateChanged -= OnCurrentStateChanged;
            tubePlayer.IsFullScreenChanged -= PlayerIsFullScreenChanged;
            tubePlayer.IsInteractiveChanged -= OnInteractiveChanged;
        }

        private void Player_Paused(object sender, RoutedEventArgs e)
        {
            _isPaused = true;
            _playerPosition = player.Position;
        }
    }
}
