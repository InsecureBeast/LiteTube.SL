﻿using System;
using System.Linq;
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
using LiteTube.Common;
using Microsoft.PlayerFramework;

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
        private MediaState _deactivatedState;
        private MediaState _playerState;
        private bool _resumed;
        private TimeSpan _playerPosition;

        public VideoPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Pivot.SelectionChanged += PivotOnSelectionChanged;
            player.IsFullScreenChanged += PlayerIsFullScreenChanged;
            player.MediaOpened += PlayerOnMediaOpened;
            CommentTextBox.GotFocus += CommentTextBoxOnGotFocus;
            CommentTextBox.LostFocus += CommentTextBoxOnLostFocus;
            CommentTextBox.TextChanged += CommentTextBoxOnTextChanged;
            
            _sensor = SimpleOrientationSensor.GetDefault();

            _sendApplicationBar = new ApplicationBar();
            _sendApplicationBarButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Send.png", "Send", Send_Click);
            _sendApplicationBar.Buttons.Add(_sendApplicationBarButton);

            _currentApplicationBar = new ApplicationBar();
            _currentApplicationBar.Mode = ApplicationBarMode.Minimized;
            _currentApplicationBar.Buttons.Add(ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Home.png", "Home", Home_Click));
            _currentApplicationBar.MenuItems.Add(ApplicationBarHelper.CreateAApplicationBarMenuItem("Copy video url", CopyVideoUrl_Click));

            _favoritesApplicationBarButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.StarAdd.png", "Add to favorites", AddToFavorites_Click);

            ApplicationBar = _currentApplicationBar;
        }

        private void PlayerOnMediaOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!_resumed) 
                return;

            LayoutHelper.InvokeFromUIThread(() =>
            {
                _resumed = false;
                player.Position = _playerPosition;
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
            NavigationHelper.OnNavigatedTo(this);
            
            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            if (viewModel.NavigationPanelViewModel.IsAuthorized)
            {
                if (_currentApplicationBar.Buttons.Contains(_favoritesApplicationBarButton))
                    return;
                _currentApplicationBar.Buttons.Add(_favoritesApplicationBarButton);
            }

            string pos;
            if (NavigationContext.QueryString.TryGetValue("pos", out pos))
            {
                _resumed = true;

                //pos exists therefore it's a reload, so delete the last entry
                //from the navigation stack
                var str = string.Format("/VideoPage.xaml?videoId={0}", viewModel.VideoId);
                if (!NavigationHelper.Contains(str))
                    return;
                
                if (NavigationService.CanGoBack)
                    NavigationService.RemoveBackEntry();

                _playerPosition = TimeSpan.Parse(pos);
            }

            _sensor.OrientationChanged += Sensor_OrientationChanged;
            PhoneApplicationService.Current.Deactivated += Current_Deactivated;
            PhoneApplicationService.Current.Activated += Current_Activated;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _playerState = player.GetMediaState();
            _sensor.OrientationChanged -= Sensor_OrientationChanged;
            base.OnNavigatedFrom(e);
        }

        private void Sensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            LayoutHelper.InvokeFromUIThread(() => 
            {
                if (args.Orientation == SimpleOrientation.Rotated90DegreesCounterclockwise)
                {
                    SupportedOrientations = SupportedPageOrientation.Landscape;
                    Orientation = PageOrientation.LandscapeRight;
                    return;
                }
                if (args.Orientation == SimpleOrientation.Rotated270DegreesCounterclockwise)
                {
                    SupportedOrientations = SupportedPageOrientation.Landscape;
                    Orientation = PageOrientation.LandscapeLeft;
                    return;
                }

                if (args.Orientation != SimpleOrientation.NotRotated) 
                    return;
                
                SupportedOrientations = SupportedPageOrientation.Portrait;
                Orientation = PageOrientation.Portrait;
            });
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);

            if (e.Orientation == PageOrientation.LandscapeLeft ||
                e.Orientation == PageOrientation.LandscapeRight ||
                e.Orientation == PageOrientation.Landscape)
            {
                SetPlayerFullScreenState();
                SetVisibilityControls(Visibility.Collapsed);
                return;
            }

            SetPlayerNormalState();
            SetVisibilityControls(Visibility.Visible);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var actualWidth = App.Current.Host.Content.ActualWidth;
            var actualHeight = App.Current.Host.Content.ActualHeight;
            var minSize = actualHeight > actualWidth ? actualHeight : actualWidth;
            var maxSize = actualHeight > actualWidth ? actualWidth : actualHeight;
            _normalHeight = minSize;
            _normalWidth = maxSize;

            SetPlayerNormalState();
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
                    if (!viewModel.RelatedVideosViewModel.IsEmpty)
                        await viewModel.RelatedVideosViewModel.FirstLoad();
                    break;

                case 2:
                    Debug.WriteLine("comments");
                    if (!viewModel.CommentsViewModel.IsEmpty)
                        await viewModel.CommentsViewModel.FirstLoad();
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
            playerCanvas.Width = player.Width;
            playerCanvas.Height = player.Height;
            PlayerMover.Y = 0;
            PaidTextBlock.Width = player.Width;
            PaidTextBlock.Height = player.Height;
        }

        private void SetPlayerFullScreenState()
        {
            player.IsFullScreen = true;
            player.Width = _normalHeight;
            player.Height = _normalWidth;
            playerCanvas.Width = player.Width;
            playerCanvas.Height = player.Height;
            PlayerMover.Y = 0;
            PaidTextBlock.Width = player.Width;
            PaidTextBlock.Height = player.Height;
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
            if (player.IsFullScreen)
            {
                SupportedOrientations = SupportedPageOrientation.Landscape; 
                Orientation = PageOrientation.LandscapeLeft;
                return;
            }

            SupportedOrientations = SupportedPageOrientation.Portrait;
            Orientation = PageOrientation.Portrait;
        }

        private void Send_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            viewModel.CommentsViewModel.AddCommentCommand.Execute(null);
        }

        private void AddToFavorites_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            viewModel.AddFavoritesCommand.Execute(null);
        }

        private void CopyVideoUrl_Click(object sender, EventArgs eventArgs)
        {
            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            Clipboard.SetText(string.Format("https://www.youtube.com/watch?v={0}", viewModel.VideoId));
        }

        private void Current_Activated(object sender, ActivatedEventArgs e)
        {
            if (_deactivatedState != null)
            {

                PhoneApplicationService.Current.Deactivated -= Current_Deactivated;
                PhoneApplicationService.Current.Activated -= Current_Activated;

                LayoutHelper.InvokeFromUIThread(() => 
                {
                    _resumed = true;
                    player.RestoreMediaState(_deactivatedState);
                    
                    var viewModel = DataContext as VideoPageViewModel;
                    if (viewModel == null)
                        return;

                    //viewModel.Reload();

                    var view = string.Format("/VideoPage.xaml?videoId={0}&pos={1}&random={2}", viewModel.VideoId, _playerPosition, Guid.NewGuid());
                    NavigationHelper.Navigate(view, viewModel);
                });
            }
        }

        private void Current_Deactivated(object sender, DeactivatedEventArgs e)
        {
            player.Close(); // shut things like ads down.
            _deactivatedState = _playerState;
            _playerPosition = player.VirtualPosition;
        }
    }
}