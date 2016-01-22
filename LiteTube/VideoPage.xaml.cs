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
using LiteTube.Common;

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

        public VideoPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Pivot.SelectionChanged += PivotOnSelectionChanged;
            player.IsFullScreenChanged += PlayerIsFullScreenChanged;
            CommentTextBox.GotFocus += CommentTextBoxOnGotFocus;
            CommentTextBox.LostFocus += CommentTextBoxOnLostFocus;
            CommentTextBox.TextChanged += CommentTextBoxOnTextChanged;

            _sensor = SimpleOrientationSensor.GetDefault();

            _sendApplicationBar = new ApplicationBar();
            _sendApplicationBarButton = ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Send.png", "Send", Send_Click);
            _sendApplicationBar.Buttons.Add(_sendApplicationBarButton);

            _currentApplicationBar = new ApplicationBar();
            _currentApplicationBar.Buttons.Add(ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Home.png", "Home", Home_Click));
            _currentApplicationBar.Buttons.Add(ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Find.png", "Find", Find_Click));

            ApplicationBar = _currentApplicationBar;
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _sensor.OrientationChanged -= Sensor_OrientationChanged;
            //player.Dispose();
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
        }

        private void SetPlayerFullScreenState()
        {
            player.IsFullScreen = true;
            player.Width = _normalHeight;
            player.Height = _normalWidth;
            playerCanvas.Width = player.Width;
            playerCanvas.Height = player.Height;
            PlayerMover.Y = 0;
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

        private void Find_Click(object sender, EventArgs e)
        {
            var datasource = App.ViewModel.DataSource;
            NavigationHelper.Navigate("/SearchPage.xaml", new SearchPageViewModel(datasource));
        }

        private void Send_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            viewModel.CommentsViewModel.AddCommentCommand.Execute(null);
        }
    }
}