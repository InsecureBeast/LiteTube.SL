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
        private MediaState _playerState;
        private bool _resumed;
        private TimeSpan _playerPosition;
        private bool _isFullScreen = false;
        private bool _isRelatedLoading = false;

        public VideoPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Pivot.SelectionChanged += PivotOnSelectionChanged;
            player.IsFullScreenChanged += PlayerIsFullScreenChanged;
            player.MediaOpened += PlayerOnMediaOpened;
            player.IsInteractiveChanged += OnInteractiveChanged;
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
            //_currentApplicationBar.Buttons.Add(ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Refresh.png", AppResources.Refresh, Refresh_Click));
            _currentApplicationBar.MenuItems.Add(ApplicationBarHelper.CreateAApplicationBarMenuItem(AppResources.CopyVideoLink, CopyVideoUrl_Click));

            _favoritesApplicationBarButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.StarAdd.png", AppResources.AddToFavorites, AddToFavorites_Click);

            ApplicationBar = _currentApplicationBar;
        }

        private void PlayerOnMediaOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            SettingsHelper.SaveCurrentVideoId(viewModel.VideoUri.AbsolutePath);

            if (!_resumed)
                return;

            LayoutHelper.InvokeFromUiThread(() =>
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

            _sensor.OrientationChanged += Sensor_OrientationChanged;
            
            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            if (viewModel.NavigationPanelViewModel.IsAuthorized)
            {
                if (_currentApplicationBar.Buttons.Contains(_favoritesApplicationBarButton))
                    return;
                _currentApplicationBar.Buttons.Add(_favoritesApplicationBarButton);
            }

            OnOrientationChanged(new OrientationChangedEventArgs(Orientation));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _playerState = player.GetMediaState();
            _sensor.OrientationChanged -= Sensor_OrientationChanged;
            base.OnNavigatedFrom(e);
        }

        private void Sensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            LayoutHelper.InvokeFromUiThread(() => 
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

            OnOrientationChanged(new OrientationChangedEventArgs(Orientation));
        }

        private void OnLayoutRootSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double gridWidth = e.NewSize.Width > e.NewSize.Height ? e.NewSize.Width : e.NewSize.Height;
            double gridHeight = e.NewSize.Width > e.NewSize.Height ? e.NewSize.Height : e.NewSize.Width;

            _normalHeight = gridWidth;
            _normalWidth = gridHeight;

            OnOrientationChanged(new OrientationChangedEventArgs(Orientation));
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
                    if (!viewModel.CommentsViewModel.IsEmpty)
                        await viewModel.CommentsViewModel.FirstLoad();
                    break;
            }
        }

        private async void OnInteractiveChanged(object sender, RoutedEventArgs e)
        {
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

        //private void Refresh_Click(object sender, EventArgs e)
        //{
        //    Reload();
        //}

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
            try
            {
                if (_playerState == null)
                    return;

                //PhoneApplicationService.Current.Deactivated -= Current_Deactivated;
                //PhoneApplicationService.Current.Activated -= Current_Activated;

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

        //private void Reload()
        //{
        //    var videoId = SettingsHelper.GetCurrentVideoId();
        //    var view = string.Format("/VideoPage.xaml?videoId={0}&pos={1}&random={2}", videoId, _playerPosition, Guid.NewGuid());
        //    NavigationHelper.Navigate(view, new VideoPageViewModel(videoId, App.ViewModel.GetDataSource, App.ViewModel.ConnectionListener));
        //}

        private void RestorePlayer()
        {
            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            player = new LiteTubePlayer
            {
                IsFullScreenVisible = true,
                IsFullScreenEnabled = true,
                VerticalAlignment = VerticalAlignment.Center,
                IsSkipAheadVisible = false,
                IsSkipBackVisible = false,
                AllowMediaStartingDeferrals = false,
                VideoTitle = viewModel.Title,
                ChannelTitle = viewModel.ChannelTitle,
                RelatedItems = viewModel.RelatedVideosViewModel.Items,
                ItemClickCommand = viewModel.RelatedVideosViewModel.ItemClickCommand
            };
            player.IsFullScreenChanged += PlayerIsFullScreenChanged;
            player.MediaOpened += PlayerOnMediaOpened;
            player.RestoreMediaState(_playerState);
            playerBg.Children.Add(player);

            if (_isFullScreen)
                SetPlayerFullScreenState();
            else
                SetPlayerNormalState();
        }

        private void Current_Deactivated(object sender, DeactivatedEventArgs e)
        {
            _playerPosition = player.VirtualPosition;
            player.Close(); // shut things like ads down.
            player.Dispose();
        }
    }
}
