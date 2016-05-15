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
using System.Linq;
using System.Net;
using System.IO;
using LiteTube.Common;
using LiteTube.DataModel;

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
        //private Microsoft.PlayerFramework.Adaptive.AdaptivePlugin adaptivePlugin;

        public VideoPage()
        {
            InitializeComponent();
            Pivot.SelectionChanged += PivotOnSelectionChanged;
            // add adaptive plugin in order to play smooth streaming
            //adaptivePlugin = new Microsoft.PlayerFramework.Adaptive.AdaptivePlugin();
            //player.Plugins.Add(adaptivePlugin);

            player.IsFullScreenChanged += PlayerIsFullScreenChanged;
            player.MediaOpened += PlayerOnMediaOpened;
            player.IsInteractiveChanged += OnInteractiveChanged;
            player.MediaFailed += Player_MediaFailed;
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

        private void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(this);

            _sensor.OrientationChanged += Sensor_OrientationChanged;
            
            if (IsLandscapeOrientation(Orientation))
                SetVisibilityControls(Visibility.Collapsed);
            else
                SetVisibilityControls(Visibility.Visible);

            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            if (viewModel.NavigationPanelViewModel.IsAuthorized)
            {
                if (_currentApplicationBar.Buttons.Contains(_favoritesApplicationBarButton))
                    return;
                _currentApplicationBar.Buttons.Add(_favoritesApplicationBarButton);
            }

            var audio = new Microsoft.PlayerFramework.AudioSelectionPlugin();
            player.Plugins.Add(audio);

            //var a_url = new Uri("https://r1---sn-jgxqvapo3-axqe.googlevideo.com/videoplayback?lmt=1462225019377039&sver=3&upn=jgT-rfwv9u8&requiressl=yes&keepalive=yes&ms=au&mt=1462293177&mv=m&dur=5027.561&clen=79850424&ip=178.162.123.135&initcwndbps=3320000&gir=yes&ipbits=0&mm=31&mn=sn-jgxqvapo3-axqe&itag=140&sparams=clen,dur,gir,id,initcwndbps,ip,ipbits,itag,keepalive,lmt,mime,mm,mn,ms,mv,pl,requiressl,source,upn,expire&id=o-APkTxXnHF3MIjsRmAO05Ulr8SbZFFJtD_SFuL5oxRJsS&expire=1462314867&signature=BB27B9C0DF22C231B932FE88B5DB9792DB6A001A.85C550BF86D390B353750468FA7D3084BBB8798C&source=youtube&mime=audio/mp4&pl=17&fexp=9408214,9416126,9416891,9419451,9422596,9425352,9428398,9428657,9431012,9431021,9431449,9433096,9433946,9434003,9434290,9434611,9434633,9435333,9435850&key=yt6");
            //var v_url = new Uri("https://r1---sn-jgxqvapo3-axqe.googlevideo.com/videoplayback?fexp=9406983,9416126,9416891,9422596,9428398,9431012,9433096,9433946,9434290&keepalive=yes&requiressl=yes&sver=3&sparams=clen,dur,gir,id,initcwndbps,ip,ipbits,itag,keepalive,lmt,mime,mm,mn,ms,mv,pl,requiressl,source,upn,expire&gir=yes&clen=70208259&key=yt6&lmt=1462227549148746&ip=178.162.123.135&upn=pFXxN6YIzZ0&mt=1462290245&expire=1462311970&mm=31&ipbits=0&mn=sn-jgxqvapo3-axqe&itag=160&dur=5027.499&initcwndbps=3313750&id=o-AHWwij9ubhRCFpzNrMl_NMaKaLRFMhuKusg6S7OuGkIU&pl=17&source=youtube&mv=m&mime=video/mp4&ms=au&signature=D5B4D5836B7087CF5B24E30B1BE2EF155F7D3692.0BC4C51784DBCD9608418EABD99EF3417FE782AF");

            var v_url = new Uri("https://r3---sn-jgxqvapo3-axqe.googlevideo.com/videoplayback?sparams=clen,dur,gir,id,initcwndbps,ip,ipbits,itag,keepalive,lmt,mime,mm,mn,ms,mv,pl,requiressl,source,upn,expire&clen=1354425&source=youtube&dur=96.899&initcwndbps=3223750&expire=1463338210&sver=3&mv=m&mime=video/mp4&id=o-AH_T86gy3XU984RyYBbcxTueApdO7uThnrmniQsWFjdU&pl=17&mm=31&mn=sn-jgxqvapo3-axqe&ipbits=0&fexp=9416126,9416891,9422596,9428398,9431012,9433096,9433946,9436446&ip=178.162.123.135&requiressl=yes&ms=au&mt=1463316355&gir=yes&signature=22F3B9CB44DA548F52D6FAE0818C3E739BFA5E61.011D465492F2BF50A88A0D2701232F3B3208F73B&upn=10Ti6DOh2U4&keepalive=yes&itag=160&key=yt6&lmt=1463265031055313");
            var a_url = new Uri("https://r3---sn-jgxqvapo3-axqe.googlevideo.com/videoplayback?sparams=clen,dur,gir,id,initcwndbps,ip,ipbits,itag,keepalive,lmt,mime,mm,mn,ms,mv,pl,requiressl,source,upn,expire&clen=1546889&source=youtube&dur=97.338&initcwndbps=3223750&expire=1463338210&sver=3&mv=m&mime=audio/mp4&id=o-AH_T86gy3XU984RyYBbcxTueApdO7uThnrmniQsWFjdU&pl=17&mm=31&mn=sn-jgxqvapo3-axqe&ipbits=0&fexp=9416126,9416891,9422596,9428398,9431012,9433096,9433946,9436446&ip=178.162.123.135&requiressl=yes&ms=au&mt=1463316355&gir=yes&signature=05153BF65E6F15D5C3F137A436B322D00CA74DB6.9CD1CAEC3F53C8C9E061D96FD400E160D74B3B5A&upn=10Ti6DOh2U4&keepalive=yes&itag=140&key=yt6&lmt=1463264947925549");


            Stream vstream = await YouTubeWeb.HttpGetStreamAsync(v_url.AbsoluteUri);
            Stream astream = await YouTubeWeb.HttpGetStreamAsync(a_url.AbsoluteUri);
            var mss = new VideoStreamSource(vstream, astream);// (int)_normalWidth, (int)_normalHeight);
            //player.SetSource(mss);
            //player.Source = v_url;
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
            _playerPosition = player.VirtualPosition;
            player.Close(); // shut things like ads down.
            player.Dispose();
        }

        private void ChangeOrientation(PageOrientation orientation)
        {
            if (IsLandscapeOrientation(orientation))
            {
                SetPlayerFullScreenState();
                SetVisibilityControls(Visibility.Collapsed);
                return;
            }

            SetPlayerNormalState();
            SetVisibilityControls(Visibility.Visible);
        }

        private static bool IsLandscapeOrientation(PageOrientation orientation)
        {
            return orientation == PageOrientation.LandscapeLeft ||
                   orientation == PageOrientation.LandscapeRight ||
                   orientation == PageOrientation.Landscape;
        }

        private PageOrientation ToPageOrientation(SimpleOrientation orienatation)
        {
            switch (orienatation)
            {
                case SimpleOrientation.NotRotated:
                    return PageOrientation.PortraitUp;
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return PageOrientation.LandscapeLeft;
                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    return PageOrientation.Portrait;
                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return PageOrientation.LandscapeRight;
                case SimpleOrientation.Faceup:
                    break; 
                case SimpleOrientation.Facedown:
                    break;
                default:
                    break;
            }

            return Orientation;
        }
    }
}
